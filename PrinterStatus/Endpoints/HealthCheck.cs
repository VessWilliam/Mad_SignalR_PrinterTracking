using PrinterStatus.Model;

namespace PrinterStatus.Endpoints;

public static class HealthCheck
{
    public static void MapHealthCheckEndpoint(this WebApplication app)
    {
        app.MapGet("/health", () =>
        {
            return Results.Ok(new HealthCheckResponse("Running", DateTime.UtcNow));
        });
    }

}
