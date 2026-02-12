using Microsoft.AspNetCore.Mvc;
using WekezaCRM.Application.Interfaces;
using WekezaCRM.Application.DTOs;

namespace WekezaCRM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AnalyticsController : ControllerBase
{
    private readonly IAnalyticsRepository _repository;
    private readonly ILogger<AnalyticsController> _logger;

    public AnalyticsController(
        IAnalyticsRepository repository,
        ILogger<AnalyticsController> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <summary>
    /// Get customer analytics
    /// </summary>
    [HttpGet("customers")]
    public async Task<ActionResult<CustomerAnalyticsDto>> GetCustomerAnalytics(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        try
        {
            var analytics = await _repository.GetCustomerAnalyticsAsync(startDate, endDate);
            return Ok(analytics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving customer analytics");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get case analytics
    /// </summary>
    [HttpGet("cases")]
    public async Task<ActionResult<CaseAnalyticsDto>> GetCaseAnalytics(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        try
        {
            var analytics = await _repository.GetCaseAnalyticsAsync(startDate, endDate);
            return Ok(analytics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving case analytics");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get interaction analytics
    /// </summary>
    [HttpGet("interactions")]
    public async Task<ActionResult<InteractionAnalyticsDto>> GetInteractionAnalytics(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        try
        {
            var analytics = await _repository.GetInteractionAnalyticsAsync(startDate, endDate);
            return Ok(analytics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving interaction analytics");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get comprehensive dashboard analytics
    /// </summary>
    [HttpGet("dashboard")]
    public async Task<ActionResult<DashboardAnalyticsDto>> GetDashboardAnalytics(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        try
        {
            var customerAnalytics = await _repository.GetCustomerAnalyticsAsync(startDate, endDate);
            var caseAnalytics = await _repository.GetCaseAnalyticsAsync(startDate, endDate);
            var interactionAnalytics = await _repository.GetInteractionAnalyticsAsync(startDate, endDate);

            var dashboard = new DashboardAnalyticsDto
            {
                CustomerAnalytics = customerAnalytics,
                CaseAnalytics = caseAnalytics,
                InteractionAnalytics = interactionAnalytics,
                GeneratedAt = DateTime.UtcNow
            };

            return Ok(dashboard);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving dashboard analytics");
            return StatusCode(500, "Internal server error");
        }
    }
}

public record DashboardAnalyticsDto
{
    public CustomerAnalyticsDto CustomerAnalytics { get; set; } = null!;
    public CaseAnalyticsDto CaseAnalytics { get; set; } = null!;
    public InteractionAnalyticsDto InteractionAnalytics { get; set; } = null!;
    public DateTime GeneratedAt { get; set; }
}
