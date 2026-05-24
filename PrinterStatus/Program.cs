using PrinterStatus.Extensions;
using PrinterStatus.Services.IService;
using PrinterStatusSignalR.Model;
using PrinterStatusSignalR.PrinterMonitorWorker;
using PrinterStatusSignalR.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSingleton<IPrinterStatusService, PrinterStatusService>();
builder.Services.AddHostedService<PrinterMonitorWorker>();


builder.Services.Configure<Printers>(builder
    .Configuration.GetSection("Printers"));

builder.Services.AddSignalR(option =>
{
    option.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
    option.KeepAliveInterval = TimeSpan.FromSeconds(15);
    option.HandshakeTimeout = TimeSpan.FromSeconds(15);
});

var app = builder.Build();

app.UseHttpsRedirection();

// Map Endpoints
app.MapEndpoints();

app.Run();

