using System.Net.Sockets;

namespace Wisedev.Laser.Server.Network;

internal interface IGatewayEventListener
{
    void OnConnect(Socket client);
}
