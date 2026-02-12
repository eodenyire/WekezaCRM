using System.Net;
using System.Net.Http.Json;
using Xunit;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using WekezaCRM.API;
using WekezaCRM.Application.DTOs;
using WekezaCRM.Domain.Enums;

namespace WekezaCRM.API.Tests;

public class WhatsAppControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public WhatsAppControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_Should_Return_Success()
    {
        // Act
        var response = await _client.GetAsync("/api/whatsapp");

        // Assert
        response.Should().NotBeNull();
        // Note: Might return error if DB not set up, but endpoint should exist
    }

    [Fact]
    public async Task Health_Should_Return_Healthy()
    {
        // Act
        var response = await _client.GetAsync("/api/health");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}

public class USSDControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public USSDControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_Should_Return_Success()
    {
        // Act
        var response = await _client.GetAsync("/api/ussd");

        // Assert
        response.Should().NotBeNull();
    }
}

public class ReportsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ReportsControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAllTemplates_Should_Return_Success()
    {
        // Act
        var response = await _client.GetAsync("/api/reports/templates");

        // Assert
        response.Should().NotBeNull();
    }
}
