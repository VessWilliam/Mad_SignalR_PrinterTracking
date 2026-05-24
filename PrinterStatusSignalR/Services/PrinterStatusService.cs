using Microsoft.Extensions.Options;
using PrinterStatus.Services.IService;
using PrinterStatusSignalR.Model;
using PrinterStatusSignalR.Models;
using PrinterStatusSignalR.Models.Enums;
using System.Printing;
namespace PrinterStatusSignalR.Services;

public class PrinterStatusService : IPrinterStatusService
{
    private readonly Printers _printers;

    public PrinterStatusService(IOptions<Printers> printers)
    {
        _printers = printers.Value;
    }

    public string GetPrinterAnalyze(PrinterConfig printer, PrinterTrackingState stats)
    {
        if (printer.IsOffline)
            return nameof(PrinterHealthStatus.Offline);

        DateTime now = DateTime.UtcNow;

        if (stats.LastChangeTime == default)
        {
            stats.LastChangeTime = now;
            stats.LastJobs = printer.Jobs;
        }

        if (printer.Jobs == 0)
        {
            stats.LastJobs = 0;
            stats.LastChangeTime = now;
            return nameof(PrinterHealthStatus.Ready);
        }

        var jobDelta = printer.Jobs - stats.LastJobs;

        if (jobDelta != 0)
        {
            stats.LastJobs = printer.Jobs;
            stats.LastChangeTime = now;
            return nameof(PrinterHealthStatus.Printing);
        }

        var stuckThreshold = printer.IsTicketPrint
            ? TimeSpan.FromSeconds(_printers.TicketStuckAfterSeconds)
            : TimeSpan.FromSeconds(_printers.StuckAfterSeconds);

        if (now - stats.LastChangeTime > stuckThreshold)
            return nameof(PrinterHealthStatus.Stuck);

        return nameof(PrinterHealthStatus.Printing);
    }

    public IEnumerable<PrinterConfig> GetPrinterStatus()
    {
        var server = new LocalPrintServer();
        var queues = server.GetPrintQueues();

        var configMaps = _printers.SelectedPrinter
            .Where(p => !string.IsNullOrEmpty(p))
            .ToDictionary(p => p, StringComparer.OrdinalIgnoreCase);

        return queues.Where(q => configMaps.ContainsKey(q.Name))
               .Select(q => new PrinterConfig
               {
                   PrinterName = q.Name,
                   IsOffline = q.IsOffline,
                   IsBusy = q.IsBusy,
                   Jobs = q.NumberOfJobs,
                   IsTicketPrint = false
               }).ToList();
    }
}
