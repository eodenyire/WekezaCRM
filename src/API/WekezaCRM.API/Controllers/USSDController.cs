using Microsoft.AspNetCore.Mvc;
using WekezaCRM.Application.Interfaces;
using WekezaCRM.Application.DTOs;
using WekezaCRM.Domain.Entities;
using WekezaCRM.Domain.Enums;

namespace WekezaCRM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class USSDController : ControllerBase
{
    private readonly IUSSDRepository _repository;
    private readonly ILogger<USSDController> _logger;

    public USSDController(
        IUSSDRepository repository,
        ILogger<USSDController> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <summary>
    /// Get all USSD sessions
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<USSDSessionDto>>> GetAll()
    {
        try
        {
            var sessions = await _repository.GetAllAsync();
            var sessionDtos = sessions.Select(MapToDto);
            return Ok(sessionDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving USSD sessions");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get USSD session by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<USSDSessionDto>> GetById(Guid id)
    {
        try
        {
            var session = await _repository.GetByIdAsync(id);
            if (session == null)
                return NotFound($"USSD session with ID {id} not found");

            return Ok(MapToDto(session));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving USSD session {SessionId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Handle USSD request (Africa's Mobile Money USSD)
    /// </summary>
    [HttpPost("handle")]
    public async Task<ActionResult<USSDResponse>> HandleUSSD([FromBody] USSDRequest request)
    {
        try
        {
            // Get or create session
            var session = await _repository.GetBySessionIdAsync(request.SessionId);
            
            if (session == null)
            {
                // New session
                session = new USSDSession
                {
                    Id = Guid.NewGuid(),
                    SessionId = request.SessionId,
                    PhoneNumber = request.PhoneNumber,
                    Status = USSDSessionStatus.Active,
                    CurrentMenu = "main",
                    MenuHistory = "main",
                    StartedAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                };
                await _repository.CreateAsync(session);
            }

            // Process USSD menu
            var response = ProcessUSSDMenu(session, request.Text);

            // Update session
            session.UserInput = request.Text;
            session.MenuHistory += $",{session.CurrentMenu}";
            session.UpdatedAt = DateTime.UtcNow;

            if (response.EndSession)
            {
                session.Status = USSDSessionStatus.Completed;
                session.CompletedAt = DateTime.UtcNow;
            }

            await _repository.UpdateAsync(session);

            _logger.LogInformation("Processed USSD session {SessionId} for {PhoneNumber}", 
                request.SessionId, request.PhoneNumber);

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling USSD request");
            return StatusCode(500, new USSDResponse 
            { 
                Text = "Service temporarily unavailable. Please try again later.",
                EndSession = true
            });
        }
    }

    private static USSDResponse ProcessUSSDMenu(USSDSession session, string? userInput)
    {
        // Simulated USSD menu processing
        if (string.IsNullOrEmpty(userInput))
        {
            // Initial menu
            return new USSDResponse
            {
                Text = "Welcome to Wekeza Bank\n" +
                       "1. Check Balance\n" +
                       "2. Mini Statement\n" +
                       "3. Transfer Money\n" +
                       "4. Pay Bills\n" +
                       "5. Customer Service",
                EndSession = false
            };
        }

        // Process menu selection
        return userInput switch
        {
            "1" => new USSDResponse
            {
                Text = "Your account balance is KES 15,450.00\n\n" +
                       "Thank you for using Wekeza Bank.",
                EndSession = true
            },
            "2" => new USSDResponse
            {
                Text = "Last 3 transactions:\n" +
                       "1. ATM Withdrawal: -500.00\n" +
                       "2. Salary Deposit: +25,000.00\n" +
                       "3. Utility Bill: -1,200.00",
                EndSession = true
            },
            "5" => new USSDResponse
            {
                Text = "Customer Service: 0800-WEKEZA\n" +
                       "Email: support@wekeza.com\n" +
                       "Available 24/7",
                EndSession = true
            },
            _ => new USSDResponse
            {
                Text = "Invalid selection. Please try again.",
                EndSession = true
            }
        };
    }

    private static USSDSessionDto MapToDto(USSDSession session) => new()
    {
        Id = session.Id,
        SessionId = session.SessionId,
        PhoneNumber = session.PhoneNumber,
        CustomerId = session.CustomerId,
        Status = session.Status,
        CurrentMenu = session.CurrentMenu,
        StartedAt = session.StartedAt,
        CompletedAt = session.CompletedAt
    };
}

public record USSDRequest(
    string SessionId,
    string PhoneNumber,
    string? Text = null
);

public record USSDResponse
{
    public string Text { get; set; } = string.Empty;
    public bool EndSession { get; set; }
}
