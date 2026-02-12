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

public class CasesControllerTests
{
    private readonly Mock<ICaseRepository> _mockCaseRepository;
    private readonly Mock<ICustomerRepository> _mockCustomerRepository;
    private readonly Mock<ILogger<CasesController>> _mockLogger;
    private readonly CasesController _controller;
    private readonly Customer _testCustomer;

    public CasesControllerTests()
    {
        _mockCaseRepository = new Mock<ICaseRepository>();
        _mockCustomerRepository = new Mock<ICustomerRepository>();
        _mockLogger = new Mock<ILogger<CasesController>>();
        _controller = new CasesController(_mockCaseRepository.Object, _mockCustomerRepository.Object, _mockLogger.Object);

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
    public async Task GetAllCases_Should_Return_Ok_With_Cases()
    {
        // Arrange
        var cases = new List<Case>
        {
            new() {
                Id = Guid.NewGuid(),
                CustomerId = _testCustomer.Id,
                CaseNumber = "CASE001",
                Title = "Issue 1",
                Description = "Description 1",
                Status = CaseStatus.Open,
                Priority = CasePriority.High,
                Category = "Technical",
                SubCategory = "Login",
                Customer = _testCustomer
            },
            new() {
                Id = Guid.NewGuid(),
                CustomerId = _testCustomer.Id,
                CaseNumber = "CASE002",
                Title = "Issue 2",
                Description = "Description 2",
                Status = CaseStatus.InProgress,
                Priority = CasePriority.Medium,
                Category = "Billing",
                SubCategory = "Charges",
                Customer = _testCustomer
            }
        };

        _mockCaseRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(cases);

        // Act
        var result = await _controller.GetAllCases();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedCases = okResult.Value as IEnumerable<CaseDto>;
        returnedCases.Should().NotBeNull();
        returnedCases.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetCase_Should_Return_Ok_When_Case_Exists()
    {
        // Arrange
        var caseId = Guid.NewGuid();
        var caseEntity = new Case
        {
            Id = caseId,
            CustomerId = _testCustomer.Id,
            CaseNumber = "CASE001",
            Title = "Test Case",
            Description = "Test Description",
            Status = CaseStatus.Open,
            Priority = CasePriority.High,
            Category = "Technical",
            SubCategory = "Login",
            Customer = _testCustomer
        };

        _mockCaseRepository.Setup(r => r.GetByIdAsync(caseId))
            .ReturnsAsync(caseEntity);

        // Act
        var result = await _controller.GetCase(caseId);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedCase = okResult.Value.Should().BeAssignableTo<CaseDto>().Subject;
        returnedCase.Id.Should().Be(caseId);
        returnedCase.CaseNumber.Should().Be("CASE001");
    }

    [Fact]
    public async Task GetCase_Should_Return_NotFound_When_Case_Does_Not_Exist()
    {
        // Arrange
        var caseId = Guid.NewGuid();
        _mockCaseRepository.Setup(r => r.GetByIdAsync(caseId))
            .ReturnsAsync((Case?)null);

        // Act
        var result = await _controller.GetCase(caseId);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task GetCasesByCustomer_Should_Return_Ok_With_Filtered_Cases()
    {
        // Arrange
        var customerId = _testCustomer.Id;
        var cases = new List<Case>
        {
            new() {
                Id = Guid.NewGuid(),
                CustomerId = customerId,
                CaseNumber = "CASE001",
                Title = "Case 1",
                Description = "Description 1",
                Status = CaseStatus.Open,
                Priority = CasePriority.Low,
                Category = "General",
                SubCategory = "Info"
            },
            new() {
                Id = Guid.NewGuid(),
                CustomerId = customerId,
                CaseNumber = "CASE002",
                Title = "Case 2",
                Description = "Description 2",
                Status = CaseStatus.Resolved,
                Priority = CasePriority.High,
                Category = "Technical",
                SubCategory = "Bug"
            }
        };

        _mockCaseRepository.Setup(r => r.GetByCustomerIdAsync(customerId))
            .ReturnsAsync(cases);

        // Act
        var result = await _controller.GetCasesByCustomer(customerId);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedCases = okResult.Value as IEnumerable<CaseDto>;
        returnedCases.Should().NotBeNull();
        returnedCases.Should().HaveCount(2);
        returnedCases.Should().AllSatisfy(c => c.CustomerId.Should().Be(customerId));
    }

    [Fact]
    public async Task CreateCase_Should_Return_NotFound_When_Customer_Does_Not_Exist()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var request = new CreateCaseRequest(
            CustomerId: customerId,
            Title: "Test Case",
            Description: "Test Description",
            Priority: CasePriority.Medium,
            Category: "Technical",
            SubCategory: "System"
        );

        _mockCustomerRepository.Setup(r => r.GetByIdAsync(customerId))
            .ReturnsAsync((Customer?)null);

        // Act
        var result = await _controller.CreateCase(request);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task DeleteCase_Should_Return_NoContent_When_Case_Deleted()
    {
        // Arrange
        var caseId = Guid.NewGuid();
        _mockCaseRepository.Setup(r => r.DeleteAsync(caseId))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteCase(caseId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task DeleteCase_Should_Return_NotFound_When_Case_Does_Not_Exist()
    {
        // Arrange
        var caseId = Guid.NewGuid();
        _mockCaseRepository.Setup(r => r.DeleteAsync(caseId))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.DeleteCase(caseId);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Theory]
    [InlineData(CaseStatus.Open)]
    [InlineData(CaseStatus.InProgress)]
    [InlineData(CaseStatus.Resolved)]
    [InlineData(CaseStatus.Closed)]
    [InlineData(CaseStatus.Escalated)]
    [InlineData(CaseStatus.PendingCustomer)]
    public async Task GetAllCases_Should_Handle_All_Case_Statuses(CaseStatus status)
    {
        // Arrange
        var cases = new List<Case>
        {
            new() {
                Id = Guid.NewGuid(),
                CustomerId = _testCustomer.Id,
                CaseNumber = "CASE001",
                Title = "Test Case",
                Description = "Test Description",
                Status = status,
                Priority = CasePriority.Medium,
                Category = "General",
                SubCategory = "Info",
                Customer = _testCustomer
            }
        };

        _mockCaseRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(cases);

        // Act
        var result = await _controller.GetAllCases();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedCases = okResult.Value as IEnumerable<CaseDto>;
        returnedCases.Should().NotBeNull();
        returnedCases.Should().HaveCount(1);
        returnedCases.First().Status.Should().Be(status);
    }

    [Theory]
    [InlineData(CasePriority.Low)]
    [InlineData(CasePriority.Medium)]
    [InlineData(CasePriority.High)]
    [InlineData(CasePriority.Critical)]
    public async Task GetAllCases_Should_Handle_All_Case_Priorities(CasePriority priority)
    {
        // Arrange
        var cases = new List<Case>
        {
            new() {
                Id = Guid.NewGuid(),
                CustomerId = _testCustomer.Id,
                CaseNumber = "CASE001",
                Title = "Test Case",
                Description = "Test Description",
                Status = CaseStatus.Open,
                Priority = priority,
                Category = "General",
                SubCategory = "Info",
                Customer = _testCustomer
            }
        };

        _mockCaseRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(cases);

        // Act
        var result = await _controller.GetAllCases();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedCases = okResult.Value as IEnumerable<CaseDto>;
        returnedCases.Should().NotBeNull();
        returnedCases.Should().HaveCount(1);
        returnedCases.First().Priority.Should().Be(priority);
    }
}
