using FluentAssertions;
using WekezaCRM.Domain.Entities;
using WekezaCRM.Domain.Enums;
using Xunit;

namespace WekezaCRM.UnitTests.Entities;

public class CustomerEntityTests
{
    [Fact]
    public void Customer_Should_Initialize_With_Default_Values()
    {
        // Arrange & Act
        var customer = new Customer();

        // Assert
        customer.FirstName.Should().BeEmpty();
        customer.LastName.Should().BeEmpty();
        customer.Email.Should().BeEmpty();
        customer.PhoneNumber.Should().BeEmpty();
        customer.IsActive.Should().BeTrue();
        customer.Accounts.Should().BeEmpty();
        customer.Interactions.Should().BeEmpty();
        customer.Cases.Should().BeEmpty();
        customer.Campaigns.Should().BeEmpty();
    }

    [Fact]
    public void Customer_Should_Set_Properties_Correctly()
    {
        // Arrange & Act
        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            PhoneNumber = "+254712345678",
            DateOfBirth = new DateTime(1990, 1, 1),
            Address = "123 Main Street",
            City = "Nairobi",
            Country = "Kenya",
            Segment = CustomerSegment.Retail,
            KYCStatus = KYCStatus.Verified,
            CustomerReference = "CUST001",
            CreditScore = 750m,
            LifetimeValue = 100000m,
            RiskScore = 5,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System"
        };

        // Assert
        customer.FirstName.Should().Be("John");
        customer.LastName.Should().Be("Doe");
        customer.Email.Should().Be("john.doe@example.com");
        customer.PhoneNumber.Should().Be("+254712345678");
        customer.DateOfBirth.Should().Be(new DateTime(1990, 1, 1));
        customer.Address.Should().Be("123 Main Street");
        customer.City.Should().Be("Nairobi");
        customer.Country.Should().Be("Kenya");
        customer.Segment.Should().Be(CustomerSegment.Retail);
        customer.KYCStatus.Should().Be(KYCStatus.Verified);
        customer.CustomerReference.Should().Be("CUST001");
        customer.CreditScore.Should().Be(750m);
        customer.LifetimeValue.Should().Be(100000m);
        customer.RiskScore.Should().Be(5);
        customer.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Customer_Should_Support_All_Segments()
    {
        // Arrange
        var segments = new[] 
        { 
            CustomerSegment.Retail, 
            CustomerSegment.SME, 
            CustomerSegment.Corporate, 
            CustomerSegment.HighNetWorth 
        };

        // Act & Assert
        foreach (var segment in segments)
        {
            var customer = new Customer { Segment = segment };
            customer.Segment.Should().Be(segment);
        }
    }

    [Fact]
    public void Customer_Should_Support_All_KYC_Statuses()
    {
        // Arrange
        var statuses = new[] 
        { 
            KYCStatus.Pending, 
            KYCStatus.InProgress, 
            KYCStatus.Verified, 
            KYCStatus.Rejected, 
            KYCStatus.Expired 
        };

        // Act & Assert
        foreach (var status in statuses)
        {
            var customer = new Customer { KYCStatus = status };
            customer.KYCStatus.Should().Be(status);
        }
    }

    [Fact]
    public void Customer_Should_Allow_Null_Optional_Fields()
    {
        // Arrange & Act
        var customer = new Customer
        {
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane.smith@example.com",
            PhoneNumber = "+254722222222"
        };

        // Assert
        customer.DateOfBirth.Should().BeNull();
        customer.Address.Should().BeNull();
        customer.City.Should().BeNull();
        customer.Country.Should().BeNull();
        customer.CustomerReference.Should().BeNull();
        customer.CreditScore.Should().BeNull();
        customer.LifetimeValue.Should().BeNull();
        customer.RiskScore.Should().BeNull();
    }

    [Theory]
    [InlineData("john.doe@example.com")]
    [InlineData("jane.smith@test.co.ke")]
    [InlineData("admin@wekeza.com")]
    public void Customer_Should_Accept_Valid_Email_Formats(string email)
    {
        // Arrange & Act
        var customer = new Customer { Email = email };

        // Assert
        customer.Email.Should().Be(email);
    }

    [Theory]
    [InlineData("+254712345678")]
    [InlineData("+254722222222")]
    [InlineData("0712345678")]
    public void Customer_Should_Accept_Various_Phone_Number_Formats(string phoneNumber)
    {
        // Arrange & Act
        var customer = new Customer { PhoneNumber = phoneNumber };

        // Assert
        customer.PhoneNumber.Should().Be(phoneNumber);
    }

    [Fact]
    public void Customer_Should_Support_Collection_Navigation_Properties()
    {
        // Arrange
        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            FirstName = "Test",
            LastName = "User",
            Email = "test@example.com",
            PhoneNumber = "+254700000000"
        };

        var account = new Account
        {
            Id = Guid.NewGuid(),
            CustomerId = customer.Id,
            AccountNumber = "ACC001"
        };

        // Act
        customer.Accounts.Add(account);

        // Assert
        customer.Accounts.Should().HaveCount(1);
        customer.Accounts.First().Should().Be(account);
    }

    [Theory]
    [InlineData(300.0)]
    [InlineData(750.5)]
    [InlineData(850.25)]
    public void Customer_Should_Accept_Various_Credit_Scores(decimal creditScore)
    {
        // Arrange & Act
        var customer = new Customer { CreditScore = creditScore };

        // Assert
        customer.CreditScore.Should().Be(creditScore);
    }

    [Theory]
    [InlineData(1000.00)]
    [InlineData(50000.50)]
    [InlineData(1000000.99)]
    public void Customer_Should_Accept_Various_Lifetime_Values(decimal lifetimeValue)
    {
        // Arrange & Act
        var customer = new Customer { LifetimeValue = lifetimeValue };

        // Assert
        customer.LifetimeValue.Should().Be(lifetimeValue);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    public void Customer_Should_Accept_Various_Risk_Scores(int riskScore)
    {
        // Arrange & Act
        var customer = new Customer { RiskScore = riskScore };

        // Assert
        customer.RiskScore.Should().Be(riskScore);
    }
}
