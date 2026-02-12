using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using WekezaCRM.API.Controllers;
using WekezaCRM.Application.Interfaces;
using WekezaCRM.Domain.Entities;
using WekezaCRM.Domain.Enums;
using WekezaCRM.Application.DTOs;
using Xunit;

namespace WekezaCRM.UnitTests.Controllers;

public class CustomersControllerTests
{
    private readonly Mock<ICustomerRepository> _mockRepository;
    private readonly Mock<ILogger<CustomersController>> _mockLogger;
    private readonly CustomersController _controller;

    public CustomersControllerTests()
    {
        _mockRepository = new Mock<ICustomerRepository>();
        _mockLogger = new Mock<ILogger<CustomersController>>();
        _controller = new CustomersController(_mockRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetAllCustomers_Should_Return_Ok_With_Customers()
    {
        // Arrange
        var customers = new List<Customer>
        {
            new() {
                Id = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                PhoneNumber = "+254700000000",
                Segment = CustomerSegment.Retail,
                KYCStatus = KYCStatus.Verified,
                IsActive = true
            },
            new() {
                Id = Guid.NewGuid(),
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane@example.com",
                PhoneNumber = "+254711111111",
                Segment = CustomerSegment.SME,
                KYCStatus = KYCStatus.Pending,
                IsActive = true
            }
        };

        _mockRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(customers);

        // Act
        var result = await _controller.GetAllCustomers();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedCustomers = okResult.Value as IEnumerable<CustomerDto>;
        returnedCustomers.Should().NotBeNull();
        returnedCustomers.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetCustomer_Should_Return_Ok_When_Customer_Exists()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var customer = new Customer
        {
            Id = customerId,
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            PhoneNumber = "+254700000000",
            Segment = CustomerSegment.Retail,
            KYCStatus = KYCStatus.Verified,
            IsActive = true
        };

        _mockRepository.Setup(r => r.GetByIdAsync(customerId))
            .ReturnsAsync(customer);

        // Act
        var result = await _controller.GetCustomer(customerId);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedCustomer = okResult.Value.Should().BeAssignableTo<CustomerDto>().Subject;
        returnedCustomer.Id.Should().Be(customerId);
        returnedCustomer.FirstName.Should().Be("John");
    }

    [Fact]
    public async Task GetCustomer_Should_Return_NotFound_When_Customer_Does_Not_Exist()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        _mockRepository.Setup(r => r.GetByIdAsync(customerId))
            .ReturnsAsync((Customer?)null);

        // Act
        var result = await _controller.GetCustomer(customerId);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task GetCustomerByEmail_Should_Return_Ok_When_Customer_Exists()
    {
        // Arrange
        var email = "john@example.com";
        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Doe",
            Email = email,
            PhoneNumber = "+254700000000",
            Segment = CustomerSegment.Retail,
            KYCStatus = KYCStatus.Verified,
            IsActive = true
        };

        _mockRepository.Setup(r => r.GetByEmailAsync(email))
            .ReturnsAsync(customer);

        // Act
        var result = await _controller.GetCustomerByEmail(email);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedCustomer = okResult.Value.Should().BeAssignableTo<CustomerDto>().Subject;
        returnedCustomer.Email.Should().Be(email);
    }

    [Fact]
    public async Task GetCustomerByEmail_Should_Return_NotFound_When_Customer_Does_Not_Exist()
    {
        // Arrange
        var email = "nonexistent@example.com";
        _mockRepository.Setup(r => r.GetByEmailAsync(email))
            .ReturnsAsync((Customer?)null);

        // Act
        var result = await _controller.GetCustomerByEmail(email);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task GetCustomersBySegment_Should_Return_Ok_With_Filtered_Customers()
    {
        // Arrange
        var segment = "Retail";
        var customers = new List<Customer>
        {
            new() {
                Id = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                PhoneNumber = "+254700000000",
                Segment = CustomerSegment.Retail,
                KYCStatus = KYCStatus.Verified,
                IsActive = true
            },
            new() {
                Id = Guid.NewGuid(),
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane@example.com",
                PhoneNumber = "+254711111111",
                Segment = CustomerSegment.Retail,
                KYCStatus = KYCStatus.Pending,
                IsActive = true
            }
        };

        _mockRepository.Setup(r => r.GetBySegmentAsync(segment))
            .ReturnsAsync(customers);

        // Act
        var result = await _controller.GetCustomersBySegment(segment);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedCustomers = okResult.Value as IEnumerable<CustomerDto>;
        returnedCustomers.Should().NotBeNull();
        returnedCustomers.Should().HaveCount(2);
        returnedCustomers.Should().AllSatisfy(c => c.Segment.Should().Be(CustomerSegment.Retail));
    }

    [Fact]
    public async Task CreateCustomer_Should_Return_CreatedAtAction()
    {
        // Arrange
        var request = new CreateCustomerRequest(
            FirstName: "New",
            LastName: "Customer",
            Email: "new@example.com",
            PhoneNumber: "+254722222222",
            DateOfBirth: new DateTime(1990, 1, 1),
            Address: "123 Street",
            City: "Nairobi",
            Country: "Kenya",
            Segment: CustomerSegment.Retail
        );

        _mockRepository.Setup(r => r.CreateAsync(It.IsAny<Customer>()))
            .ReturnsAsync((Customer c) => c);

        // Act
        var result = await _controller.CreateCustomer(request);

        // Assert
        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        var returnedCustomer = createdResult.Value.Should().BeAssignableTo<CustomerDto>().Subject;
        returnedCustomer.FirstName.Should().Be("New");
        returnedCustomer.LastName.Should().Be("Customer");
        returnedCustomer.Email.Should().Be("new@example.com");
        returnedCustomer.KYCStatus.Should().Be(KYCStatus.Pending);
        returnedCustomer.IsActive.Should().BeTrue();
        returnedCustomer.CustomerReference.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task UpdateCustomer_Should_Return_Ok_When_Customer_Exists()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var existingCustomer = new Customer
        {
            Id = customerId,
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            PhoneNumber = "+254700000000",
            Segment = CustomerSegment.Retail,
            KYCStatus = KYCStatus.Verified,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System"
        };

        var updateRequest = new UpdateCustomerRequest(
            FirstName: "Updated",
            LastName: "Name",
            PhoneNumber: "+254733333333",
            Address: "New Address",
            City: "Mombasa",
            Country: "Kenya",
            Segment: CustomerSegment.SME
        );

        _mockRepository.Setup(r => r.GetByIdAsync(customerId))
            .ReturnsAsync(existingCustomer);

        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Customer>()))
            .ReturnsAsync((Customer c) => c);

        // Act
        var result = await _controller.UpdateCustomer(customerId, updateRequest);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedCustomer = okResult.Value.Should().BeAssignableTo<CustomerDto>().Subject;
        returnedCustomer.FirstName.Should().Be("Updated");
        returnedCustomer.LastName.Should().Be("Name");
        returnedCustomer.Segment.Should().Be(CustomerSegment.SME);
    }

    [Fact]
    public async Task UpdateCustomer_Should_Return_NotFound_When_Customer_Does_Not_Exist()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var updateRequest = new UpdateCustomerRequest(
            FirstName: "Updated",
            LastName: null,
            PhoneNumber: null,
            Address: null,
            City: null,
            Country: null,
            Segment: null
        );

        _mockRepository.Setup(r => r.GetByIdAsync(customerId))
            .ReturnsAsync((Customer?)null);

        // Act
        var result = await _controller.UpdateCustomer(customerId, updateRequest);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task DeleteCustomer_Should_Return_NoContent_When_Customer_Deleted()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        _mockRepository.Setup(r => r.DeleteAsync(customerId))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteCustomer(customerId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task DeleteCustomer_Should_Return_NotFound_When_Customer_Does_Not_Exist()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        _mockRepository.Setup(r => r.DeleteAsync(customerId))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.DeleteCustomer(customerId);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task CreateCustomer_Should_Generate_Unique_Customer_Reference()
    {
        // Arrange
        var request = new CreateCustomerRequest(
            FirstName: "Test",
            LastName: "Customer",
            Email: "test@example.com",
            PhoneNumber: "+254744444444",
            DateOfBirth: null,
            Address: null,
            City: null,
            Country: null,
            Segment: CustomerSegment.HighNetWorth
        );

        Customer? capturedCustomer = null;
        _mockRepository.Setup(r => r.CreateAsync(It.IsAny<Customer>()))
            .Callback<Customer>(c => capturedCustomer = c)
            .ReturnsAsync((Customer c) => c);

        // Act
        await _controller.CreateCustomer(request);

        // Assert
        capturedCustomer.Should().NotBeNull();
        capturedCustomer!.CustomerReference.Should().NotBeNullOrEmpty();
        capturedCustomer.CustomerReference.Should().StartWith("CUS-");
    }

    [Fact]
    public async Task UpdateCustomer_Should_Only_Update_Provided_Fields()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var existingCustomer = new Customer
        {
            Id = customerId,
            FirstName = "Original",
            LastName = "Name",
            Email = "original@example.com",
            PhoneNumber = "+254700000000",
            Address = "Original Address",
            City = "Nairobi",
            Country = "Kenya",
            Segment = CustomerSegment.Retail,
            KYCStatus = KYCStatus.Verified,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System"
        };

        var updateRequest = new UpdateCustomerRequest(
            FirstName: "Updated",
            LastName: null, // Should not change
            PhoneNumber: null, // Should not change
            Address: null,
            City: null,
            Country: null,
            Segment: null
        );

        _mockRepository.Setup(r => r.GetByIdAsync(customerId))
            .ReturnsAsync(existingCustomer);

        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Customer>()))
            .ReturnsAsync((Customer c) => c);

        // Act
        var result = await _controller.UpdateCustomer(customerId, updateRequest);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedCustomer = okResult.Value.Should().BeAssignableTo<CustomerDto>().Subject;
        returnedCustomer.FirstName.Should().Be("Updated");
        returnedCustomer.LastName.Should().Be("Name"); // Should remain unchanged
        returnedCustomer.PhoneNumber.Should().Be("+254700000000"); // Should remain unchanged
    }
}
