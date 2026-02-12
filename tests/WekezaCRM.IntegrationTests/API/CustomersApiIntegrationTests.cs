using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using WekezaCRM.Domain.Entities;
using WekezaCRM.Domain.Enums;
using WekezaCRM.Infrastructure.Data;
using WekezaCRM.Infrastructure.Repositories;
using Xunit;

namespace WekezaCRM.IntegrationTests.API;

public class CustomersApiIntegrationTests : IDisposable
{
    private readonly CRMDbContext _context;
    private readonly CustomerRepository _repository;

    public CustomersApiIntegrationTests()
    {
        var options = new DbContextOptionsBuilder<CRMDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new CRMDbContext(options);
        _repository = new CustomerRepository(_context);
    }

    [Fact]
    public async Task Complete_Customer_Lifecycle_Should_Work()
    {
        // Arrange - Create
        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            FirstName = "Integration",
            LastName = "Test",
            Email = "integration@test.com",
            PhoneNumber = "+254700000000",
            Segment = CustomerSegment.Retail,
            KYCStatus = KYCStatus.Pending,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System"
        };

        // Act - Create
        var created = await _repository.CreateAsync(customer);
        created.Should().NotBeNull();
        created.Id.Should().Be(customer.Id);

        // Act - Read
        var retrieved = await _repository.GetByIdAsync(customer.Id);
        retrieved.Should().NotBeNull();
        retrieved!.FirstName.Should().Be("Integration");

        // Act - Update
        retrieved.FirstName = "Updated";
        retrieved.UpdatedAt = DateTime.UtcNow;
        await _repository.UpdateAsync(retrieved);

        var updated = await _repository.GetByIdAsync(customer.Id);
        updated!.FirstName.Should().Be("Updated");

        // Act - Delete
        var deleted = await _repository.DeleteAsync(customer.Id);
        deleted.Should().BeTrue();

        var notFound = await _repository.GetByIdAsync(customer.Id);
        notFound.Should().BeNull();
    }

    [Fact]
    public async Task Customer_With_Multiple_Accounts_Should_Be_Retrieved_With_Accounts()
    {
        // Arrange
        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            FirstName = "Account",
            LastName = "Holder",
            Email = "accounts@test.com",
            PhoneNumber = "+254711111111",
            Segment = CustomerSegment.SME,
            KYCStatus = KYCStatus.Verified,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System"
        };

        var account1 = new Account
        {
            Id = Guid.NewGuid(),
            CustomerId = customer.Id,
            AccountNumber = "ACC001",
            AccountType = "Savings",
            Balance = 10000,
            Currency = "KES",
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System"
        };

        var account2 = new Account
        {
            Id = Guid.NewGuid(),
            CustomerId = customer.Id,
            AccountNumber = "ACC002",
            AccountType = "Current",
            Balance = 50000,
            Currency = "KES",
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System"
        };

        customer.Accounts.Add(account1);
        customer.Accounts.Add(account2);

        // Act
        await _repository.CreateAsync(customer);
        var retrieved = await _repository.GetByIdAsync(customer.Id);

        // Assert
        retrieved.Should().NotBeNull();
        retrieved!.Accounts.Should().HaveCount(2);
        retrieved.Accounts.Should().Contain(a => a.AccountNumber == "ACC001");
        retrieved.Accounts.Should().Contain(a => a.AccountNumber == "ACC002");
    }

    [Fact]
    public async Task Customers_Can_Be_Filtered_By_Segment()
    {
        // Arrange
        var customers = new[]
        {
            new Customer
            {
                Id = Guid.NewGuid(),
                FirstName = "Retail1",
                LastName = "Customer",
                Email = "retail1@test.com",
                PhoneNumber = "+254700000001",
                Segment = CustomerSegment.Retail,
                KYCStatus = KYCStatus.Verified,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            },
            new Customer
            {
                Id = Guid.NewGuid(),
                FirstName = "Corporate1",
                LastName = "Customer",
                Email = "corporate1@test.com",
                PhoneNumber = "+254700000002",
                Segment = CustomerSegment.Corporate,
                KYCStatus = KYCStatus.Verified,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            },
            new Customer
            {
                Id = Guid.NewGuid(),
                FirstName = "Retail2",
                LastName = "Customer",
                Email = "retail2@test.com",
                PhoneNumber = "+254700000003",
                Segment = CustomerSegment.Retail,
                KYCStatus = KYCStatus.Verified,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            }
        };

        foreach (var customer in customers)
        {
            await _repository.CreateAsync(customer);
        }

        // Act
        var retailCustomers = await _repository.GetBySegmentAsync("Retail");

        // Assert
        retailCustomers.Should().HaveCount(2);
        retailCustomers.Should().AllSatisfy(c => c.Segment.Should().Be(CustomerSegment.Retail));
    }

    [Fact]
    public async Task Customer_Email_Should_Be_Retrievable()
    {
        // Arrange
        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            FirstName = "Email",
            LastName = "Test",
            Email = "unique@email.com",
            PhoneNumber = "+254722222222",
            Segment = CustomerSegment.HighNetWorth,
            KYCStatus = KYCStatus.Verified,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System"
        };

        await _repository.CreateAsync(customer);

        // Act
        var retrieved = await _repository.GetByEmailAsync("unique@email.com");

        // Assert
        retrieved.Should().NotBeNull();
        retrieved!.Email.Should().Be("unique@email.com");
        retrieved.FirstName.Should().Be("Email");
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
