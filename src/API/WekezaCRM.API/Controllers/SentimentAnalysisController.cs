using Microsoft.AspNetCore.Mvc;
using WekezaCRM.Application.Interfaces;
using WekezaCRM.Application.DTOs;
using WekezaCRM.Domain.Entities;
using WekezaCRM.Domain.Enums;

namespace WekezaCRM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class SentimentAnalysisController : ControllerBase
{
    private readonly ISentimentAnalysisRepository _repository;
    private readonly ILogger<SentimentAnalysisController> _logger;

    public SentimentAnalysisController(
        ISentimentAnalysisRepository repository,
        ILogger<SentimentAnalysisController> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <summary>
    /// Get all sentiment analyses
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SentimentAnalysisDto>>> GetAll()
    {
        try
        {
            var analyses = await _repository.GetAllAsync();
            var analysisDtos = analyses.Select(MapToDto);
            return Ok(analysisDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving sentiment analyses");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get sentiment analysis by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<SentimentAnalysisDto>> GetById(Guid id)
    {
        try
        {
            var analysis = await _repository.GetByIdAsync(id);
            if (analysis == null)
                return NotFound($"Sentiment analysis with ID {id} not found");

            return Ok(MapToDto(analysis));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving sentiment analysis {AnalysisId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get sentiment analyses for a customer
    /// </summary>
    [HttpGet("customer/{customerId}")]
    public async Task<ActionResult<IEnumerable<SentimentAnalysisDto>>> GetByCustomer(Guid customerId)
    {
        try
        {
            var analyses = await _repository.GetByCustomerIdAsync(customerId);
            var analysisDtos = analyses.Select(MapToDto);
            return Ok(analysisDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving sentiment for customer {CustomerId}", customerId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get sentiment analysis for an interaction
    /// </summary>
    [HttpGet("interaction/{interactionId}")]
    public async Task<ActionResult<IEnumerable<SentimentAnalysisDto>>> GetByInteraction(Guid interactionId)
    {
        try
        {
            var analyses = await _repository.GetByInteractionIdAsync(interactionId);
            var analysisDtos = analyses.Select(MapToDto);
            return Ok(analysisDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving sentiment for interaction {InteractionId}", interactionId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get sentiment analysis for a case
    /// </summary>
    [HttpGet("case/{caseId}")]
    public async Task<ActionResult<IEnumerable<SentimentAnalysisDto>>> GetByCase(Guid caseId)
    {
        try
        {
            var analyses = await _repository.GetByCaseIdAsync(caseId);
            var analysisDtos = analyses.Select(MapToDto);
            return Ok(analysisDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving sentiment for case {CaseId}", caseId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Analyze sentiment for text (AI simulation)
    /// </summary>
    [HttpPost("analyze")]
    public async Task<ActionResult<SentimentAnalysisDto>> AnalyzeSentiment([FromBody] AnalyzeSentimentRequest request)
    {
        try
        {
            // Simulated sentiment analysis - in production this would call ML service
            var (sentimentType, score) = SimulateAnalysis(request.Text);

            var analysis = new SentimentAnalysis
            {
                Id = Guid.NewGuid(),
                CustomerId = request.CustomerId,
                InteractionId = request.InteractionId,
                CaseId = request.CaseId,
                SentimentType = sentimentType,
                SentimentScore = score,
                TextAnalyzed = request.Text,
                AnalyzedDate = DateTime.UtcNow,
                KeyPhrases = ExtractKeyPhrases(request.Text),
                AnalysisMetadata = "Simulated analysis v1.0",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "AI-Engine"
            };

            var created = await _repository.CreateAsync(analysis);
            _logger.LogInformation("Analyzed sentiment for customer {CustomerId}", request.CustomerId);

            return Ok(MapToDto(created));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error analyzing sentiment");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Delete sentiment analysis
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        try
        {
            var result = await _repository.DeleteAsync(id);
            if (!result)
                return NotFound($"Sentiment analysis with ID {id} not found");

            _logger.LogInformation("Deleted sentiment analysis {AnalysisId}", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting sentiment analysis {AnalysisId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    // Simulated sentiment analysis logic
    private static (SentimentType, decimal) SimulateAnalysis(string text)
    {
        var lowerText = text.ToLower();
        
        // Simple keyword-based simulation
        var positiveWords = new[] { "happy", "great", "excellent", "thank", "satisfied", "good", "appreciate" };
        var negativeWords = new[] { "unhappy", "bad", "terrible", "angry", "frustrated", "poor", "disappointed" };

        var positiveCount = positiveWords.Count(w => lowerText.Contains(w));
        var negativeCount = negativeWords.Count(w => lowerText.Contains(w));

        if (negativeCount > positiveCount + 1)
            return (SentimentType.VeryNegative, 0.2m);
        if (negativeCount > positiveCount)
            return (SentimentType.Negative, 0.4m);
        if (positiveCount > negativeCount)
            return (SentimentType.Positive, 0.8m);
        
        return (SentimentType.Neutral, 0.5m);
    }

    private static string ExtractKeyPhrases(string text)
    {
        // Simple simulation - in production would use NLP
        var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return string.Join(", ", words.Take(5));
    }

    private static SentimentAnalysisDto MapToDto(SentimentAnalysis analysis) => new()
    {
        Id = analysis.Id,
        CustomerId = analysis.CustomerId,
        InteractionId = analysis.InteractionId,
        CaseId = analysis.CaseId,
        SentimentType = analysis.SentimentType,
        SentimentScore = analysis.SentimentScore,
        TextAnalyzed = analysis.TextAnalyzed,
        AnalyzedDate = analysis.AnalyzedDate,
        KeyPhrases = analysis.KeyPhrases
    };
}

public record AnalyzeSentimentRequest(
    Guid CustomerId,
    string Text,
    Guid? InteractionId = null,
    Guid? CaseId = null
);
