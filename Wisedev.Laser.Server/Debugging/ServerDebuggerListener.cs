using Microsoft.Extensions.Logging;
using Wisedev.Laser.Titan.Debug;

namespace Wisedev.Laser.Server.Debugging;

internal class ServerDebuggerListener : IDebuggerListener
{
    private readonly ILogger _logger;
    private readonly ILogger _hudPrintLogger;

    public ServerDebuggerListener(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger("Logic");
        _hudPrintLogger = loggerFactory.CreateLogger("HudPrint");
    }

    public void Error(string log)
    {
        _logger.LogError("{debuggerMessage}", log);
    }

    public void HudPrint(string log)
    {
        _hudPrintLogger.LogInformation("{debuggerMessage}", log);
    }

    public void Print(string log)
    {
        _logger.LogInformation("{debuggerMessage}", log);
    }

    public void Warning(string log)
    {
        _logger.LogWarning("{debuggerMessage}", log);
    }
}
