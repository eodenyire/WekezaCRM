using FluentAssertions;
using WekezaCRM.Domain.Entities;
using WekezaCRM.Domain.Enums;
using Xunit;

namespace WekezaCRM.UnitTests.Entities;

public class InteractionEntityTests
{
    [Fact]
    public void Interaction_Should_Initialize_With_Default_Values()
    {
        // Arrange & Act
        var interaction = new Interaction();

        // Assert
        interaction.Subject.Should().BeEmpty();
        interaction.Description.Should().BeEmpty();
        interaction.InteractionDate.Should().Be(default(DateTime));
    }

    [Fact]
    public void Interaction_Should_Set_Properties_Correctly()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var interactionDate = DateTime.UtcNow;

        // Act
        var interaction = new Interaction
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            Channel = InteractionChannel.Email,
            Subject = "Account Inquiry",
            Description = "Customer inquired about account balance",
            InteractionDate = interactionDate,
            DurationMinutes = 15,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "Agent001"
        };

        // Assert
        interaction.CustomerId.Should().Be(customerId);
        interaction.Channel.Should().Be(InteractionChannel.Email);
        interaction.Subject.Should().Be("Account Inquiry");
        interaction.Description.Should().Be("Customer inquired about account balance");
        interaction.InteractionDate.Should().Be(interactionDate);
        interaction.DurationMinutes.Should().Be(15);
    }

    [Fact]
    public void Interaction_Should_Support_All_Channels()
    {
        // Arrange
        var channels = new[]
        {
            InteractionChannel.Branch,
            InteractionChannel.CallCenter,
            InteractionChannel.Email,
            InteractionChannel.SMS,
            InteractionChannel.WhatsApp,
            InteractionChannel.MobileApp,
            InteractionChannel.Web,
            InteractionChannel.ATM
        };

        // Act & Assert
        foreach (var channel in channels)
        {
            var interaction = new Interaction { Channel = channel };
            interaction.Channel.Should().Be(channel);
        }
    }

    [Theory]
    [InlineData(5)]
    [InlineData(15)]
    [InlineData(30)]
    [InlineData(60)]
    public void Interaction_Should_Accept_Various_Duration_Values(int duration)
    {
        // Arrange & Act
        var interaction = new Interaction { DurationMinutes = duration };

        // Assert
        interaction.DurationMinutes.Should().Be(duration);
    }

    [Fact]
    public void Interaction_Should_Support_Customer_Navigation_Property()
    {
        // Arrange
        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            FirstName = "Jane",
            LastName = "Doe",
            Email = "jane@example.com",
            PhoneNumber = "+254700000000"
        };

        var interaction = new Interaction
        {
            Id = Guid.NewGuid(),
            CustomerId = customer.Id,
            Subject = "Test",
            Description = "Test interaction",
            Customer = customer
        };

        // Act & Assert
        interaction.Customer.Should().Be(customer);
        interaction.CustomerId.Should().Be(customer.Id);
    }

    [Theory]
    [InlineData(InteractionChannel.Branch, "Branch Visit", "Customer visited for loan application")]
    [InlineData(InteractionChannel.CallCenter, "Phone Call", "Customer called about card issue")]
    [InlineData(InteractionChannel.Email, "Email Inquiry", "Customer emailed about statement")]
    [InlineData(InteractionChannel.WhatsApp, "WhatsApp Chat", "Customer messaged on WhatsApp")]
    public void Interaction_Should_Handle_Different_Channel_Scenarios(
        InteractionChannel channel,
        string subject,
        string description)
    {
        // Arrange & Act
        var interaction = new Interaction
        {
            Channel = channel,
            Subject = subject,
            Description = description
        };

        // Assert
        interaction.Channel.Should().Be(channel);
        interaction.Subject.Should().Be(subject);
        interaction.Description.Should().Be(description);
    }

    [Fact]
    public void Interaction_Should_Allow_Null_Optional_Duration()
    {
        // Arrange & Act
        var interaction = new Interaction
        {
            CustomerId = Guid.NewGuid(),
            Subject = "Quick Note",
            Description = "Brief interaction"
        };

        // Assert
        interaction.DurationMinutes.Should().BeNull();
    }
}
