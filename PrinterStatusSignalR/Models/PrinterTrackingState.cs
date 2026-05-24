using PrinterStatusSignalR.Models.Enums;

namespace PrinterStatusSignalR.Models;

public class PrinterTrackingState
{
    public int LastJobs { get; set; } = 0;
    public DateTime LastChangeTime { get; set; } = DateTime.UtcNow;
    public PrinterHealthStatus Status { get; set; } = PrinterHealthStatus.Unknown;

}
