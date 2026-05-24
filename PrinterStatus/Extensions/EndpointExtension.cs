using PrinterStatus.Endpoints;
using PrinterStatusSignalR.Hubs;

namespace PrinterStatus.Extensions;

public static class EndpointExtension
{

    private const string PRINTER_HUB_PATH = "/printerhub";

    public static WebApplication MapEndpoints(this WebApplication app)
    {
        // Map API endpoints
        app.MapHealthCheckEndpoint();

        // SignalR Hub endpoints
        app.MapHub<PrinterStatusHub>(PRINTER_HUB_PATH);

        return app;
    }
}
