using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using WekezaCRM.Domain.Entities;
using WekezaCRM.Domain.Enums;
using WekezaCRM.Infrastructure.Data;
using WekezaCRM.Infrastructure.Repositories;
using Xunit;

namespace WekezaCRM.UnitTests.Repositories;

public class CustomerRepositoryTests : IDisposable
{
    private readonly CRMDbContext _context;
    private readonly CustomerRepository _repository;

    public CustomerRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<CRMDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new CRMDbContext(options);
        _repository = new CustomerRepository(_context);
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Customer_When_Exists()
    {
        // Arrange
        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            PhoneNumber = "+254712345678",
            Segment = CustomerSegment.Retail,
            KYCStatus = KYCStatus.Pending,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "Test"
        };
        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(customer.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(customer.Id);
        result.FirstName.Should().Be("John");
        result.LastName.Should().Be("Doe");
        result.Email.Should().Be("john@example.com");
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
    public async Task GetByIdAsync_Should_Include_Related_Entities()
    {
        // Arrange
        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane@example.com",
            PhoneNumber = "+254722222222",
            Segment = CustomerSegment.SME,
            KYCStatus = KYCStatus.Verified,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "Test"
        };

        var account = new Account
        {
            Id = Guid.NewGuid(),
            CustomerId = customer.Id,
            AccountNumber = "ACC001",
            AccountType = "Savings",
            Balance = 10000,
            Currency = "KES",
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "Test"
        };

        customer.Accounts.Add(account);
        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(customer.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Accounts.Should().HaveCount(1);
        result.Accounts.First().AccountNumber.Should().Be("ACC001");
    }

    [Fact]
    public async Task GetByEmailAsync_Should_Return_Customer_When_Exists()
    {
        // Arrange
        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            FirstName = "Alice",
            LastName = "Brown",
            Email = "alice@example.com",
            PhoneNumber = "+254733333333",
            Segment = CustomerSegment.Corporate,
            KYCStatus = KYCStatus.Verified,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "Test"
        };
        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByEmailAsync("alice@example.com");

        // Assert
        result.Should().NotBeNull();
        result!.Email.Should().Be("alice@example.com");
        result.FirstName.Should().Be("Alice");
    }

    [Fact]
    public async Task GetByEmailAsync_Should_Return_Null_When_Not_Exists()
    {
        // Arrange & Act
        var result = await _repository.GetByEmailAsync("nonexistent@example.com");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_Should_Return_All_Customers()
    {
        // Arrange
        var customers = new[]
        {
            new Customer
            {
                Id = Guid.NewGuid(),
                FirstName = "Customer1",
                LastName = "Test1",
                Email = "customer1@example.com",
                PhoneNumber = "+254711111111",
                Segment = CustomerSegment.Retail,
                KYCStatus = KYCStatus.Pending,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "Test"
            },
            new Customer
            {
                Id = Guid.NewGuid(),
                FirstName = "Customer2",
                LastName = "Test2",
                Email = "customer2@example.com",
                PhoneNumber = "+254722222222",
                Segment = CustomerSegment.SME,
                KYCStatus = KYCStatus.Verified,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "Test"
            },
            new Customer
            {
                Id = Guid.NewGuid(),
                FirstName = "Customer3",
                LastName = "Test3",
                Email = "customer3@example.com",
                PhoneNumber = "+254733333333",
                Segment = CustomerSegment.Corporate,
                KYCStatus = KYCStatus.InProgress,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "Test"
            }
        };

        _context.Customers.AddRange(customers);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().HaveCount(3);
        result.Should().Contain(c => c.FirstName == "Customer1");
        result.Should().Contain(c => c.FirstName == "Customer2");
        result.Should().Contain(c => c.FirstName == "Customer3");
    }

    [Fact]
    public async Task GetAllAsync_Should_Return_Empty_List_When_No_Customers()
    {
        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetBySegmentAsync_Should_Return_Customers_By_Segment()
    {
        // Arrange
        var customers = new[]
        {
            new Customer
            {
                Id = Guid.NewGuid(),
                FirstName = "Retail1",
                LastName = "Customer",
                Email = "retail1@example.com",
                PhoneNumber = "+254711111111",
                Segment = CustomerSegment.Retail,
                KYCStatus = KYCStatus.Pending,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "Test"
            },
            new Customer
            {
                Id = Guid.NewGuid(),
                FirstName = "SME1",
                LastName = "Customer",
                Email = "sme1@example.com",
                PhoneNumber = "+254722222222",
                Segment = CustomerSegment.SME,
                KYCStatus = KYCStatus.Verified,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "Test"
            },
            new Customer
            {
                Id = Guid.NewGuid(),
                FirstName = "Retail2",
                LastName = "Customer",
                Email = "retail2@example.com",
                PhoneNumber = "+254733333333",
                Segment = CustomerSegment.Retail,
                KYCStatus = KYCStatus.Verified,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "Test"
            }
        };

        _context.Customers.AddRange(customers);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetBySegmentAsync("Retail");

        // Assert
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(c => c.Segment.Should().Be(CustomerSegment.Retail));
    }

    [Fact]
    public async Task CreateAsync_Should_Add_Customer_To_Database()
    {
        // Arrange
        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            FirstName = "New",
            LastName = "Customer",
            Email = "new@example.com",
            PhoneNumber = "+254744444444",
            Segment = CustomerSegment.HighNetWorth,
            KYCStatus = KYCStatus.Pending,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "Test"
        };

        // Act
        var result = await _repository.CreateAsync(customer);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(customer.Id);
        
        var savedCustomer = await _context.Customers.FindAsync(customer.Id);
        savedCustomer.Should().NotBeNull();
        savedCustomer!.FirstName.Should().Be("New");
    }

    [Fact]
    public async Task UpdateAsync_Should_Update_Customer_In_Database()
    {
        // Arrange
        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            FirstName = "Original",
            LastName = "Name",
            Email = "original@example.com",
            PhoneNumber = "+254755555555",
            Segment = CustomerSegment.Retail,
            KYCStatus = KYCStatus.Pending,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "Test"
        };

        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();

        // Act
        customer.FirstName = "Updated";
        customer.LastName = "Customer";
        customer.UpdatedAt = DateTime.UtcNow;
        customer.UpdatedBy = "Test";

        var result = await _repository.UpdateAsync(customer);

        // Assert
        result.Should().NotBeNull();
        result.FirstName.Should().Be("Updated");
        result.LastName.Should().Be("Customer");
        
        var updatedCustomer = await _context.Customers.FindAsync(customer.Id);
        updatedCustomer!.FirstName.Should().Be("Updated");
        updatedCustomer.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task DeleteAsync_Should_Remove_Customer_From_Database()
    {
        // Arrange
        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            FirstName = "ToDelete",
            LastName = "Customer",
            Email = "delete@example.com",
            PhoneNumber = "+254766666666",
            Segment = CustomerSegment.Retail,
            KYCStatus = KYCStatus.Pending,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "Test"
        };

        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.DeleteAsync(customer.Id);

        // Assert
        result.Should().BeTrue();
        
        var deletedCustomer = await _context.Customers.FindAsync(customer.Id);
        deletedCustomer.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_Should_Return_False_When_Customer_Not_Found()
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
