using FluentAssertions;
using WekezaCRM.Domain.Entities;
using Xunit;

namespace WekezaCRM.UnitTests.Entities;

public class AccountEntityTests
{
    [Fact]
    public void Account_Should_Initialize_With_Default_Values()
    {
        // Arrange & Act
        var account = new Account();

        // Assert
        account.AccountNumber.Should().BeEmpty();
        account.AccountType.Should().BeEmpty();
        account.Currency.Should().Be("KES"); // Default currency
        account.Balance.Should().Be(0);
        account.IsActive.Should().BeTrue(); // Default is active
    }

    [Fact]
    public void Account_Should_Set_Properties_Correctly()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var accountId = Guid.NewGuid();

        // Act
        var account = new Account
        {
            Id = accountId,
            CustomerId = customerId,
            AccountNumber = "ACC12345678",
            AccountType = "Savings",
            Balance = 50000.50m,
            Currency = "KES",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System"
        };

        // Assert
        account.Id.Should().Be(accountId);
        account.CustomerId.Should().Be(customerId);
        account.AccountNumber.Should().Be("ACC12345678");
        account.AccountType.Should().Be("Savings");
        account.Balance.Should().Be(50000.50m);
        account.Currency.Should().Be("KES");
        account.IsActive.Should().BeTrue();
    }

    [Theory]
    [InlineData("Savings")]
    [InlineData("Current")]
    [InlineData("Fixed Deposit")]
    [InlineData("Loan")]
    public void Account_Should_Support_Various_Account_Types(string accountType)
    {
        // Arrange & Act
        var account = new Account { AccountType = accountType };

        // Assert
        account.AccountType.Should().Be(accountType);
    }

    [Theory]
    [InlineData("KES")]
    [InlineData("USD")]
    [InlineData("EUR")]
    [InlineData("GBP")]
    public void Account_Should_Support_Various_Currencies(string currency)
    {
        // Arrange & Act
        var account = new Account { Currency = currency };

        // Assert
        account.Currency.Should().Be(currency);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(100.50)]
    [InlineData(1000000.99)]
    [InlineData(-500.00)] // Overdraft
    public void Account_Should_Support_Various_Balance_Amounts(decimal balance)
    {
        // Arrange & Act
        var account = new Account { Balance = balance };

        // Assert
        account.Balance.Should().Be(balance);
    }

    [Fact]
    public void Account_Should_Support_Customer_Navigation_Property()
    {
        // Arrange
        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            PhoneNumber = "+254700000000"
        };

        var account = new Account
        {
            Id = Guid.NewGuid(),
            CustomerId = customer.Id,
            AccountNumber = "ACC001",
            Customer = customer
        };

        // Act & Assert
        account.Customer.Should().Be(customer);
        account.CustomerId.Should().Be(customer.Id);
    }

    [Fact]
    public void Account_Should_Support_Transactions_Collection()
    {
        // Arrange
        var account = new Account
        {
            Id = Guid.NewGuid(),
            CustomerId = Guid.NewGuid(),
            AccountNumber = "ACC001",
            AccountType = "Savings",
            Balance = 10000,
            Currency = "KES"
        };

        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            AccountId = account.Id,
            TransactionType = "Deposit",
            Amount = 5000,
            TransactionDate = DateTime.UtcNow
        };

        // Act
        account.Transactions.Add(transaction);

        // Assert
        account.Transactions.Should().HaveCount(1);
        account.Transactions.First().Should().Be(transaction);
    }

    [Fact]
    public void Account_Should_Track_Creation_And_Update_Timestamps()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var account = new Account
        {
            AccountNumber = "ACC001",
            CreatedAt = now,
            CreatedBy = "System",
            UpdatedAt = now.AddDays(1),
            UpdatedBy = "Admin"
        };

        // Assert
        account.CreatedAt.Should().Be(now);
        account.UpdatedAt.Should().Be(now.AddDays(1));
        account.CreatedBy.Should().Be("System");
        account.UpdatedBy.Should().Be("Admin");
    }
}
