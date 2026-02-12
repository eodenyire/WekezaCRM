using Microsoft.AspNetCore.Mvc;
using WekezaCRM.Application.Interfaces;
using WekezaCRM.Application.DTOs;
using WekezaCRM.Domain.Entities;
using WekezaCRM.Domain.Enums;

namespace WekezaCRM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class WhatsAppController : ControllerBase
{
    private readonly IWhatsAppRepository _repository;
    private readonly ILogger<WhatsAppController> _logger;

    public WhatsAppController(
        IWhatsAppRepository repository,
        ILogger<WhatsAppController> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <summary>
    /// Get all WhatsApp messages
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<WhatsAppMessageDto>>> GetAll()
    {
        try
        {
            var messages = await _repository.GetAllAsync();
            var messageDtos = messages.Select(MapToDto);
            return Ok(messageDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving WhatsApp messages");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get WhatsApp message by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<WhatsAppMessageDto>> GetById(Guid id)
    {
        try
        {
            var message = await _repository.GetByIdAsync(id);
            if (message == null)
                return NotFound($"WhatsApp message with ID {id} not found");

            return Ok(MapToDto(message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving WhatsApp message {MessageId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get WhatsApp messages for a customer
    /// </summary>
    [HttpGet("customer/{customerId}")]
    public async Task<ActionResult<IEnumerable<WhatsAppMessageDto>>> GetByCustomer(Guid customerId)
    {
        try
        {
            var messages = await _repository.GetByCustomerIdAsync(customerId);
            var messageDtos = messages.Select(MapToDto);
            return Ok(messageDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving WhatsApp messages for customer {CustomerId}", customerId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Send WhatsApp message
    /// </summary>
    [HttpPost("send")]
    public async Task<ActionResult<WhatsAppMessageDto>> SendMessage([FromBody] SendWhatsAppMessageRequest request)
    {
        try
        {
            var message = new WhatsAppMessage
            {
                Id = Guid.NewGuid(),
                CustomerId = request.CustomerId,
                PhoneNumber = request.PhoneNumber,
                MessageType = request.MessageType,
                Status = WhatsAppMessageStatus.Pending,
                Content = request.Content,
                MediaUrl = request.MediaUrl,
                TemplateId = request.TemplateId,
                IsInbound = false,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            };

            // Simulate sending message
            message.Status = WhatsAppMessageStatus.Sent;
            message.SentAt = DateTime.UtcNow;
            message.WhatsAppMessageId = $"wamid.{Guid.NewGuid():N}";

            var created = await _repository.CreateAsync(message);
            _logger.LogInformation("Sent WhatsApp message {MessageId} to {PhoneNumber}", created.Id, request.PhoneNumber);

            return Ok(MapToDto(created));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending WhatsApp message");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Receive WhatsApp webhook
    /// </summary>
    [HttpPost("webhook")]
    public async Task<ActionResult> ReceiveWebhook([FromBody] WhatsAppWebhookRequest request)
    {
        try
        {
            // Simulated webhook processing
            _logger.LogInformation("Received WhatsApp webhook for message {MessageId}", request.MessageId);

            // Update message status if exists
            // In production, find message by WhatsAppMessageId and update status
            
            return Ok(new { status = "received" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing WhatsApp webhook");
            return StatusCode(500, "Internal server error");
        }
    }

    private static WhatsAppMessageDto MapToDto(WhatsAppMessage message) => new()
    {
        Id = message.Id,
        CustomerId = message.CustomerId,
        PhoneNumber = message.PhoneNumber,
        MessageType = message.MessageType,
        Status = message.Status,
        Content = message.Content,
        MediaUrl = message.MediaUrl,
        SentAt = message.SentAt,
        DeliveredAt = message.DeliveredAt,
        ReadAt = message.ReadAt,
        IsInbound = message.IsInbound
    };
}

public record SendWhatsAppMessageRequest(
    Guid CustomerId,
    string PhoneNumber,
    WhatsAppMessageType MessageType,
    string Content,
    string? MediaUrl = null,
    string? TemplateId = null
);

public record WhatsAppWebhookRequest(
    string MessageId,
    string Status,
    string? Content = null
);
