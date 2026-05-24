namespace PrinterStatusSignalR.Model;

public class Printers
{
    public int StuckAfterSeconds { get; set; } = 10;
    public int TicketStuckAfterSeconds { get; set; } = 5;
    public List<string> SelectedPrinter { get; set; } = new();
}

