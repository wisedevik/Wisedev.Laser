using Microsoft.Extensions.Logging;
using Wisedev.Laser.Logic.Message.Auth;
using Wisedev.Laser.Server.Network.Connection;
using Wisedev.Laser.Server.Protocol.Attributes;

namespace Wisedev.Laser.Server.Protocol.Handlers;

[ServiceNode(1)]
internal class AuthenticationMessageHandler : MessageHandlerBase
{
    private ClientConnection _connection;
    private ILogger _logger;

    public AuthenticationMessageHandler(ClientConnection connection, ILogger<AuthenticationMessageHandler> logger)
    {
        _connection = connection;
        _logger = logger;
    }

    [MessageHandler(10101)]
    public async Task OnLoginMessage(LoginMessage message)
    {
        _logger.LogInformation("test!");
        await Task.CompletedTask;
    }
}
