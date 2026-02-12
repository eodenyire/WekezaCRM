using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using WekezaCRM.Domain.Entities;
using WekezaCRM.Domain.Enums;
using WekezaCRM.Infrastructure.Data;
using WekezaCRM.Infrastructure.Repositories;
using Xunit;

namespace WekezaCRM.IntegrationTests.Database;

public class CaseRepositoryIntegrationTests : IDisposable
{
    private readonly CRMDbContext _context;
    private readonly CaseRepository _repository;
    private readonly Customer _testCustomer;

    public CaseRepositoryIntegrationTests()
    {
        var options = new DbContextOptionsBuilder<CRMDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new CRMDbContext(options);
        _repository = new CaseRepository(_context);

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
    public async Task Complete_Case_Lifecycle_Should_Work()
    {
        // Arrange - Create
        var caseEntity = new Case
        {
            Id = Guid.NewGuid(),
            CustomerId = _testCustomer.Id,
            CaseNumber = "CASE001",
            Title = "Test Case",
            Description = "Test Description",
            Status = CaseStatus.Open,
            Priority = CasePriority.High,
            Category = "Technical",
            SubCategory = "Login",
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "Agent"
        };

        // Act - Create
        var created = await _repository.CreateAsync(caseEntity);
        created.Should().NotBeNull();

        // Act - Read
        var retrieved = await _repository.GetByIdAsync(caseEntity.Id);
        retrieved.Should().NotBeNull();
        retrieved!.Title.Should().Be("Test Case");

        // Act - Update
        retrieved.Status = CaseStatus.Resolved;
        retrieved.Resolution = "Issue fixed";
        retrieved.ResolvedAt = DateTime.UtcNow;
        await _repository.UpdateAsync(retrieved);

        var updated = await _repository.GetByIdAsync(caseEntity.Id);
        updated!.Status.Should().Be(CaseStatus.Resolved);
        updated.Resolution.Should().Be("Issue fixed");

        // Act - Delete
        var deleted = await _repository.DeleteAsync(caseEntity.Id);
        deleted.Should().BeTrue();

        var notFound = await _repository.GetByIdAsync(caseEntity.Id);
        notFound.Should().BeNull();
    }

    [Fact]
    public async Task Case_With_Multiple_CaseNotes_Should_Be_Retrieved_With_Notes()
    {
        // Arrange
        var caseEntity = new Case
        {
            Id = Guid.NewGuid(),
            CustomerId = _testCustomer.Id,
            CaseNumber = "CASE002",
            Title = "Case with Notes",
            Description = "Test",
            Status = CaseStatus.InProgress,
            Priority = CasePriority.Medium,
            Category = "Support",
            SubCategory = "General",
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "Agent"
        };

        var note1 = new CaseNote
        {
            Id = Guid.NewGuid(),
            CaseId = caseEntity.Id,
            Note = "Customer contacted",
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "Agent"
        };

        var note2 = new CaseNote
        {
            Id = Guid.NewGuid(),
            CaseId = caseEntity.Id,
            Note = "Issue diagnosed",
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "Agent"
        };

        caseEntity.CaseNotes.Add(note1);
        caseEntity.CaseNotes.Add(note2);

        // Act
        await _repository.CreateAsync(caseEntity);
        var retrieved = await _repository.GetByIdAsync(caseEntity.Id);

        // Assert
        retrieved.Should().NotBeNull();
        retrieved!.CaseNotes.Should().HaveCount(2);
    }

    [Fact]
    public async Task Cases_Can_Be_Retrieved_By_Customer()
    {
        // Arrange
        var cases = new[]
        {
            new Case
            {
                Id = Guid.NewGuid(),
                CustomerId = _testCustomer.Id,
                CaseNumber = "CASE003",
                Title = "Case 1",
                Description = "Test",
                Status = CaseStatus.Open,
                Priority = CasePriority.Low,
                Category = "General",
                SubCategory = "Info",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "Agent"
            },
            new Case
            {
                Id = Guid.NewGuid(),
                CustomerId = _testCustomer.Id,
                CaseNumber = "CASE004",
                Title = "Case 2",
                Description = "Test",
                Status = CaseStatus.Resolved,
                Priority = CasePriority.High,
                Category = "Technical",
                SubCategory = "Bug",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "Agent"
            }
        };

        foreach (var caseEntity in cases)
        {
            await _repository.CreateAsync(caseEntity);
        }

        // Act
        var customerCases = await _repository.GetByCustomerIdAsync(_testCustomer.Id);

        // Assert
        customerCases.Should().HaveCount(2);
    }

    [Fact]
    public async Task Case_Status_Transitions_Should_Work()
    {
        // Arrange
        var caseEntity = new Case
        {
            Id = Guid.NewGuid(),
            CustomerId = _testCustomer.Id,
            CaseNumber = "CASE005",
            Title = "Status Test",
            Description = "Testing status transitions",
            Status = CaseStatus.Open,
            Priority = CasePriority.Medium,
            Category = "Test",
            SubCategory = "Test",
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "Agent"
        };

        await _repository.CreateAsync(caseEntity);

        // Act & Assert - Open to InProgress
        caseEntity.Status = CaseStatus.InProgress;
        await _repository.UpdateAsync(caseEntity);
        var retrieved1 = await _repository.GetByIdAsync(caseEntity.Id);
        retrieved1!.Status.Should().Be(CaseStatus.InProgress);

        // Act & Assert - InProgress to Resolved
        caseEntity.Status = CaseStatus.Resolved;
        caseEntity.ResolvedAt = DateTime.UtcNow;
        await _repository.UpdateAsync(caseEntity);
        var retrieved2 = await _repository.GetByIdAsync(caseEntity.Id);
        retrieved2!.Status.Should().Be(CaseStatus.Resolved);
        retrieved2.ResolvedAt.Should().NotBeNull();

        // Act & Assert - Resolved to Closed
        caseEntity.Status = CaseStatus.Closed;
        caseEntity.ClosedAt = DateTime.UtcNow;
        await _repository.UpdateAsync(caseEntity);
        var retrieved3 = await _repository.GetByIdAsync(caseEntity.Id);
        retrieved3!.Status.Should().Be(CaseStatus.Closed);
        retrieved3.ClosedAt.Should().NotBeNull();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
