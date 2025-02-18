using Microsoft.Extensions.Hosting;
using Wisedev.Laser.Server.Network;
using Wisedev.Laser.Server.Resources;
using Wisedev.Laser.Titan.Debug;
using Wisedev.Laser.Logic.Data.Laser.Objects;
using Wisedev.Laser.Logic.Data.Laser;

namespace Wisedev.Laser.Server;

internal class LaserServer : IHostedService
{
    private readonly IServerGateway _gateway;

    public LaserServer(IServerGateway gateway, IDebuggerListener debuggerListener)
    {
        _gateway = gateway;


        Debugger.SetListener(debuggerListener);
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _gateway.Start();

        ResourceManager.Init();

        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _gateway.ShutdownAsync();
    }
}
