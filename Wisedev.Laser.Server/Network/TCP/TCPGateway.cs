
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Sockets;
using Wisedev.Laser.Server.Network.Extension;
using Wisedev.Laser.Server.Options;

namespace Wisedev.Laser.Server.Network.TCP;

internal class TCPGateway : IServerGateway
{
    private const int BACKLOG = 100;

    private readonly Socket _socket;
    private readonly CancellationTokenSource _cts;
    private readonly ILogger _logger;
    private readonly IOptions<GatewayOptions> _options;
    private readonly IGatewayEventListener _gatewayEventListener;

    private Task _listenTask;

    public TCPGateway(ILogger<TCPGateway> logger, IOptions<GatewayOptions> options, IGatewayEventListener gatewayEventListener)
    {
        _logger = logger;
        _options = options;

        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        _cts = new CancellationTokenSource();
        _gatewayEventListener = gatewayEventListener;
    }

    public void Start()
    {
        _socket.Bind(_options.Value.IPEndPoint);
        _socket.Listen(BACKLOG);

        _logger.LogInformation("Server is listening on {ip}", _options.Value.IPEndPoint);

        _listenTask = HandleClientAsync(_cts.Token);
    }

    private async Task HandleClientAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            Socket? client = await _socket.AcceptSocketAsync(token);
            if (client == null) break;

            _gatewayEventListener.OnConnect(client);
        }
    }

    public async Task ShutdownAsync()
    {
        if (_gatewayEventListener != null)
        {
            await _cts.CancelAsync();
            await _listenTask!;
        }

        _socket.Close();
    }
}
