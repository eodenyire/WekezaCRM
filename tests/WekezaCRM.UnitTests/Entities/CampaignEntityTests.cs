using FluentAssertions;
using WekezaCRM.Domain.Entities;
using Xunit;

namespace WekezaCRM.UnitTests.Entities;

public class CampaignEntityTests
{
    [Fact]
    public void Campaign_Should_Initialize_With_Default_Values()
    {
        // Arrange & Act
        var campaign = new Campaign();

        // Assert
        campaign.Name.Should().BeEmpty();
        campaign.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Campaign_Should_Set_Properties_Correctly()
    {
        // Arrange & Act
        var campaign = new Campaign
        {
            Name = "Test Campaign",
            Description = "Test"
        };

        // Assert
        campaign.Name.Should().Be("Test Campaign");
    }

    [Fact]
    public void Campaign_Can_Have_Multiple_Customers()
    {
        // Arrange & Act
        var campaign = new Campaign { Id = Guid.NewGuid() };

        // Assert
        campaign.Customers.Should().BeEmpty();
    }

    [Fact]
    public void Campaign_StartDate_Can_Be_Set()
    {
        // Arrange
        var date = DateTime.UtcNow;

        // Act
        var campaign = new Campaign { StartDate = date };

        // Assert
        campaign.StartDate.Should().Be(date);
    }

    [Fact]
    public void Campaign_EndDate_Can_Be_Set()
    {
        // Arrange
        var date = DateTime.UtcNow.AddDays(30);

        // Act
        var campaign = new Campaign { EndDate = date };

        // Assert
        campaign.EndDate.Should().Be(date);
    }

    [Fact]
    public void Campaign_TargetSegment_Can_Be_Set()
    {
        // Arrange & Act
        var campaign = new Campaign { TargetSegment = "Retail" };

        // Assert
        campaign.TargetSegment.Should().Be("Retail");
    }

    [Theory]
    [InlineData("Retail")]
    [InlineData("SME")]
    [InlineData("Corporate")]
    public void Campaign_Should_Support_Various_Segments(string segment)
    {
        // Arrange & Act
        var campaign = new Campaign { TargetSegment = segment };

        // Assert
        campaign.TargetSegment.Should().Be(segment);
    }

    [Fact]
    public void Campaign_TargetCustomers_Can_Be_Set()
    {
        // Arrange & Act
        var campaign = new Campaign { TargetCustomers = 1000 };

        // Assert
        campaign.TargetCustomers.Should().Be(1000);
    }

    [Fact]
    public void Campaign_ReachedCustomers_Can_Be_Tracked()
    {
        // Arrange & Act
        var campaign = new Campaign { ReachedCustomers = 750 };

        // Assert
        campaign.ReachedCustomers.Should().Be(750);
    }

    [Fact]
    public void Campaign_IsActive_Can_Be_Set()
    {
        // Arrange & Act
        var campaign = new Campaign { IsActive = false };

        // Assert
        campaign.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Campaign_Description_Can_Be_Long()
    {
        // Arrange
        var longDesc = new string('A', 500);

        // Act
        var campaign = new Campaign { Description = longDesc };

        // Assert
        campaign.Description.Should().HaveLength(500);
    }

    [Fact]
    public void Campaign_Customers_Collection_Should_Be_Empty_Initially()
    {
        // Arrange & Act
        var campaign = new Campaign();

        // Assert
        campaign.Customers.Should().BeEmpty();
    }

    [Fact]
    public void Campaign_Can_Add_Customers()
    {
        // Arrange
        var campaign = new Campaign { Id = Guid.NewGuid() };
        var customer = new Customer { Id = Guid.NewGuid() };

        // Act
        campaign.Customers.Add(customer);

        // Assert
        campaign.Customers.Should().HaveCount(1);
    }

    [Fact]
    public void Campaign_StartDate_Can_Be_In_Future()
    {
        // Arrange
        var futureDate = DateTime.UtcNow.AddMonths(1);

        // Act
        var campaign = new Campaign { StartDate = futureDate };

        // Assert
        campaign.StartDate.Should().BeAfter(DateTime.UtcNow);
    }

    [Fact]
    public void Campaign_EndDate_Should_Be_After_StartDate()
    {
        // Arrange
        var start = DateTime.UtcNow;
        var end = start.AddDays(30);

        // Act
        var campaign = new Campaign
        {
            StartDate = start,
            EndDate = end
        };

        // Assert
        campaign.EndDate.Should().BeAfter(campaign.StartDate);
    }
}
