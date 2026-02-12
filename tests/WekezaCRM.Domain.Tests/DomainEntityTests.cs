using Xunit;
using FluentAssertions;
using WekezaCRM.Domain.Entities;
using WekezaCRM.Domain.Enums;

namespace WekezaCRM.Domain.Tests;

public class CustomerTests
{
    [Fact]
    public void Customer_Should_Be_Created_With_Valid_Data()
    {
        // Arrange & Act
        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            PhoneNumber = "+254712345678",
            Segment = CustomerSegment.Retail,
            KYCStatus = KYCStatus.Pending,
            IsActive = true
        };

        // Assert
        customer.FirstName.Should().Be("John");
        customer.LastName.Should().Be("Doe");
        customer.Email.Should().Be("john.doe@example.com");
        customer.Segment.Should().Be(CustomerSegment.Retail);
        customer.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Customer_Should_Have_Collections_Initialized()
    {
        // Arrange & Act
        var customer = new Customer();

        // Assert
        customer.Accounts.Should().NotBeNull();
        customer.Interactions.Should().NotBeNull();
        customer.Cases.Should().NotBeNull();
        customer.Campaigns.Should().NotBeNull();
    }
}

public class WhatsAppMessageTests
{
    [Fact]
    public void WhatsAppMessage_Should_Be_Created_With_Valid_Data()
    {
        // Arrange & Act
        var message = new WhatsAppMessage
        {
            Id = Guid.NewGuid(),
            CustomerId = Guid.NewGuid(),
            PhoneNumber = "+254712345678",
            MessageType = WhatsAppMessageType.Text,
            Status = WhatsAppMessageStatus.Sent,
            Content = "Hello World",
            IsInbound = false
        };

        // Assert
        message.PhoneNumber.Should().Be("+254712345678");
        message.MessageType.Should().Be(WhatsAppMessageType.Text);
        message.Status.Should().Be(WhatsAppMessageStatus.Sent);
        message.IsInbound.Should().BeFalse();
    }
}

public class USSDSessionTests
{
    [Fact]
    public void USSDSession_Should_Be_Created_With_Valid_Data()
    {
        // Arrange & Act
        var session = new USSDSession
        {
            Id = Guid.NewGuid(),
            SessionId = "SESSION123",
            PhoneNumber = "+254712345678",
            Status = USSDSessionStatus.Active,
            CurrentMenu = "main",
            StartedAt = DateTime.UtcNow
        };

        // Assert
        session.SessionId.Should().Be("SESSION123");
        session.Status.Should().Be(USSDSessionStatus.Active);
        session.CurrentMenu.Should().Be("main");
    }
}

public class ReportTemplateTests
{
    [Fact]
    public void ReportTemplate_Should_Be_Created_With_Valid_Data()
    {
        // Arrange & Act
        var template = new ReportTemplate
        {
            Id = Guid.NewGuid(),
            Name = "Customer Report",
            Description = "Monthly customer analysis",
            ReportType = "CustomerAnalysis",
            DefaultFormat = ReportFormat.PDF,
            IsActive = true
        };

        // Assert
        template.Name.Should().Be("Customer Report");
        template.DefaultFormat.Should().Be(ReportFormat.PDF);
        template.IsActive.Should().BeTrue();
        template.ReportSchedules.Should().NotBeNull();
        template.GeneratedReports.Should().NotBeNull();
    }
}
