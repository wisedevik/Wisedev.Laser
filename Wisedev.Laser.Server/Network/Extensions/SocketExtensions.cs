using System.Net.Sockets;

namespace Wisedev.Laser.Server.Network.Extension;

internal static class SocketExtensions
{
    public static async ValueTask<Socket?> AcceptSocketAsync(this Socket socket, CancellationToken ct)
    {
        try
        {
            return await socket.AcceptAsync(ct);
        }
        catch (OperationCanceledException)
        {
            return null;
        }
    }
}
