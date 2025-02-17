namespace Wisedev.Laser.Server.Network;

internal interface IServerGateway
{
    void Start();
    Task ShutdownAsync();
}
