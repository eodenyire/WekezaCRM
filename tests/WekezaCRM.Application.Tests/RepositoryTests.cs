using Xunit;
using FluentAssertions;
using Moq;
using Microsoft.EntityFrameworkCore;
using WekezaCRM.Application.Interfaces;
using WekezaCRM.Domain.Entities;
using WekezaCRM.Domain.Enums;
using WekezaCRM.Infrastructure.Data;
using WekezaCRM.Infrastructure.Repositories;

namespace WekezaCRM.Application.Tests;

public class CustomerRepositoryTests
{
    private CRMDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<CRMDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new CRMDbContext(options);
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Customer_When_Exists()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new CustomerRepository(context);
        var customerId = Guid.NewGuid();
        
        var customer = new Customer
        {
            Id = customerId,
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            PhoneNumber = "+254712345678",
            Segment = CustomerSegment.Retail,
            KYCStatus = KYCStatus.Verified,
            IsActive = true
        };
        
        context.Customers.Add(customer);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByIdAsync(customerId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(customerId);
        result.FirstName.Should().Be("John");
        result.Email.Should().Be("john@example.com");
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Null_When_Not_Exists()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new CustomerRepository(context);
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await repository.GetByIdAsync(nonExistentId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_Should_Return_All_Customers()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new CustomerRepository(context);
        
        context.Customers.AddRange(
            new Customer { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe", Email = "john@test.com", PhoneNumber = "+254700000001", Segment = CustomerSegment.Retail, KYCStatus = KYCStatus.Pending, IsActive = true },
            new Customer { Id = Guid.NewGuid(), FirstName = "Jane", LastName = "Smith", Email = "jane@test.com", PhoneNumber = "+254700000002", Segment = CustomerSegment.SME, KYCStatus = KYCStatus.Verified, IsActive = true },
            new Customer { Id = Guid.NewGuid(), FirstName = "Bob", LastName = "Johnson", Email = "bob@test.com", PhoneNumber = "+254700000003", Segment = CustomerSegment.Corporate, KYCStatus = KYCStatus.Verified, IsActive = true }
        );
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        result.Should().HaveCount(3);
        result.Should().Contain(c => c.FirstName == "John");
        result.Should().Contain(c => c.FirstName == "Jane");
        result.Should().Contain(c => c.FirstName == "Bob");
    }

    [Fact]
    public async Task GetByEmailAsync_Should_Return_Customer_With_Matching_Email()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new CustomerRepository(context);
        
        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Doe",
            Email = "unique@example.com",
            PhoneNumber = "+254712345678",
            Segment = CustomerSegment.Retail,
            KYCStatus = KYCStatus.Pending,
            IsActive = true
        };
        
        context.Customers.Add(customer);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByEmailAsync("unique@example.com");

        // Assert
        result.Should().NotBeNull();
        result!.Email.Should().Be("unique@example.com");
    }

    [Fact]
    public async Task GetBySegmentAsync_Should_Return_Customers_In_Segment()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new CustomerRepository(context);
        
        context.Customers.AddRange(
            new Customer { Id = Guid.NewGuid(), FirstName = "Retail1", LastName = "Customer", Email = "retail1@test.com", PhoneNumber = "+254700000001", Segment = CustomerSegment.Retail, KYCStatus = KYCStatus.Verified, IsActive = true },
            new Customer { Id = Guid.NewGuid(), FirstName = "SME1", LastName = "Customer", Email = "sme1@test.com", PhoneNumber = "+254700000002", Segment = CustomerSegment.SME, KYCStatus = KYCStatus.Verified, IsActive = true },
            new Customer { Id = Guid.NewGuid(), FirstName = "Retail2", LastName = "Customer", Email = "retail2@test.com", PhoneNumber = "+254700000003", Segment = CustomerSegment.Retail, KYCStatus = KYCStatus.Verified, IsActive = true }
        );
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetBySegmentAsync("Retail");

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(c => c.Segment == CustomerSegment.Retail);
    }

    [Fact]
    public async Task CreateAsync_Should_Add_Customer_To_Database()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new CustomerRepository(context);
        
        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            FirstName = "New",
            LastName = "Customer",
            Email = "new@example.com",
            PhoneNumber = "+254712345678",
            Segment = CustomerSegment.Retail,
            KYCStatus = KYCStatus.Pending,
            IsActive = true
        };

        // Act
        var result = await repository.CreateAsync(customer);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(customer.Id);
        
        var savedCustomer = await context.Customers.FindAsync(customer.Id);
        savedCustomer.Should().NotBeNull();
        savedCustomer!.Email.Should().Be("new@example.com");
    }

    [Fact]
    public async Task UpdateAsync_Should_Modify_Existing_Customer()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new CustomerRepository(context);
        
        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            FirstName = "Original",
            LastName = "Name",
            Email = "original@example.com",
            PhoneNumber = "+254712345678",
            Segment = CustomerSegment.Retail,
            KYCStatus = KYCStatus.Pending,
            IsActive = true
        };
        
        context.Customers.Add(customer);
        await context.SaveChangesAsync();
        
        context.Entry(customer).State = EntityState.Detached;

        // Act
        customer.FirstName = "Updated";
        customer.LastName = "Customer";
        var result = await repository.UpdateAsync(customer);

        // Assert
        result.FirstName.Should().Be("Updated");
        result.LastName.Should().Be("Customer");
        
        var updatedCustomer = await context.Customers.FindAsync(customer.Id);
        updatedCustomer!.FirstName.Should().Be("Updated");
    }

    [Fact]
    public async Task DeleteAsync_Should_Remove_Customer_From_Database()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new CustomerRepository(context);
        
        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            FirstName = "ToDelete",
            LastName = "Customer",
            Email = "delete@example.com",
            PhoneNumber = "+254712345678",
            Segment = CustomerSegment.Retail,
            KYCStatus = KYCStatus.Pending,
            IsActive = true
        };
        
        context.Customers.Add(customer);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.DeleteAsync(customer.Id);

        // Assert
        result.Should().BeTrue();
        
        var deletedCustomer = await context.Customers.FindAsync(customer.Id);
        deletedCustomer.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_Should_Return_False_When_Customer_Not_Found()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new CustomerRepository(context);
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await repository.DeleteAsync(nonExistentId);

        // Assert
        result.Should().BeFalse();
    }
}

public class CaseRepositoryTests
{
    private CRMDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<CRMDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new CRMDbContext(options);
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Case_When_Exists()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new CaseRepository(context);
        var caseId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        
        // Add customer first
        context.Customers.Add(new Customer
        {
            Id = customerId,
            FirstName = "Test",
            LastName = "Customer",
            Email = "test@example.com",
            PhoneNumber = "+254700000000",
            Segment = CustomerSegment.Retail,
            KYCStatus = KYCStatus.Pending,
            IsActive = true
        });
        
        var caseEntity = new Case
        {
            Id = caseId,
            CustomerId = customerId,
            CaseNumber = "CASE-001",
            Title = "Test Case",
            Description = "Test Description",
            Status = CaseStatus.Open,
            Priority = CasePriority.Medium,
            Category = "Technical",
            SubCategory = "Login"
        };
        
        context.Cases.Add(caseEntity);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByIdAsync(caseId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(caseId);
        result.CaseNumber.Should().Be("CASE-001");
    }

    [Fact]
    public async Task GetByCustomerIdAsync_Should_Return_Customer_Cases()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new CaseRepository(context);
        var customerId = Guid.NewGuid();
        
        // Add customer
        context.Customers.Add(new Customer
        {
            Id = customerId,
            FirstName = "Test",
            LastName = "Customer",
            Email = "test@example.com",
            PhoneNumber = "+254700000000",
            Segment = CustomerSegment.Retail,
            KYCStatus = KYCStatus.Pending,
            IsActive = true
        });
        
        context.Cases.AddRange(
            new Case { Id = Guid.NewGuid(), CustomerId = customerId, CaseNumber = "CASE-001", Title = "Case 1", Description = "Desc 1", Status = CaseStatus.Open, Priority = CasePriority.Low, Category = "Tech", SubCategory = "Sub" },
            new Case { Id = Guid.NewGuid(), CustomerId = customerId, CaseNumber = "CASE-002", Title = "Case 2", Description = "Desc 2", Status = CaseStatus.InProgress, Priority = CasePriority.Medium, Category = "Tech", SubCategory = "Sub" }
        );
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByCustomerIdAsync(customerId);

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(c => c.CustomerId == customerId);
    }

    [Fact]
    public async Task CreateAsync_Should_Add_Case_To_Database()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new CaseRepository(context);
        var customerId = Guid.NewGuid();
        
        // Add customer
        context.Customers.Add(new Customer
        {
            Id = customerId,
            FirstName = "Test",
            LastName = "Customer",
            Email = "test@example.com",
            PhoneNumber = "+254700000000",
            Segment = CustomerSegment.Retail,
            KYCStatus = KYCStatus.Pending,
            IsActive = true
        });
        await context.SaveChangesAsync();
        
        var caseEntity = new Case
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            CaseNumber = "CASE-NEW",
            Title = "New Case",
            Description = "New Description",
            Status = CaseStatus.Open,
            Priority = CasePriority.High,
            Category = "Support",
            SubCategory = "General"
        };

        // Act
        var result = await repository.CreateAsync(caseEntity);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(caseEntity.Id);
        
        var savedCase = await context.Cases.FindAsync(caseEntity.Id);
        savedCase.Should().NotBeNull();
        savedCase!.Title.Should().Be("New Case");
    }
}

public class NextBestActionRepositoryTests
{
    private CRMDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<CRMDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new CRMDbContext(options);
    }

    [Fact]
    public async Task GetPendingByCustomerIdAsync_Should_Return_Only_Incomplete_Actions()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new NextBestActionRepository(context);
        var customerId = Guid.NewGuid();
        
        // Add customer
        context.Customers.Add(new Customer
        {
            Id = customerId,
            FirstName = "Test",
            LastName = "Customer",
            Email = "test@example.com",
            PhoneNumber = "+254700000000",
            Segment = CustomerSegment.Retail,
            KYCStatus = KYCStatus.Pending,
            IsActive = true
        });
        
        context.NextBestActions.AddRange(
            new NextBestAction 
            { 
                Id = Guid.NewGuid(), 
                CustomerId = customerId, 
                ActionType = ActionType.ProductRecommendation,
                Title = "Action 1", 
                Description = "Desc 1", 
                ConfidenceScore = 0.9m,
                RecommendedDate = DateTime.UtcNow,
                IsCompleted = false 
            },
            new NextBestAction 
            { 
                Id = Guid.NewGuid(), 
                CustomerId = customerId, 
                ActionType = ActionType.FollowUpCall,
                Title = "Action 2", 
                Description = "Desc 2", 
                ConfidenceScore = 0.8m,
                RecommendedDate = DateTime.UtcNow,
                IsCompleted = true 
            },
            new NextBestAction 
            { 
                Id = Guid.NewGuid(), 
                CustomerId = customerId, 
                ActionType = ActionType.SendEmail,
                Title = "Action 3", 
                Description = "Desc 3", 
                ConfidenceScore = 0.7m,
                RecommendedDate = DateTime.UtcNow,
                IsCompleted = false 
            }
        );
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetPendingByCustomerIdAsync(customerId);

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(a => !a.IsCompleted);
    }
}
