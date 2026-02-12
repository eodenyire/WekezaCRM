using FluentAssertions;
using WekezaCRM.Domain.Entities;
using WekezaCRM.Domain.Enums;
using Xunit;

namespace WekezaCRM.UnitTests.EdgeCases;

public class CustomerEdgeCaseTests
{
    [Fact]
    public void Customer_With_Empty_Guid_Should_Be_Created()
    {
        var customer = new Customer { Id = Guid.Empty };
        customer.Id.Should().Be(Guid.Empty);
    }

    [Fact]
    public void Customer_With_Very_Long_Name_Should_Be_Accepted()
    {
        var longName = new string('A', 500);
        var customer = new Customer { FirstName = longName, LastName = longName };
        customer.FirstName.Should().HaveLength(500);
    }

    [Fact]
    public void Customer_With_Special_Characters_Should_Be_Accepted()
    {
        var customer = new Customer { FirstName = "O'Brien", LastName = "Müller-Schmidt" };
        customer.FirstName.Should().Be("O'Brien");
    }

    [Fact]
    public void Customer_With_Unicode_Characters_Should_Be_Accepted()
    {
        var customer = new Customer { FirstName = "李", LastName = "明", Address = "东京市" };
        customer.FirstName.Should().Be("李");
    }

    [Fact]
    public void Customer_With_Maximum_CreditScore_Should_Be_Valid()
    {
        var customer = new Customer { CreditScore = decimal.MaxValue };
        customer.CreditScore.Should().Be(decimal.MaxValue);
    }

    [Fact]
    public void Customer_With_Negative_LifetimeValue_Should_Be_Valid()
    {
        var customer = new Customer { LifetimeValue = -10000m };
        customer.LifetimeValue.Should().Be(-10000m);
    }

    [Fact]
    public void Customer_With_Future_DateOfBirth_Should_Be_Accepted()
    {
        var futureDate = DateTime.UtcNow.AddYears(10);
        var customer = new Customer { DateOfBirth = futureDate };
        customer.DateOfBirth.Should().Be(futureDate);
    }

    [Fact]
    public void Customer_With_Very_Old_DateOfBirth_Should_Be_Accepted()
    {
        var veryOldDate = new DateTime(1900, 1, 1);
        var customer = new Customer { DateOfBirth = veryOldDate };
        customer.DateOfBirth.Should().Be(veryOldDate);
    }

    [Fact]
    public void Customer_With_Empty_PhoneNumber_Should_Be_Accepted()
    {
        var customer = new Customer { FirstName = "John", LastName = "Doe", Email = "john@example.com", PhoneNumber = "" };
        customer.PhoneNumber.Should().BeEmpty();
    }

    [Fact]
    public void Customer_With_Very_Long_Email_Should_Be_Accepted()
    {
        var longEmail = new string('a', 50) + "@" + new string('b', 50) + ".com";
        var customer = new Customer { Email = longEmail };
        customer.Email.Should().HaveLength(longEmail.Length);
    }

    [Theory]
    [InlineData(int.MinValue)]
    [InlineData(int.MaxValue)]
    [InlineData(0)]
    public void Customer_RiskScore_Should_Handle_Extreme_Values(int riskScore)
    {
        var customer = new Customer { RiskScore = riskScore };
        customer.RiskScore.Should().Be(riskScore);
    }

    [Fact]
    public void Customer_With_Same_FirstName_And_LastName_Should_Be_Valid()
    {
        var customer = new Customer { FirstName = "Smith", LastName = "Smith" };
        customer.FirstName.Should().Be("Smith");
    }

    [Fact]
    public void Customer_Can_Have_Same_City_And_Country()
    {
        var customer = new Customer { City = "Singapore", Country = "Singapore" };
        customer.City.Should().Be("Singapore");
    }

    [Fact]
    public void Customer_CreatedAt_And_UpdatedAt_Can_Be_Same()
    {
        var now = DateTime.UtcNow;
        var customer = new Customer { CreatedAt = now, UpdatedAt = now };
        customer.CreatedAt.Should().Be(now);
    }

    [Fact]
    public void Customer_UpdatedAt_Can_Be_Before_CreatedAt()
    {
        var now = DateTime.UtcNow;
        var customer = new Customer { CreatedAt = now, UpdatedAt = now.AddDays(-1) };
        customer.UpdatedAt.Should().BeBefore(customer.CreatedAt);
    }
}
