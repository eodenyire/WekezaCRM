using Microsoft.AspNetCore.Mvc;
using WekezaCRM.Application.Interfaces;
using WekezaCRM.Application.DTOs;
using WekezaCRM.Domain.Entities;
using WekezaCRM.Domain.Enums;

namespace WekezaCRM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class NotificationsController : ControllerBase
{
    private readonly INotificationRepository _repository;
    private readonly ILogger<NotificationsController> _logger;

    public NotificationsController(
        INotificationRepository repository,
        ILogger<NotificationsController> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <summary>
    /// Get all notifications
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<NotificationDto>>> GetAll()
    {
        try
        {
            var notifications = await _repository.GetAllAsync();
            var notificationDtos = notifications.Select(MapToDto);
            return Ok(notificationDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving notifications");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get notification by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<NotificationDto>> GetById(Guid id)
    {
        try
        {
            var notification = await _repository.GetByIdAsync(id);
            if (notification == null)
                return NotFound($"Notification with ID {id} not found");

            return Ok(MapToDto(notification));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving notification {NotificationId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get notifications for a user
    /// </summary>
    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<NotificationDto>>> GetByUser(Guid userId)
    {
        try
        {
            var notifications = await _repository.GetByUserIdAsync(userId);
            var notificationDtos = notifications.Select(MapToDto);
            return Ok(notificationDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving notifications for user {UserId}", userId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get unread notifications for a user
    /// </summary>
    [HttpGet("user/{userId}/unread")]
    public async Task<ActionResult<IEnumerable<NotificationDto>>> GetUnreadByUser(Guid userId)
    {
        try
        {
            var notifications = await _repository.GetUnreadByUserIdAsync(userId);
            var notificationDtos = notifications.Select(MapToDto);
            return Ok(notificationDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving unread notifications for user {UserId}", userId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Create a notification
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<NotificationDto>> Create([FromBody] CreateNotificationRequest request)
    {
        try
        {
            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                CustomerId = request.CustomerId,
                Type = request.Type,
                Title = request.Title,
                Message = request.Message,
                IsRead = false,
                ActionUrl = request.ActionUrl,
                Metadata = request.Metadata,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            };

            var created = await _repository.CreateAsync(notification);
            _logger.LogInformation("Created notification {NotificationId}", created.Id);

            return CreatedAtAction(
                nameof(GetById),
                new { id = created.Id },
                MapToDto(created));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating notification");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Mark notification as read
    /// </summary>
    [HttpPut("{id}/read")]
    public async Task<ActionResult> MarkAsRead(Guid id)
    {
        try
        {
            var result = await _repository.MarkAsReadAsync(id);
            if (!result)
                return NotFound($"Notification with ID {id} not found");

            _logger.LogInformation("Marked notification {NotificationId} as read", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking notification {NotificationId} as read", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Delete notification
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        try
        {
            var result = await _repository.DeleteAsync(id);
            if (!result)
                return NotFound($"Notification with ID {id} not found");

            _logger.LogInformation("Deleted notification {NotificationId}", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting notification {NotificationId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    private static NotificationDto MapToDto(Notification notification) => new()
    {
        Id = notification.Id,
        UserId = notification.UserId,
        CustomerId = notification.CustomerId,
        Type = notification.Type,
        Title = notification.Title,
        Message = notification.Message,
        IsRead = notification.IsRead,
        ReadAt = notification.ReadAt,
        ActionUrl = notification.ActionUrl,
        CreatedAt = notification.CreatedAt
    };
}

public record CreateNotificationRequest(
    Guid? UserId,
    Guid? CustomerId,
    NotificationType Type,
    string Title,
    string Message,
    string? ActionUrl = null,
    string? Metadata = null
);
