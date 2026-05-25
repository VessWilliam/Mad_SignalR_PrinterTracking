using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using PrinterStatus.Services.IService;
using PrinterStatusSignalR.Hubs;
using PrinterStatusSignalR.Models;

namespace PrinterStatusSignalR.PrinterMonitorWorker;

public class PrinterMonitorWorker : BackgroundService
{
    private readonly IHubContext<PrinterStatusHub> _hubContext;
    private readonly IPrinterStatusService _printerStatusService;
    private readonly Dictionary<string, PrinterTrackingState> _printerTrackStates = new();

    public PrinterMonitorWorker(IPrinterStatusService printerStatusService,
        IHubContext<PrinterStatusHub> hubContext)
    {
        _printerStatusService = printerStatusService;
        _hubContext = hubContext;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var statusResult = _printerStatusService
                         .GetPrinterStatus()
                         .Where(p => !string.IsNullOrEmpty(p.PrinterName)).Select(p => new
                         {
                             p.PrinterName,
                             p.IsOffline,
                             p.IsBusy,
                             p.Jobs,
                             p.IsTicketPrint,
                             Status = _printerStatusService
                                 .GetPrinterAnalyze(p, GetOrCreateState(p)).ToString()
                         }).ToList();


                await _hubContext.Clients.All.SendAsync("PrinterStatusUpdate", statusResult);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[WORKER ERROR] {ex.Message}");
            }

            await Task.Delay(2000, stoppingToken);
        }
    }

    private PrinterTrackingState GetOrCreateState(PrinterConfig printer)
    {
        var key = printer.PrinterName!;

        if (!_printerTrackStates.TryGetValue(key, out var stats))
        {
            stats = new PrinterTrackingState
            {
                LastJobs = printer.Jobs,
                LastChangeTime = DateTime.UtcNow,

            };

            _printerTrackStates[printer.PrinterName] = stats;
        }
        return stats;
    }
}
