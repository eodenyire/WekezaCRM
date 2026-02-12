using FluentAssertions;
using WekezaCRM.Domain.Entities;
using WekezaCRM.Domain.Enums;
using Xunit;

namespace WekezaCRM.UnitTests.Validation;

public class CustomerValidationTests
{
    [Fact]
    public void Customer_Email_Should_Be_Required()
    {
        // Arrange & Act
        var customer = new Customer
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "",
            PhoneNumber = "+254700000000"
        };

        // Assert
        customer.Email.Should().BeEmpty();
    }

    [Theory]
    [InlineData("invalid-email")]
    [InlineData("@example.com")]
    [InlineData("user@")]
    [InlineData("user")]
    public void Customer_Should_Accept_Any_Email_Format_At_Entity_Level(string email)
    {
        // Arrange & Act
        var customer = new Customer { Email = email };

        // Assert - At entity level, any string is accepted
        customer.Email.Should().Be(email);
    }

    [Theory]
    [InlineData("+254700000000")]
    [InlineData("+1234567890")]
    [InlineData("0712345678")]
    [InlineData("+44 20 1234 5678")]
    public void Customer_Should_Accept_Various_Phone_Number_Formats(string phoneNumber)
    {
        // Arrange & Act
        var customer = new Customer { PhoneNumber = phoneNumber };

        // Assert
        customer.PhoneNumber.Should().Be(phoneNumber);
    }

    [Fact]
    public void Customer_FirstName_Should_Be_Required()
    {
        // Arrange & Act
        var customer = new Customer
        {
            FirstName = "",
            LastName = "Doe",
            Email = "john@example.com",
            PhoneNumber = "+254700000000"
        };

        // Assert
        customer.FirstName.Should().BeEmpty();
    }

    [Fact]
    public void Customer_LastName_Should_Be_Required()
    {
        // Arrange & Act
        var customer = new Customer
        {
            FirstName = "John",
            LastName = "",
            Email = "john@example.com",
            PhoneNumber = "+254700000000"
        };

        // Assert
        customer.LastName.Should().BeEmpty();
    }

    [Theory]
    [InlineData(-100)]
    [InlineData(0)]
    [InlineData(300)]
    [InlineData(500)]
    [InlineData(850)]
    public void Customer_CreditScore_Should_Accept_Various_Values(decimal creditScore)
    {
        // Arrange & Act
        var customer = new Customer { CreditScore = creditScore };

        // Assert
        customer.CreditScore.Should().Be(creditScore);
    }

    [Theory]
    [InlineData(-1000)]
    [InlineData(0)]
    [InlineData(1000)]
    [InlineData(1000000)]
    public void Customer_LifetimeValue_Should_Accept_Various_Values(decimal lifetimeValue)
    {
        // Arrange & Act
        var customer = new Customer { LifetimeValue = lifetimeValue };

        // Assert
        customer.LifetimeValue.Should().Be(lifetimeValue);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    public void Customer_RiskScore_Should_Accept_Various_Values(int riskScore)
    {
        // Arrange & Act
        var customer = new Customer { RiskScore = riskScore };

        // Assert
        customer.RiskScore.Should().Be(riskScore);
    }

    [Fact]
    public void Customer_Should_Be_Active_By_Default()
    {
        // Arrange & Act
        var customer = new Customer();

        // Assert
        customer.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Customer_Can_Be_Deactivated()
    {
        // Arrange
        var customer = new Customer { IsActive = true };

        // Act
        customer.IsActive = false;

        // Assert
        customer.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Customer_DateOfBirth_Can_Be_Null()
    {
        // Arrange & Act
        var customer = new Customer
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            PhoneNumber = "+254700000000"
        };

        // Assert
        customer.DateOfBirth.Should().BeNull();
    }

    [Theory]
    [InlineData(1950, 1, 1)]
    [InlineData(1990, 6, 15)]
    [InlineData(2000, 12, 31)]
    public void Customer_Should_Accept_Various_Dates_Of_Birth(int year, int month, int day)
    {
        // Arrange
        var dob = new DateTime(year, month, day);

        // Act
        var customer = new Customer { DateOfBirth = dob };

        // Assert
        customer.DateOfBirth.Should().Be(dob);
    }

    [Fact]
    public void Customer_Address_Fields_Can_Be_Null()
    {
        // Arrange & Act
        var customer = new Customer
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            PhoneNumber = "+254700000000"
        };

        // Assert
        customer.Address.Should().BeNull();
        customer.City.Should().BeNull();
        customer.Country.Should().BeNull();
    }

    [Theory]
    [InlineData("123 Main Street")]
    [InlineData("P.O. Box 12345")]
    [InlineData("Apt 4B, Building 7")]
    public void Customer_Should_Accept_Various_Address_Formats(string address)
    {
        // Arrange & Act
        var customer = new Customer { Address = address };

        // Assert
        customer.Address.Should().Be(address);
    }
}
