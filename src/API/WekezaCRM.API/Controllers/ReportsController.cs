using Microsoft.AspNetCore.Mvc;
using WekezaCRM.Application.Interfaces;
using WekezaCRM.Application.DTOs;
using WekezaCRM.Domain.Entities;
using WekezaCRM.Domain.Enums;

namespace WekezaCRM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ReportsController : ControllerBase
{
    private readonly IReportRepository _repository;
    private readonly ILogger<ReportsController> _logger;

    public ReportsController(
        IReportRepository repository,
        ILogger<ReportsController> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    // Report Templates

    /// <summary>
    /// Get all report templates
    /// </summary>
    [HttpGet("templates")]
    public async Task<ActionResult<IEnumerable<ReportTemplateDto>>> GetAllTemplates()
    {
        try
        {
            var templates = await _repository.GetAllTemplatesAsync();
            var templateDtos = templates.Select(MapTemplateToDto);
            return Ok(templateDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving report templates");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get report template by ID
    /// </summary>
    [HttpGet("templates/{id}")]
    public async Task<ActionResult<ReportTemplateDto>> GetTemplate(Guid id)
    {
        try
        {
            var template = await _repository.GetTemplateByIdAsync(id);
            if (template == null)
                return NotFound($"Report template with ID {id} not found");

            return Ok(MapTemplateToDto(template));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving report template {TemplateId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Create report template
    /// </summary>
    [HttpPost("templates")]
    public async Task<ActionResult<ReportTemplateDto>> CreateTemplate([FromBody] CreateReportTemplateRequest request)
    {
        try
        {
            var template = new ReportTemplate
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                ReportType = request.ReportType,
                QueryDefinition = request.QueryDefinition,
                ParametersSchema = request.ParametersSchema ?? "{}",
                DefaultFormat = request.DefaultFormat,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            };

            var created = await _repository.CreateTemplateAsync(template);
            _logger.LogInformation("Created report template {TemplateId}", created.Id);

            return CreatedAtAction(nameof(GetTemplate), new { id = created.Id }, MapTemplateToDto(created));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating report template");
            return StatusCode(500, "Internal server error");
        }
    }

    // Report Schedules

    /// <summary>
    /// Get all report schedules
    /// </summary>
    [HttpGet("schedules")]
    public async Task<ActionResult<IEnumerable<ReportScheduleDto>>> GetAllSchedules()
    {
        try
        {
            var schedules = await _repository.GetAllSchedulesAsync();
            var scheduleDtos = schedules.Select(MapScheduleToDto);
            return Ok(scheduleDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving report schedules");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Create report schedule
    /// </summary>
    [HttpPost("schedules")]
    public async Task<ActionResult<ReportScheduleDto>> CreateSchedule([FromBody] CreateReportScheduleRequest request)
    {
        try
        {
            var schedule = new ReportSchedule
            {
                Id = Guid.NewGuid(),
                ReportTemplateId = request.ReportTemplateId,
                Name = request.Name,
                Frequency = request.Frequency,
                NextRunDate = CalculateNextRunDate(request.Frequency),
                IsActive = true,
                Recipients = request.Recipients,
                OutputFormat = request.OutputFormat,
                Parameters = request.Parameters,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            };

            var created = await _repository.CreateScheduleAsync(schedule);
            _logger.LogInformation("Created report schedule {ScheduleId}", created.Id);

            return Ok(MapScheduleToDto(created));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating report schedule");
            return StatusCode(500, "Internal server error");
        }
    }

    // Generated Reports

    /// <summary>
    /// Get all generated reports
    /// </summary>
    [HttpGet("generated")]
    public async Task<ActionResult<IEnumerable<GeneratedReportDto>>> GetAllGeneratedReports()
    {
        try
        {
            var reports = await _repository.GetAllGeneratedReportsAsync();
            var reportDtos = reports.Select(MapGeneratedReportToDto);
            return Ok(reportDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving generated reports");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Generate report
    /// </summary>
    [HttpPost("generate")]
    public async Task<ActionResult<GeneratedReportDto>> GenerateReport([FromBody] GenerateReportRequest request)
    {
        try
        {
            // Get template
            var template = await _repository.GetTemplateByIdAsync(request.TemplateId);
            if (template == null)
                return NotFound($"Report template with ID {request.TemplateId} not found");

            // Simulate report generation
            var report = new GeneratedReport
            {
                Id = Guid.NewGuid(),
                ReportTemplateId = request.TemplateId,
                ReportName = $"{template.Name}_{DateTime.UtcNow:yyyyMMddHHmmss}",
                GeneratedDate = DateTime.UtcNow,
                Format = request.Format ?? template.DefaultFormat,
                FilePath = $"/reports/{Guid.NewGuid()}.{GetFileExtension(request.Format ?? template.DefaultFormat)}",
                FileSizeBytes = 1024 * 150, // Simulated 150KB
                RecordCount = 250, // Simulated record count
                Parameters = request.Parameters,
                GeneratedByUserId = request.UserId,
                IsDownloaded = false,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            };

            var created = await _repository.CreateGeneratedReportAsync(report);
            _logger.LogInformation("Generated report {ReportId} from template {TemplateId}", 
                created.Id, request.TemplateId);

            return Ok(MapGeneratedReportToDto(created));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating report");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Download report
    /// </summary>
    [HttpGet("download/{id}")]
    public async Task<ActionResult> DownloadReport(Guid id)
    {
        try
        {
            var report = await _repository.GetGeneratedReportByIdAsync(id);
            if (report == null)
                return NotFound($"Generated report with ID {id} not found");

            // In production, return actual file
            // For simulation, return placeholder
            var content = System.Text.Encoding.UTF8.GetBytes($"Report: {report.ReportName}\nGenerated: {report.GeneratedDate}");
            var contentType = GetContentType(report.Format);
            var fileName = $"{report.ReportName}.{GetFileExtension(report.Format)}";

            return File(content, contentType, fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading report {ReportId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    private static DateTime CalculateNextRunDate(ReportScheduleFrequency frequency)
    {
        var now = DateTime.UtcNow;
        return frequency switch
        {
            ReportScheduleFrequency.Daily => now.AddDays(1),
            ReportScheduleFrequency.Weekly => now.AddDays(7),
            ReportScheduleFrequency.Monthly => now.AddMonths(1),
            ReportScheduleFrequency.Quarterly => now.AddMonths(3),
            ReportScheduleFrequency.Yearly => now.AddYears(1),
            _ => now
        };
    }

    private static string GetFileExtension(ReportFormat format) => format switch
    {
        ReportFormat.PDF => "pdf",
        ReportFormat.Excel => "xlsx",
        ReportFormat.CSV => "csv",
        ReportFormat.JSON => "json",
        _ => "txt"
    };

    private static string GetContentType(ReportFormat format) => format switch
    {
        ReportFormat.PDF => "application/pdf",
        ReportFormat.Excel => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        ReportFormat.CSV => "text/csv",
        ReportFormat.JSON => "application/json",
        _ => "text/plain"
    };

    private static ReportTemplateDto MapTemplateToDto(ReportTemplate template) => new()
    {
        Id = template.Id,
        Name = template.Name,
        Description = template.Description,
        ReportType = template.ReportType,
        DefaultFormat = template.DefaultFormat,
        IsActive = template.IsActive
    };

    private static ReportScheduleDto MapScheduleToDto(ReportSchedule schedule) => new()
    {
        Id = schedule.Id,
        ReportTemplateId = schedule.ReportTemplateId,
        Name = schedule.Name,
        Frequency = schedule.Frequency,
        NextRunDate = schedule.NextRunDate,
        LastRunDate = schedule.LastRunDate,
        IsActive = schedule.IsActive,
        OutputFormat = schedule.OutputFormat
    };

    private static GeneratedReportDto MapGeneratedReportToDto(GeneratedReport report) => new()
    {
        Id = report.Id,
        ReportTemplateId = report.ReportTemplateId,
        ReportName = report.ReportName,
        GeneratedDate = report.GeneratedDate,
        Format = report.Format,
        FilePath = report.FilePath,
        FileSizeBytes = report.FileSizeBytes,
        RecordCount = report.RecordCount,
        IsDownloaded = report.IsDownloaded
    };
}

public record CreateReportTemplateRequest(
    string Name,
    string Description,
    string ReportType,
    string QueryDefinition,
    string? ParametersSchema,
    ReportFormat DefaultFormat
);

public record CreateReportScheduleRequest(
    Guid ReportTemplateId,
    string Name,
    ReportScheduleFrequency Frequency,
    string Recipients,
    ReportFormat OutputFormat,
    string? Parameters
);

public record GenerateReportRequest(
    Guid TemplateId,
    ReportFormat? Format = null,
    string? Parameters = null,
    Guid? UserId = null
);
