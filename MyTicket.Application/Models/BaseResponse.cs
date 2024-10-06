namespace MyTicket.Application.Models;

public abstract class BaseResponse
{
    public string? CreatedBy { get; set; }
    public string? CreatedByName { get; set; }
    public string? CreatedByFullName { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? CreatedAtServer { get; set; }

    public string? LastUpdatedBy { get; set; }
    public string? LastUpdatedByName { get; set; }
    public string? LastUpdatedByFullName { get; set; }
    public DateTime? LastUpdatedAt { get; set; }
    public DateTime? LastUpdatedAtServer { get; set; }
}