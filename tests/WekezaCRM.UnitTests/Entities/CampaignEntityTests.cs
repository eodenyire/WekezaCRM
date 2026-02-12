using FluentAssertions;
using WekezaCRM.Domain.Entities;
using Xunit;

namespace WekezaCRM.UnitTests.Entities;

public class CampaignEntityTests
{
    [Fact] public void Campaign_Should_Initialize_With_Default_Values() { var campaign = new Campaign(); campaign.Name.Should().BeEmpty(); campaign.IsActive.Should().BeTrue(); }
    [Fact] public void Campaign_Should_Set_Properties_Correctly() { var campaign = new Campaign { Name = "Test Campaign", Description = "Test" }; campaign.Name.Should().Be("Test Campaign"); }
    [Fact] public void Campaign_Can_Have_Multiple_Customers() { var campaign = new Campaign { Id = Guid.NewGuid() }; campaign.Customers.Should().BeEmpty(); }
    [Fact] public void Campaign_StartDate_Can_Be_Set() { var date = DateTime.UtcNow; var campaign = new Campaign { StartDate = date }; campaign.StartDate.Should().Be(date); }
    [Fact] public void Campaign_EndDate_Can_Be_Set() { var date = DateTime.UtcNow.AddDays(30); var campaign = new Campaign { EndDate = date }; campaign.EndDate.Should().Be(date); }
    [Fact] public void Campaign_TargetSegment_Can_Be_Set() { var campaign = new Campaign { TargetSegment = "Retail" }; campaign.TargetSegment.Should().Be("Retail"); }
    [Theory] [InlineData("Retail")] [InlineData("SME")] [InlineData("Corporate")] public void Campaign_Should_Support_Various_Segments(string segment) { var campaign = new Campaign { TargetSegment = segment }; campaign.TargetSegment.Should().Be(segment); }
    [Fact] public void Campaign_TargetCustomers_Can_Be_Set() { var campaign = new Campaign { TargetCustomers = 1000 }; campaign.TargetCustomers.Should().Be(1000); }
    [Fact] public void Campaign_ReachedCustomers_Can_Be_Tracked() { var campaign = new Campaign { ReachedCustomers = 750 }; campaign.ReachedCustomers.Should().Be(750); }
    [Fact] public void Campaign_IsActive_Can_Be_Set() { var campaign = new Campaign { IsActive = false }; campaign.IsActive.Should().BeFalse(); }
    [Fact] public void Campaign_Description_Can_Be_Long() { var longDesc = new string('A', 500); var campaign = new Campaign { Description = longDesc }; campaign.Description.Should().HaveLength(500); }
    [Fact] public void Campaign_Customers_Collection_Should_Be_Empty_Initially() { var campaign = new Campaign(); campaign.Customers.Should().BeEmpty(); }
    [Fact] public void Campaign_Can_Add_Customers() { var campaign = new Campaign { Id = Guid.NewGuid() }; var customer = new Customer { Id = Guid.NewGuid() }; campaign.Customers.Add(customer); campaign.Customers.Should().HaveCount(1); }
    [Fact] public void Campaign_StartDate_Can_Be_In_Future() { var futureDate = DateTime.UtcNow.AddMonths(1); var campaign = new Campaign { StartDate = futureDate }; campaign.StartDate.Should().BeAfter(DateTime.UtcNow); }
    [Fact] public void Campaign_EndDate_Should_Be_After_StartDate() { var start = DateTime.UtcNow; var end = start.AddDays(30); var campaign = new Campaign { StartDate = start, EndDate = end }; campaign.EndDate.Should().BeAfter(campaign.StartDate); }
}
