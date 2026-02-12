using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using WekezaCRM.Domain.Entities;
using WekezaCRM.Domain.Enums;
using WekezaCRM.Infrastructure.Data;
using WekezaCRM.Infrastructure.Repositories;
using Xunit;

namespace WekezaCRM.UnitTests.Repositories;

public class InteractionRepositoryTests : IDisposable
{
    private readonly CRMDbContext _context;
    private readonly InteractionRepository _repository;
    private readonly Customer _testCustomer;

    public InteractionRepositoryTests()
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
            CreatedBy = "Test"
        };

        _context.Customers.Add(_testCustomer);
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Interaction_When_Exists()
    {
        // Arrange
        var interaction = new Interaction
        {
            Id = Guid.NewGuid(),
            CustomerId = _testCustomer.Id,
            Channel = InteractionChannel.Email,
            Subject = "Account Query",
            Description = "Customer asked about balance",
            InteractionDate = DateTime.UtcNow,
            DurationMinutes = 10,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "Agent"
        };

        _context.Interactions.Add(interaction);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(interaction.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(interaction.Id);
        result.Subject.Should().Be("Account Query");
        result.Customer.Should().NotBeNull();
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Null_When_Not_Exists()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _repository.GetByIdAsync(nonExistentId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByCustomerIdAsync_Should_Return_All_Customer_Interactions_Ordered_By_Date()
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
                Subject = "First Interaction",
                Description = "Description 1",
                InteractionDate = now.AddDays(-2),
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "Agent"
            },
            new Interaction
            {
                Id = Guid.NewGuid(),
                CustomerId = _testCustomer.Id,
                Channel = InteractionChannel.CallCenter,
                Subject = "Second Interaction",
                Description = "Description 2",
                InteractionDate = now.AddDays(-1),
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "Agent"
            },
            new Interaction
            {
                Id = Guid.NewGuid(),
                CustomerId = _testCustomer.Id,
                Channel = InteractionChannel.WhatsApp,
                Subject = "Third Interaction",
                Description = "Description 3",
                InteractionDate = now,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "Agent"
            }
        };

        _context.Interactions.AddRange(interactions);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByCustomerIdAsync(_testCustomer.Id);

        // Assert
        result.Should().HaveCount(3);
        result.First().Subject.Should().Be("Third Interaction"); // Most recent first
        result.Last().Subject.Should().Be("First Interaction");
    }

    [Fact]
    public async Task GetAllAsync_Should_Return_All_Interactions_Ordered_By_Date()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var interactions = new[]
        {
            new Interaction
            {
                Id = Guid.NewGuid(),
                CustomerId = _testCustomer.Id,
                Channel = InteractionChannel.Branch,
                Subject = "Interaction 1",
                Description = "Description 1",
                InteractionDate = now.AddHours(-2),
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "Agent"
            },
            new Interaction
            {
                Id = Guid.NewGuid(),
                CustomerId = _testCustomer.Id,
                Channel = InteractionChannel.Web,
                Subject = "Interaction 2",
                Description = "Description 2",
                InteractionDate = now,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "Agent"
            }
        };

        _context.Interactions.AddRange(interactions);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
        result.First().Subject.Should().Be("Interaction 2");
    }

    [Fact]
    public async Task CreateAsync_Should_Add_Interaction_To_Database()
    {
        // Arrange
        var interaction = new Interaction
        {
            Id = Guid.NewGuid(),
            CustomerId = _testCustomer.Id,
            Channel = InteractionChannel.SMS,
            Subject = "New Interaction",
            Description = "New interaction description",
            InteractionDate = DateTime.UtcNow,
            DurationMinutes = 5,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "Agent"
        };

        // Act
        var result = await _repository.CreateAsync(interaction);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(interaction.Id);

        var savedInteraction = await _context.Interactions.FindAsync(interaction.Id);
        savedInteraction.Should().NotBeNull();
        savedInteraction!.Subject.Should().Be("New Interaction");
    }

    [Fact]
    public async Task UpdateAsync_Should_Update_Interaction_In_Database()
    {
        // Arrange
        var interaction = new Interaction
        {
            Id = Guid.NewGuid(),
            CustomerId = _testCustomer.Id,
            Channel = InteractionChannel.Email,
            Subject = "Original Subject",
            Description = "Original description",
            InteractionDate = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "Agent"
        };

        _context.Interactions.Add(interaction);
        await _context.SaveChangesAsync();

        // Act
        interaction.Subject = "Updated Subject";
        interaction.Description = "Updated description";
        interaction.UpdatedAt = DateTime.UtcNow;

        var result = await _repository.UpdateAsync(interaction);

        // Assert
        result.Should().NotBeNull();
        result.Subject.Should().Be("Updated Subject");

        var updatedInteraction = await _context.Interactions.FindAsync(interaction.Id);
        updatedInteraction!.Subject.Should().Be("Updated Subject");
    }

    [Fact]
    public async Task DeleteAsync_Should_Remove_Interaction_From_Database()
    {
        // Arrange
        var interaction = new Interaction
        {
            Id = Guid.NewGuid(),
            CustomerId = _testCustomer.Id,
            Channel = InteractionChannel.MobileApp,
            Subject = "To Delete",
            Description = "This will be deleted",
            InteractionDate = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "Agent"
        };

        _context.Interactions.Add(interaction);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.DeleteAsync(interaction.Id);

        // Assert
        result.Should().BeTrue();

        var deletedInteraction = await _context.Interactions.FindAsync(interaction.Id);
        deletedInteraction.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_Should_Return_False_When_Interaction_Not_Found()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _repository.DeleteAsync(nonExistentId);

        // Assert
        result.Should().BeFalse();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
