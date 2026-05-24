using PrinterStatusSignalR.Models;

namespace PrinterStatus.Services.IService;

public interface IPrinterStatusService
{
    IEnumerable<PrinterConfig> GetPrinterStatus();

    string GetPrinterAnalyze(PrinterConfig printer, PrinterTrackingState stats);

}
