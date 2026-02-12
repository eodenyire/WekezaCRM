using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using WekezaCRM.API.Controllers;
using WekezaCRM.Application.DTOs;
using WekezaCRM.Application.Interfaces;
using WekezaCRM.Domain.Entities;
using WekezaCRM.Domain.Enums;
using Xunit;

namespace WekezaCRM.UnitTests.Controllers;

public class InteractionsControllerTests
{
    private readonly Mock<IInteractionRepository> _mockInteractionRepository;
    private readonly Mock<ICustomerRepository> _mockCustomerRepository;
    private readonly Mock<ILogger<InteractionsController>> _mockLogger;
    private readonly InteractionsController _controller;
    private readonly Customer _testCustomer;

    public InteractionsControllerTests()
    {
        _mockInteractionRepository = new Mock<IInteractionRepository>();
        _mockCustomerRepository = new Mock<ICustomerRepository>();
        _mockLogger = new Mock<ILogger<InteractionsController>>();
        _controller = new InteractionsController(_mockInteractionRepository.Object, _mockCustomerRepository.Object, _mockLogger.Object);

        _testCustomer = new Customer
        {
            Id = Guid.NewGuid(),
            FirstName = "Test",
            LastName = "Customer",
            Email = "test@example.com",
            PhoneNumber = "+254700000000",
            Segment = CustomerSegment.Retail,
            KYCStatus = KYCStatus.Verified,
            IsActive = true
        };
    }

    [Fact]
    public async Task GetAllInteractions_Should_Return_Ok_With_Interactions()
    {
        // Arrange
        var interactions = new List<Interaction>
        {
            new() {
                Id = Guid.NewGuid(),
                CustomerId = _testCustomer.Id,
                Channel = InteractionChannel.Email,
                Subject = "Account Query",
                Description = "Customer asked about balance",
                InteractionDate = DateTime.UtcNow,
                DurationMinutes = 10,
                Customer = _testCustomer
            },
            new() {
                Id = Guid.NewGuid(),
                CustomerId = _testCustomer.Id,
                Channel = InteractionChannel.CallCenter,
                Subject = "Support Call",
                Description = "Technical support provided",
                InteractionDate = DateTime.UtcNow.AddHours(-1),
                DurationMinutes = 15,
                Customer = _testCustomer
            }
        };

        _mockInteractionRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(interactions);

        // Act
        var result = await _controller.GetAllInteractions();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedInteractions = okResult.Value as IEnumerable<InteractionDto>;
        returnedInteractions.Should().NotBeNull();
        returnedInteractions.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetInteraction_Should_Return_Ok_When_Interaction_Exists()
    {
        // Arrange
        var interactionId = Guid.NewGuid();
        var interaction = new Interaction
        {
            Id = interactionId,
            CustomerId = _testCustomer.Id,
            Channel = InteractionChannel.WhatsApp,
            Subject = "Test Interaction",
            Description = "Test Description",
            InteractionDate = DateTime.UtcNow,
            DurationMinutes = 5,
            Customer = _testCustomer
        };

        _mockInteractionRepository.Setup(r => r.GetByIdAsync(interactionId))
            .ReturnsAsync(interaction);

        // Act
        var result = await _controller.GetInteraction(interactionId);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedInteraction = okResult.Value.Should().BeAssignableTo<InteractionDto>().Subject;
        returnedInteraction.Id.Should().Be(interactionId);
        returnedInteraction.Subject.Should().Be("Test Interaction");
    }

    [Fact]
    public async Task GetInteraction_Should_Return_NotFound_When_Interaction_Does_Not_Exist()
    {
        // Arrange
        var interactionId = Guid.NewGuid();
        _mockInteractionRepository.Setup(r => r.GetByIdAsync(interactionId))
            .ReturnsAsync((Interaction?)null);

        // Act
        var result = await _controller.GetInteraction(interactionId);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task GetInteractionsByCustomer_Should_Return_Ok_With_Filtered_Interactions()
    {
        // Arrange
        var customerId = _testCustomer.Id;
        var interactions = new List<Interaction>
        {
            new() {
                Id = Guid.NewGuid(),
                CustomerId = customerId,
                Channel = InteractionChannel.Branch,
                Subject = "Branch Visit",
                Description = "Customer visited branch",
                InteractionDate = DateTime.UtcNow
            },
            new() {
                Id = Guid.NewGuid(),
                CustomerId = customerId,
                Channel = InteractionChannel.MobileApp,
                Subject = "App Login",
                Description = "Customer logged into app",
                InteractionDate = DateTime.UtcNow.AddMinutes(-30)
            }
        };

        _mockInteractionRepository.Setup(r => r.GetByCustomerIdAsync(customerId))
            .ReturnsAsync(interactions);

        // Act
        var result = await _controller.GetInteractionsByCustomer(customerId);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedInteractions = okResult.Value as IEnumerable<InteractionDto>;
        returnedInteractions.Should().NotBeNull();
        returnedInteractions.Should().HaveCount(2);
        returnedInteractions.Should().AllSatisfy(i => i.CustomerId.Should().Be(customerId));
    }

    [Fact]
    public async Task DeleteInteraction_Should_Return_NoContent_When_Interaction_Deleted()
    {
        // Arrange
        var interactionId = Guid.NewGuid();
        _mockInteractionRepository.Setup(r => r.DeleteAsync(interactionId))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteInteraction(interactionId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task DeleteInteraction_Should_Return_NotFound_When_Interaction_Does_Not_Exist()
    {
        // Arrange
        var interactionId = Guid.NewGuid();
        _mockInteractionRepository.Setup(r => r.DeleteAsync(interactionId))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.DeleteInteraction(interactionId);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Theory]
    [InlineData(InteractionChannel.Branch)]
    [InlineData(InteractionChannel.CallCenter)]
    [InlineData(InteractionChannel.Email)]
    [InlineData(InteractionChannel.SMS)]
    [InlineData(InteractionChannel.WhatsApp)]
    [InlineData(InteractionChannel.MobileApp)]
    [InlineData(InteractionChannel.Web)]
    [InlineData(InteractionChannel.ATM)]
    public async Task GetAllInteractions_Should_Handle_All_Channels(InteractionChannel channel)
    {
        // Arrange
        var interactions = new List<Interaction>
        {
            new() {
                Id = Guid.NewGuid(),
                CustomerId = _testCustomer.Id,
                Channel = channel,
                Subject = "Test Interaction",
                Description = "Test Description",
                InteractionDate = DateTime.UtcNow,
                Customer = _testCustomer
            }
        };

        _mockInteractionRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(interactions);

        // Act
        var result = await _controller.GetAllInteractions();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedInteractions = okResult.Value as IEnumerable<InteractionDto>;
        returnedInteractions.Should().NotBeNull();
        returnedInteractions.Should().HaveCount(1);
        returnedInteractions.First().Channel.Should().Be(channel);
    }
}
