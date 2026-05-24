namespace PrinterStatusSignalR.Models;

public class PrinterConfig
{
    public string PrinterName { get; set; } = string.Empty;
    public bool IsOffline { get; set; }
    public bool IsBusy { get; set; }
    public int Jobs { get; set; }
    public bool IsTicketPrint { get; set; }
}
