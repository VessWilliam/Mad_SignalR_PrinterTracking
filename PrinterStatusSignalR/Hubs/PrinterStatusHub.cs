using Microsoft.AspNetCore.SignalR;

namespace PrinterStatusSignalR.Hubs;

/// <summary>
/// Acts as a real-time broadcast channel for printer status updates.
/// This hub is server-driven (no client-to-server methods required).
/// </summary>
public class PrinterStatusHub : Hub { }
