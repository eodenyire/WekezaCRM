using Xunit;
using FluentAssertions;
using WekezaCRM.Domain.Entities;
using WekezaCRM.Domain.Enums;

namespace WekezaCRM.Domain.Tests;

public class CaseEntityTests
{
    [Fact]
    public void Case_Should_Be_Created_With_Valid_Data()
    {
        // Arrange & Act
        var caseEntity = new Case
        {
            Id = Guid.NewGuid(),
            CustomerId = Guid.NewGuid(),
            CaseNumber = "CASE-001",
            Title = "Account Issue",
            Description = "Customer cannot access account",
            Status = CaseStatus.Open,
            Priority = CasePriority.High,
            Category = "Technical",
            SubCategory = "Login"
        };

        // Assert
        caseEntity.CaseNumber.Should().Be("CASE-001");
        caseEntity.Title.Should().Be("Account Issue");
        caseEntity.Status.Should().Be(CaseStatus.Open);
        caseEntity.Priority.Should().Be(CasePriority.High);
    }

    [Fact]
    public void Case_Should_Have_CaseNotes_Collection_Initialized()
    {
        // Arrange & Act
        var caseEntity = new Case();

        // Assert
        caseEntity.CaseNotes.Should().NotBeNull();
        caseEntity.CaseNotes.Should().BeEmpty();
    }

    [Theory]
    [InlineData(CaseStatus.Open)]
    [InlineData(CaseStatus.InProgress)]
    [InlineData(CaseStatus.Resolved)]
    [InlineData(CaseStatus.Closed)]
    public void Case_Should_Support_All_Status_Values(CaseStatus status)
    {
        // Arrange & Act
        var caseEntity = new Case { Status = status };

        // Assert
        caseEntity.Status.Should().Be(status);
    }

    [Theory]
    [InlineData(CasePriority.Low)]
    [InlineData(CasePriority.Medium)]
    [InlineData(CasePriority.High)]
    [InlineData(CasePriority.Critical)]
    public void Case_Should_Support_All_Priority_Values(CasePriority priority)
    {
        // Arrange & Act
        var caseEntity = new Case { Priority = priority };

        // Assert
        caseEntity.Priority.Should().Be(priority);
    }
}

public class InteractionEntityTests
{
    [Fact]
    public void Interaction_Should_Be_Created_With_Valid_Data()
    {
        // Arrange & Act
        var interaction = new Interaction
        {
            Id = Guid.NewGuid(),
            CustomerId = Guid.NewGuid(),
            Channel = InteractionChannel.Email,
            Subject = "Account Inquiry",
            Description = "Customer asking about balance",
            InteractionDate = DateTime.UtcNow,
            DurationMinutes = 15
        };

        // Assert
        interaction.Channel.Should().Be(InteractionChannel.Email);
        interaction.Subject.Should().Be("Account Inquiry");
        interaction.DurationMinutes.Should().Be(15);
    }

    [Theory]
    [InlineData(InteractionChannel.Branch)]
    [InlineData(InteractionChannel.CallCenter)]
    [InlineData(InteractionChannel.Email)]
    [InlineData(InteractionChannel.WhatsApp)]
    [InlineData(InteractionChannel.MobileApp)]
    public void Interaction_Should_Support_All_Channel_Values(InteractionChannel channel)
    {
        // Arrange & Act
        var interaction = new Interaction { Channel = channel };

        // Assert
        interaction.Channel.Should().Be(channel);
    }
}

public class AccountEntityTests
{
    [Fact]
    public void Account_Should_Be_Created_With_Valid_Data()
    {
        // Arrange & Act
        var account = new Account
        {
            Id = Guid.NewGuid(),
            CustomerId = Guid.NewGuid(),
            AccountNumber = "ACC-123456",
            AccountType = "Savings",
            Balance = 1000.50m,
            Currency = "KES",
            IsActive = true
        };

        // Assert
        account.AccountNumber.Should().Be("ACC-123456");
        account.Balance.Should().Be(1000.50m);
        account.Currency.Should().Be("KES");
        account.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Account_Should_Have_Transactions_Collection_Initialized()
    {
        // Arrange & Act
        var account = new Account();

        // Assert
        account.Transactions.Should().NotBeNull();
        account.Transactions.Should().BeEmpty();
    }

    [Fact]
    public void Account_Balance_Should_Be_Decimal_Type()
    {
        // Arrange & Act
        var account = new Account { Balance = 12345.67m };

        // Assert
        account.Balance.Should().Be(12345.67m);
    }
}

public class NextBestActionEntityTests
{
    [Fact]
    public void NextBestAction_Should_Be_Created_With_Valid_Data()
    {
        // Arrange & Act
        var action = new NextBestAction
        {
            Id = Guid.NewGuid(),
            CustomerId = Guid.NewGuid(),
            ActionType = ActionType.ProductRecommendation,
            Title = "Upgrade to Premium",
            Description = "Customer eligible for premium account",
            ConfidenceScore = 0.85m,
            RecommendedDate = DateTime.UtcNow,
            IsCompleted = false
        };

        // Assert
        action.ActionType.Should().Be(ActionType.ProductRecommendation);
        action.ConfidenceScore.Should().Be(0.85m);
        action.IsCompleted.Should().BeFalse();
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(0.5)]
    [InlineData(1.0)]
    public void NextBestAction_ConfidenceScore_Should_Be_Between_Zero_And_One(decimal score)
    {
        // Arrange & Act
        var action = new NextBestAction { ConfidenceScore = score };

        // Assert
        action.ConfidenceScore.Should().BeInRange(0m, 1m);
    }
}

public class SentimentAnalysisEntityTests
{
    [Fact]
    public void SentimentAnalysis_Should_Be_Created_With_Valid_Data()
    {
        // Arrange & Act
        var sentiment = new SentimentAnalysis
        {
            Id = Guid.NewGuid(),
            CustomerId = Guid.NewGuid(),
            SentimentType = SentimentType.Positive,
            SentimentScore = 0.8m,
            TextAnalyzed = "Great service!",
            AnalyzedDate = DateTime.UtcNow
        };

        // Assert
        sentiment.SentimentType.Should().Be(SentimentType.Positive);
        sentiment.SentimentScore.Should().Be(0.8m);
        sentiment.TextAnalyzed.Should().Be("Great service!");
    }

    [Theory]
    [InlineData(SentimentType.Positive)]
    [InlineData(SentimentType.Neutral)]
    [InlineData(SentimentType.Negative)]
    [InlineData(SentimentType.VeryNegative)]
    public void SentimentAnalysis_Should_Support_All_Sentiment_Types(SentimentType sentimentType)
    {
        // Arrange & Act
        var sentiment = new SentimentAnalysis { SentimentType = sentimentType };

        // Assert
        sentiment.SentimentType.Should().Be(sentimentType);
    }
}

public class WorkflowDefinitionEntityTests
{
    [Fact]
    public void WorkflowDefinition_Should_Be_Created_With_Valid_Data()
    {
        // Arrange & Act
        var workflow = new WorkflowDefinition
        {
            Id = Guid.NewGuid(),
            Name = "Case Escalation",
            Description = "Auto-escalate high priority cases",
            TriggerType = "CaseCreated",
            TriggerConditions = "{\"priority\": \"high\"}",
            Actions = "[{\"type\": \"notify\"}]",
            IsActive = true
        };

        // Assert
        workflow.Name.Should().Be("Case Escalation");
        workflow.IsActive.Should().BeTrue();
        workflow.WorkflowInstances.Should().NotBeNull();
    }
}

public class NotificationEntityTests
{
    [Fact]
    public void Notification_Should_Be_Created_With_Valid_Data()
    {
        // Arrange & Act
        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Type = NotificationType.Alert,
            Title = "High Priority Case",
            Message = "New high priority case assigned",
            IsRead = false
        };

        // Assert
        notification.Type.Should().Be(NotificationType.Alert);
        notification.IsRead.Should().BeFalse();
    }

    [Fact]
    public void Notification_ReadAt_Should_Be_Null_When_Not_Read()
    {
        // Arrange & Act
        var notification = new Notification { IsRead = false };

        // Assert
        notification.ReadAt.Should().BeNull();
    }
}

public class GeneratedReportEntityTests
{
    [Fact]
    public void GeneratedReport_Should_Be_Created_With_Valid_Data()
    {
        // Arrange & Act
        var report = new GeneratedReport
        {
            Id = Guid.NewGuid(),
            ReportTemplateId = Guid.NewGuid(),
            ReportName = "Monthly Sales Report",
            GeneratedDate = DateTime.UtcNow,
            Format = ReportFormat.PDF,
            FilePath = "/reports/monthly_sales.pdf",
            FileSizeBytes = 1024000,
            RecordCount = 150
        };

        // Assert
        report.ReportName.Should().Be("Monthly Sales Report");
        report.Format.Should().Be(ReportFormat.PDF);
        report.FileSizeBytes.Should().BeGreaterThan(0);
        report.RecordCount.Should().Be(150);
    }

    [Theory]
    [InlineData(ReportFormat.PDF)]
    [InlineData(ReportFormat.Excel)]
    [InlineData(ReportFormat.CSV)]
    [InlineData(ReportFormat.JSON)]
    public void GeneratedReport_Should_Support_All_Format_Types(ReportFormat format)
    {
        // Arrange & Act
        var report = new GeneratedReport { Format = format };

        // Assert
        report.Format.Should().Be(format);
    }
}
