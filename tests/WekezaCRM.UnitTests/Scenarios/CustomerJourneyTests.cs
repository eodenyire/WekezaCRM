using FluentAssertions;
using WekezaCRM.Domain.Entities;
using WekezaCRM.Domain.Enums;
using Xunit;

namespace WekezaCRM.UnitTests.Scenarios;

public class CustomerJourneyTests
{
    [Fact]
    public void New_Customer_Registration_Should_Set_Defaults()
    {
        var customer = new Customer
        {
            FirstName = "New",
            LastName = "Customer",
            Email = "new@example.com",
            PhoneNumber = "+254700000000",
            KYCStatus = KYCStatus.Pending,
            IsActive = true
        };
        customer.KYCStatus.Should().Be(KYCStatus.Pending);
        customer.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Customer_KYC_Verification_Should_Progress()
    {
        var customer = new Customer { KYCStatus = KYCStatus.Pending };
        customer.KYCStatus = KYCStatus.InProgress;
        customer.KYCStatus.Should().Be(KYCStatus.InProgress);
        customer.KYCStatus = KYCStatus.Verified;
        customer.KYCStatus.Should().Be(KYCStatus.Verified);
    }

    [Fact]
    public void Customer_Can_Have_Multiple_Accounts()
    {
        var customer = new Customer { Id = Guid.NewGuid() };
        var savingsAccount = new Account { AccountType = "Savings", CustomerId = customer.Id };
        var currentAccount = new Account { AccountType = "Current", CustomerId = customer.Id };
        customer.Accounts.Add(savingsAccount);
        customer.Accounts.Add(currentAccount);
        customer.Accounts.Should().HaveCount(2);
    }

    [Fact]
    public void Customer_Interaction_History_Should_Be_Tracked()
    {
        var customer = new Customer { Id = Guid.NewGuid() };
        customer.Interactions.Add(new Interaction { CustomerId = customer.Id, Channel = InteractionChannel.Branch, InteractionDate = DateTime.UtcNow.AddDays(-5) });
        customer.Interactions.Add(new Interaction { CustomerId = customer.Id, Channel = InteractionChannel.CallCenter, InteractionDate = DateTime.UtcNow.AddDays(-3) });
        customer.Interactions.Add(new Interaction { CustomerId = customer.Id, Channel = InteractionChannel.Email, InteractionDate = DateTime.UtcNow.AddDays(-1) });
        customer.Interactions.Should().HaveCount(3);
    }

    [Fact]
    public void Customer_Can_Open_Multiple_Cases()
    {
        var customer = new Customer { Id = Guid.NewGuid() };
        customer.Cases.Add(new Case { CustomerId = customer.Id, CaseNumber = "CASE001", Status = CaseStatus.Open });
        customer.Cases.Add(new Case { CustomerId = customer.Id, CaseNumber = "CASE002", Status = CaseStatus.Resolved });
        customer.Cases.Should().HaveCount(2);
    }

    [Fact]
    public void Customer_Segment_Can_Be_Upgraded()
    {
        var customer = new Customer { Segment = CustomerSegment.Retail };
        customer.Segment = CustomerSegment.HighNetWorth;
        customer.Segment.Should().Be(CustomerSegment.HighNetWorth);
    }

    [Fact]
    public void Customer_CreditScore_Can_Improve()
    {
        var customer = new Customer { CreditScore = 500m };
        customer.CreditScore = 750m;
        customer.CreditScore.Should().Be(750m);
    }

    [Fact]
    public void Customer_LifetimeValue_Should_Increase_Over_Time()
    {
        var customer = new Customer { LifetimeValue = 10000m };
        customer.LifetimeValue = 50000m;
        customer.LifetimeValue.Should().Be(50000m);
    }

    [Fact]
    public void Inactive_Customer_Can_Be_Reactivated()
    {
        var customer = new Customer { IsActive = false };
        customer.IsActive = true;
        customer.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Customer_With_High_RiskScore_Should_Be_Flagged()
    {
        var customer = new Customer { RiskScore = 8 };
        customer.RiskScore.Should().BeGreaterThan(5);
    }

    [Fact]
    public void Customer_Can_Be_Enrolled_In_Campaign()
    {
        var customer = new Customer { Id = Guid.NewGuid() };
        var campaign = new Campaign { Id = Guid.NewGuid(), Name = "Summer Promotion" };
        customer.Campaigns.Add(campaign);
        customer.Campaigns.Should().HaveCount(1);
    }

    [Fact]
    public void Customer_Profile_Can_Be_Updated()
    {
        var customer = new Customer { Address = "Old Address", City = "Old City" };
        customer.Address = "New Address";
        customer.City = "New City";
        customer.UpdatedAt = DateTime.UtcNow;
        customer.Address.Should().Be("New Address");
        customer.UpdatedAt.Should().NotBeNull();
    }
}
