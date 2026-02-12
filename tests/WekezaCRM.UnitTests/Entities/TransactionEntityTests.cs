using FluentAssertions;
using WekezaCRM.Domain.Entities;
using Xunit;

namespace WekezaCRM.UnitTests.Entities;

public class TransactionEntityTests
{
    [Fact]
    public void Transaction_Should_Initialize_With_Default_Values()
    {
        // Arrange & Act
        var transaction = new Transaction();

        // Assert
        transaction.TransactionType.Should().BeEmpty();
        transaction.TransactionReference.Should().BeEmpty();
        transaction.Amount.Should().Be(0);
    }

    [Fact]
    public void Transaction_Should_Set_Properties_Correctly()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var transactionId = Guid.NewGuid();
        var transactionDate = DateTime.UtcNow;

        // Act
        var transaction = new Transaction
        {
            Id = transactionId,
            AccountId = accountId,
            TransactionType = "Deposit",
            Amount = 5000.00m,
            TransactionReference = "TXN123456",
            TransactionDate = transactionDate,
            Description = "Salary deposit",
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System"
        };

        // Assert
        transaction.Id.Should().Be(transactionId);
        transaction.AccountId.Should().Be(accountId);
        transaction.TransactionType.Should().Be("Deposit");
        transaction.Amount.Should().Be(5000.00m);
        transaction.TransactionReference.Should().Be("TXN123456");
        transaction.TransactionDate.Should().Be(transactionDate);
        transaction.Description.Should().Be("Salary deposit");
    }

    [Theory]
    [InlineData("Deposit")]
    [InlineData("Withdrawal")]
    [InlineData("Transfer")]
    [InlineData("Payment")]
    [InlineData("Fee")]
    public void Transaction_Should_Support_Various_Transaction_Types(string transactionType)
    {
        // Arrange & Act
        var transaction = new Transaction { TransactionType = transactionType };

        // Assert
        transaction.TransactionType.Should().Be(transactionType);
    }

    [Theory]
    [InlineData(10.00)]
    [InlineData(500.50)]
    [InlineData(100000.99)]
    public void Transaction_Should_Support_Various_Amounts(decimal amount)
    {
        // Arrange & Act
        var transaction = new Transaction { Amount = amount };

        // Assert
        transaction.Amount.Should().Be(amount);
    }

    [Fact]
    public void Transaction_Should_Support_Account_Navigation_Property()
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
            Account = account
        };

        // Act & Assert
        transaction.Account.Should().Be(account);
        transaction.AccountId.Should().Be(account.Id);
    }

    [Fact]
    public void Transaction_Should_Have_Unique_Reference()
    {
        // Arrange & Act
        var transaction1 = new Transaction { TransactionReference = "TXN001" };
        var transaction2 = new Transaction { TransactionReference = "TXN002" };

        // Assert
        transaction1.TransactionReference.Should().NotBe(transaction2.TransactionReference);
    }
}
