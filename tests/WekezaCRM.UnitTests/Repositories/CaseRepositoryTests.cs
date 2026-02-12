using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using WekezaCRM.Domain.Entities;
using WekezaCRM.Domain.Enums;
using WekezaCRM.Infrastructure.Data;
using WekezaCRM.Infrastructure.Repositories;
using Xunit;

namespace WekezaCRM.UnitTests.Repositories;

public class CaseRepositoryTests : IDisposable
{
    private readonly CRMDbContext _context;
    private readonly CaseRepository _repository;
    private readonly Customer _testCustomer;

    public CaseRepositoryTests()
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
            CreatedBy = "Test"
        };

        _context.Customers.Add(_testCustomer);
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Case_When_Exists()
    {
        // Arrange
        var caseEntity = new Case
        {
            Id = Guid.NewGuid(),
            CustomerId = _testCustomer.Id,
            CaseNumber = "CASE001",
            Title = "Account Issue",
            Description = "Cannot access account",
            Status = CaseStatus.Open,
            Priority = CasePriority.High,
            Category = "Technical",
            SubCategory = "Login",
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "Agent"
        };

        _context.Cases.Add(caseEntity);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(caseEntity.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(caseEntity.Id);
        result.CaseNumber.Should().Be("CASE001");
        result.Title.Should().Be("Account Issue");
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
    public async Task GetByCustomerIdAsync_Should_Return_All_Customer_Cases()
    {
        // Arrange
        var cases = new[]
        {
            new Case
            {
                Id = Guid.NewGuid(),
                CustomerId = _testCustomer.Id,
                CaseNumber = "CASE001",
                Title = "Issue 1",
                Description = "Description 1",
                Status = CaseStatus.Open,
                Priority = CasePriority.Medium,
                Category = "General",
                SubCategory = "Inquiry",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "Agent"
            },
            new Case
            {
                Id = Guid.NewGuid(),
                CustomerId = _testCustomer.Id,
                CaseNumber = "CASE002",
                Title = "Issue 2",
                Description = "Description 2",
                Status = CaseStatus.InProgress,
                Priority = CasePriority.High,
                Category = "Technical",
                SubCategory = "System",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "Agent"
            }
        };

        _context.Cases.AddRange(cases);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByCustomerIdAsync(_testCustomer.Id);

        // Assert
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(c => c.CustomerId.Should().Be(_testCustomer.Id));
    }

    [Fact]
    public async Task GetAllAsync_Should_Return_All_Cases()
    {
        // Arrange
        var cases = new[]
        {
            new Case
            {
                Id = Guid.NewGuid(),
                CustomerId = _testCustomer.Id,
                CaseNumber = "CASE001",
                Title = "Case 1",
                Description = "Description 1",
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
                CaseNumber = "CASE002",
                Title = "Case 2",
                Description = "Description 2",
                Status = CaseStatus.Resolved,
                Priority = CasePriority.Critical,
                Category = "Technical",
                SubCategory = "Bug",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "Agent"
            }
        };

        _context.Cases.AddRange(cases);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(c => c.CaseNumber == "CASE001");
        result.Should().Contain(c => c.CaseNumber == "CASE002");
    }

    [Fact]
    public async Task CreateAsync_Should_Add_Case_To_Database()
    {
        // Arrange
        var caseEntity = new Case
        {
            Id = Guid.NewGuid(),
            CustomerId = _testCustomer.Id,
            CaseNumber = "CASE999",
            Title = "New Case",
            Description = "New case description",
            Status = CaseStatus.Open,
            Priority = CasePriority.Medium,
            Category = "Support",
            SubCategory = "General",
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "Agent"
        };

        // Act
        var result = await _repository.CreateAsync(caseEntity);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(caseEntity.Id);

        var savedCase = await _context.Cases.FindAsync(caseEntity.Id);
        savedCase.Should().NotBeNull();
        savedCase!.Title.Should().Be("New Case");
    }

    [Fact]
    public async Task UpdateAsync_Should_Update_Case_In_Database()
    {
        // Arrange
        var caseEntity = new Case
        {
            Id = Guid.NewGuid(),
            CustomerId = _testCustomer.Id,
            CaseNumber = "CASE100",
            Title = "Original Title",
            Description = "Original description",
            Status = CaseStatus.Open,
            Priority = CasePriority.Low,
            Category = "General",
            SubCategory = "Info",
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "Agent"
        };

        _context.Cases.Add(caseEntity);
        await _context.SaveChangesAsync();

        // Act
        caseEntity.Status = CaseStatus.Resolved;
        caseEntity.Resolution = "Issue resolved";
        caseEntity.ResolvedAt = DateTime.UtcNow;
        caseEntity.UpdatedAt = DateTime.UtcNow;

        var result = await _repository.UpdateAsync(caseEntity);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(CaseStatus.Resolved);
        result.Resolution.Should().Be("Issue resolved");

        var updatedCase = await _context.Cases.FindAsync(caseEntity.Id);
        updatedCase!.Status.Should().Be(CaseStatus.Resolved);
        updatedCase.ResolvedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task DeleteAsync_Should_Remove_Case_From_Database()
    {
        // Arrange
        var caseEntity = new Case
        {
            Id = Guid.NewGuid(),
            CustomerId = _testCustomer.Id,
            CaseNumber = "CASE200",
            Title = "To Delete",
            Description = "This will be deleted",
            Status = CaseStatus.Open,
            Priority = CasePriority.Low,
            Category = "Test",
            SubCategory = "Test",
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "Agent"
        };

        _context.Cases.Add(caseEntity);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.DeleteAsync(caseEntity.Id);

        // Assert
        result.Should().BeTrue();

        var deletedCase = await _context.Cases.FindAsync(caseEntity.Id);
        deletedCase.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_Should_Return_False_When_Case_Not_Found()
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
