namespace ReviewAnythingAPI.Models;

public class Report
{
    public int ReportId { get; set; }
    public int? ReporterUserId { get; set; }
    public int TargetId { get; set; }
    public string ReportedItemType { get; set; }
    public int ReportReasonId { get; set; }
    public string ReportDetails { get; set; }
    public DateTime ReportDate { get; set; }
    public int StatusReportId { get; set; }
    public int? ReviewedByUserId { get; set; }
    public DateTime? ReviewedDate { get; set; }
    
    public ApplicationUser? UserCreator { get; set; }
    public ApplicationUser? ReviewerUser { get; set; }
    public StatusReport StatusReportName { get; set; }
    public ReportReason ReportReasonName { get; set; }

}