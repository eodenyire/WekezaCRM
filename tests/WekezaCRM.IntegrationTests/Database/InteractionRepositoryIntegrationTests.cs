using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using WekezaCRM.Domain.Entities;
using WekezaCRM.Domain.Enums;
using WekezaCRM.Infrastructure.Data;
using WekezaCRM.Infrastructure.Repositories;
using Xunit;

namespace WekezaCRM.IntegrationTests.Database;

public class InteractionRepositoryIntegrationTests : IDisposable
{
    private readonly CRMDbContext _context;
    private readonly InteractionRepository _repository;
    private readonly Customer _testCustomer;

    public InteractionRepositoryIntegrationTests()
    {
        var options = new DbContextOptionsBuilder<CRMDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new CRMDbContext(options);
        _repository = new InteractionRepository(_context);

        _testCustomer = new Customer
        {
            Id = Guid.NewGuid(),
            FirstName = "Test",
            LastName = "Customer",
            Email = "test@example.com",
            PhoneNumber = "+254700000000",
            Segment = CustomerSegment.Retail,
            KYCStatus = KYCStatus.Verified,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System"
        };

        _context.Customers.Add(_testCustomer);
        _context.SaveChanges();
    }

    [Fact]
    public async Task Complete_Interaction_Lifecycle_Should_Work()
    {
        // Arrange
        var interaction = new Interaction
        {
            Id = Guid.NewGuid(),
            CustomerId = _testCustomer.Id,
            Channel = InteractionChannel.Email,
            Subject = "Test Interaction",
            Description = "Test Description",
            InteractionDate = DateTime.UtcNow,
            DurationMinutes = 15,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "Agent"
        };

        // Act - Create
        var created = await _repository.CreateAsync(interaction);
        created.Should().NotBeNull();

        // Act - Read
        var retrieved = await _repository.GetByIdAsync(interaction.Id);
        retrieved.Should().NotBeNull();
        retrieved!.Subject.Should().Be("Test Interaction");

        // Act - Update
        retrieved.Subject = "Updated Subject";
        retrieved.UpdatedAt = DateTime.UtcNow;
        await _repository.UpdateAsync(retrieved);

        var updated = await _repository.GetByIdAsync(interaction.Id);
        updated!.Subject.Should().Be("Updated Subject");

        // Act - Delete
        var deleted = await _repository.DeleteAsync(interaction.Id);
        deleted.Should().BeTrue();

        var notFound = await _repository.GetByIdAsync(interaction.Id);
        notFound.Should().BeNull();
    }

    [Fact]
    public async Task Interactions_Should_Be_Ordered_By_Date_Descending()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var interactions = new[]
        {
            new Interaction
            {
                Id = Guid.NewGuid(),
                CustomerId = _testCustomer.Id,
                Channel = InteractionChannel.Email,
                Subject = "First",
                Description = "First interaction",
                InteractionDate = now.AddDays(-2),
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "Agent"
            },
            new Interaction
            {
                Id = Guid.NewGuid(),
                CustomerId = _testCustomer.Id,
                Channel = InteractionChannel.CallCenter,
                Subject = "Third",
                Description = "Latest interaction",
                InteractionDate = now,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "Agent"
            },
            new Interaction
            {
                Id = Guid.NewGuid(),
                CustomerId = _testCustomer.Id,
                Channel = InteractionChannel.WhatsApp,
                Subject = "Second",
                Description = "Middle interaction",
                InteractionDate = now.AddDays(-1),
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "Agent"
            }
        };

        foreach (var interaction in interactions)
        {
            await _repository.CreateAsync(interaction);
        }

        // Act
        var retrieved = await _repository.GetByCustomerIdAsync(_testCustomer.Id);

        // Assert
        retrieved.Should().HaveCount(3);
        retrieved.First().Subject.Should().Be("Third"); // Most recent
        retrieved.Last().Subject.Should().Be("First"); // Oldest
    }

    [Theory]
    [InlineData(InteractionChannel.Branch)]
    [InlineData(InteractionChannel.CallCenter)]
    [InlineData(InteractionChannel.Email)]
    [InlineData(InteractionChannel.SMS)]
    [InlineData(InteractionChannel.WhatsApp)]
    [InlineData(InteractionChannel.MobileApp)]
    [InlineData(InteractionChannel.Web)]
    [InlineData(InteractionChannel.ATM)]
    public async Task Interactions_Support_All_Channels(InteractionChannel channel)
    {
        // Arrange
        var interaction = new Interaction
        {
            Id = Guid.NewGuid(),
            CustomerId = _testCustomer.Id,
            Channel = channel,
            Subject = "Channel Test",
            Description = "Testing channel",
            InteractionDate = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "Agent"
        };

        // Act
        await _repository.CreateAsync(interaction);
        var retrieved = await _repository.GetByIdAsync(interaction.Id);

        // Assert
        retrieved.Should().NotBeNull();
        retrieved!.Channel.Should().Be(channel);
    }

    [Fact]
    public async Task Customer_Interaction_History_Can_Be_Retrieved()
    {
        // Arrange
        var interactions = Enumerable.Range(1, 10).Select(i => new Interaction
        {
            Id = Guid.NewGuid(),
            CustomerId = _testCustomer.Id,
            Channel = InteractionChannel.Email,
            Subject = $"Interaction {i}",
            Description = $"Description {i}",
            InteractionDate = DateTime.UtcNow.AddDays(-i),
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "Agent"
        }).ToArray();

        foreach (var interaction in interactions)
        {
            await _repository.CreateAsync(interaction);
        }

        // Act
        var history = await _repository.GetByCustomerIdAsync(_testCustomer.Id);

        // Assert
        history.Should().HaveCount(10);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
