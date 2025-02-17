using Microsoft.Extensions.Logging;
using System.Net.Sockets;
using System.Text;
using Wisedev.Laser.Server.Protocol;
using Wisedev.Laser.Titan.Message;

namespace Wisedev.Laser.Server.Network.Connection;

internal class ClientConnection
{
    const int RECV_BUFFER_SIZE = 1024 * 4;
    const int RECV_TIMEOUT = 30000;

    private readonly IConnectionListener _listener;
    private readonly MessageManager _messageManager;

    private byte[] _receiveBuffer;
    private Socket _client;

    private ILogger _logger;

    public Socket Client
    {
        get
        {
            return _client ?? throw new InvalidOperationException("Trying to get _client when it's NULL");
        }
    }

    public ClientConnection(IConnectionListener listener, MessageManager messageManager, ILogger<ClientConnection> logger)
    {
        _messageManager = messageManager;
        _logger = logger;

        _listener = listener;
        _listener.OnSend = SendAsync;
        _listener.RecvCallback = OnMessage;

        _receiveBuffer = GC.AllocateUninitializedArray<byte>(RECV_BUFFER_SIZE);
    }

    public void SetClient(Socket client)
    {
        _client = client;
    }

    public async Task RunAsync()
    {
        int recvIdx = 0;
        Memory<byte> recvBufferMem = _receiveBuffer.AsMemory();

        while (true)
        {
            int r = await ReceiveAsync(recvBufferMem[recvIdx..], RECV_TIMEOUT);
            if (r == 0) break;

            recvIdx += r;
            int consumedBytes = await _listener.OnReceive(recvBufferMem, recvIdx);
            if (consumedBytes > 0)
            {
                Buffer.BlockCopy(_receiveBuffer, consumedBytes, _receiveBuffer, 0, recvIdx -= consumedBytes);
            }
            else if (consumedBytes < 0) break;
        }
    }

    public async Task SendMessage(PiranhaMessage message)
    {
        await _listener.Send(message);
    }
    private async Task OnMessage(PiranhaMessage message)
    {
        await _messageManager.ReceiveMessage(message);
    }

    private async ValueTask SendAsync(Memory<byte> buffer)
    {
        await Client.SendAsync(buffer, default);
    }

    private async ValueTask<int> ReceiveAsync(Memory<byte> buffer, int timeout)
    {
        CancellationTokenSource cts = new(TimeSpan.FromMilliseconds(timeout));
        return await Client.ReceiveAsync(buffer, cts.Token);

    }
}
