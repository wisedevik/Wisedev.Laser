using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Sockets;

namespace Wisedev.Laser.Server.Network.Connection;

internal class ClientConnectionManager : IGatewayEventListener
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger _logger;

    public ClientConnectionManager(IServiceScopeFactory scopeFactory, ILogger<ClientConnectionManager> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public void OnConnect(Socket client)
    {
        _logger.LogInformation("New connection from {endPoint}", client.RemoteEndPoint);
        _ = RunSessionAsync(client);
    }

    private async Task RunSessionAsync(Socket client)
    {
        using IServiceScope scope = _scopeFactory.CreateScope();
        ClientConnection session = scope.ServiceProvider.GetRequiredService<ClientConnection>();
        try
        {
            session.SetClient(client);
            await session.RunAsync();
        }
        catch (OperationCanceledException ex) { }
        catch (Exception ex)
        {
            _logger.LogError("Unhandled exception occurred while processing session, trace:\n{exception}", ex);
        }
        finally
        {
            client.Dispose();
        }
    }
}
