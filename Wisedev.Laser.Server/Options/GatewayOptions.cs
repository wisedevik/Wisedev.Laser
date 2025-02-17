using System.Net;

namespace Wisedev.Laser.Server.Options;

internal record GatewayOptions
{
    public string Host { get; set; }
    public int Port { get; set; }

    public IPEndPoint IPEndPoint => new IPEndPoint(IPAddress.Parse(Host), Port);

    public override string ToString()
    {
        return $"{Host}:{Port}";
    }
}
