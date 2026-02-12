using System.Net;
using System.Net.Http.Json;
using Xunit;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using WekezaCRM.API;

namespace WekezaCRM.API.Tests;

public class EndpointExistenceTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public EndpointExistenceTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Health_Check_Should_Return_OK()
    {
        // Act
        var response = await _client.GetAsync("/api/health");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Theory]
    [InlineData("/api/customers")]
    [InlineData("/api/cases")]
    [InlineData("/api/interactions")]
    [InlineData("/api/nextbestactions")]
    [InlineData("/api/sentimentanalysis")]
    [InlineData("/api/workflows/definitions")]
    [InlineData("/api/workflows/instances")]
    [InlineData("/api/notifications")]
    [InlineData("/api/analytics/customers")]
    [InlineData("/api/analytics/cases")]
    [InlineData("/api/analytics/interactions")]
    [InlineData("/api/analytics/dashboard")]
    [InlineData("/api/whatsapp")]
    [InlineData("/api/ussd")]
    [InlineData("/api/reports/templates")]
    public async Task Endpoint_Should_Exist(string endpoint)
    {
        // Act
        var response = await _client.GetAsync(endpoint);

        // Assert  
        // We just want to verify the endpoint exists (not 404)
        // It might return 500 if DB not set up properly, but that's OK for this test
        response.StatusCode.Should().NotBe(HttpStatusCode.NotFound);
    }
}
