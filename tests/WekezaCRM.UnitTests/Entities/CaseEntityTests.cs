using FluentAssertions;
using WekezaCRM.Domain.Entities;
using WekezaCRM.Domain.Enums;
using Xunit;

namespace WekezaCRM.UnitTests.Entities;

public class CaseEntityTests
{
    [Fact]
    public void Case_Should_Initialize_With_Default_Values()
    {
        // Arrange & Act
        var caseEntity = new Case();

        // Assert
        caseEntity.CaseNumber.Should().BeEmpty();
        caseEntity.Title.Should().BeEmpty();
        caseEntity.Description.Should().BeEmpty();
        caseEntity.Category.Should().BeEmpty();
        caseEntity.SubCategory.Should().BeEmpty();
        caseEntity.CaseNotes.Should().BeEmpty();
    }

    [Fact]
    public void Case_Should_Set_Properties_Correctly()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var assignedUserId = Guid.NewGuid();
        var now = DateTime.UtcNow;

        // Act
        var caseEntity = new Case
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            CaseNumber = "CASE001",
            Title = "Account Issue",
            Description = "Customer cannot access account",
            Status = CaseStatus.Open,
            Priority = CasePriority.High,
            Category = "Technical",
            SubCategory = "Login",
            AssignedToUserId = assignedUserId,
            ResolvedAt = now,
            ClosedAt = now.AddHours(1),
            Resolution = "Password reset completed",
            SLADurationHours = 24,
            CreatedAt = now,
            CreatedBy = "Agent001"
        };

        // Assert
        caseEntity.CustomerId.Should().Be(customerId);
        caseEntity.CaseNumber.Should().Be("CASE001");
        caseEntity.Title.Should().Be("Account Issue");
        caseEntity.Description.Should().Be("Customer cannot access account");
        caseEntity.Status.Should().Be(CaseStatus.Open);
        caseEntity.Priority.Should().Be(CasePriority.High);
        caseEntity.Category.Should().Be("Technical");
        caseEntity.SubCategory.Should().Be("Login");
        caseEntity.AssignedToUserId.Should().Be(assignedUserId);
        caseEntity.ResolvedAt.Should().Be(now);
        caseEntity.ClosedAt.Should().Be(now.AddHours(1));
        caseEntity.Resolution.Should().Be("Password reset completed");
        caseEntity.SLADurationHours.Should().Be(24);
    }

    [Fact]
    public void Case_Should_Support_All_Status_Values()
    {
        // Arrange
        var statuses = new[]
        {
            CaseStatus.Open,
            CaseStatus.InProgress,
            CaseStatus.PendingCustomer,
            CaseStatus.Resolved,
            CaseStatus.Closed,
            CaseStatus.Escalated
        };

        // Act & Assert
        foreach (var status in statuses)
        {
            var caseEntity = new Case { Status = status };
            caseEntity.Status.Should().Be(status);
        }
    }

    [Fact]
    public void Case_Should_Support_All_Priority_Values()
    {
        // Arrange
        var priorities = new[]
        {
            CasePriority.Low,
            CasePriority.Medium,
            CasePriority.High,
            CasePriority.Critical
        };

        // Act & Assert
        foreach (var priority in priorities)
        {
            var caseEntity = new Case { Priority = priority };
            caseEntity.Priority.Should().Be(priority);
        }
    }

    [Fact]
    public void Case_Should_Allow_Null_Optional_Fields()
    {
        // Arrange & Act
        var caseEntity = new Case
        {
            CustomerId = Guid.NewGuid(),
            CaseNumber = "CASE002",
            Title = "Test Case",
            Description = "Test Description"
        };

        // Assert
        caseEntity.AssignedToUserId.Should().BeNull();
        caseEntity.ResolvedAt.Should().BeNull();
        caseEntity.ClosedAt.Should().BeNull();
        caseEntity.Resolution.Should().BeNull();
        caseEntity.SLADurationHours.Should().BeNull();
    }

    [Theory]
    [InlineData("Technical", "Login")]
    [InlineData("Billing", "Charges")]
    [InlineData("General", "Inquiry")]
    public void Case_Should_Accept_Various_Categories(string category, string subCategory)
    {
        // Arrange & Act
        var caseEntity = new Case
        {
            Category = category,
            SubCategory = subCategory
        };

        // Assert
        caseEntity.Category.Should().Be(category);
        caseEntity.SubCategory.Should().Be(subCategory);
    }

    [Theory]
    [InlineData(2)]
    [InlineData(24)]
    [InlineData(48)]
    [InlineData(72)]
    public void Case_Should_Accept_Various_SLA_Durations(int slaDuration)
    {
        // Arrange & Act
        var caseEntity = new Case { SLADurationHours = slaDuration };

        // Assert
        caseEntity.SLADurationHours.Should().Be(slaDuration);
    }

    [Fact]
    public void Case_Should_Support_Customer_Navigation_Property()
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

        var caseEntity = new Case
        {
            Id = Guid.NewGuid(),
            CustomerId = customer.Id,
            CaseNumber = "CASE003",
            Title = "Test",
            Description = "Test",
            Customer = customer
        };

        // Act & Assert
        caseEntity.Customer.Should().Be(customer);
        caseEntity.CustomerId.Should().Be(customer.Id);
    }

    [Fact]
    public void Case_Should_Support_CaseNotes_Collection()
    {
        // Arrange
        var caseEntity = new Case
        {
            Id = Guid.NewGuid(),
            CustomerId = Guid.NewGuid(),
            CaseNumber = "CASE004"
        };

        var note = new CaseNote
        {
            Id = Guid.NewGuid(),
            CaseId = caseEntity.Id,
            Note = "Customer contacted via phone"
        };

        // Act
        caseEntity.CaseNotes.Add(note);

        // Assert
        caseEntity.CaseNotes.Should().HaveCount(1);
        caseEntity.CaseNotes.First().Should().Be(note);
    }

    [Fact]
    public void Case_Should_Track_Resolution_Timestamps()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var caseEntity = new Case
        {
            CustomerId = Guid.NewGuid(),
            CaseNumber = "CASE005",
            Title = "Test",
            Description = "Test",
            Status = CaseStatus.Resolved,
            ResolvedAt = now,
            ClosedAt = now.AddDays(1)
        };

        // Assert
        caseEntity.ResolvedAt.Should().Be(now);
        caseEntity.ClosedAt.Should().Be(now.AddDays(1));
        caseEntity.ClosedAt.Should().BeAfter(caseEntity.ResolvedAt.Value);
    }
}
