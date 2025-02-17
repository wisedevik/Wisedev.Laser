using Wisedev.Laser.Titan.Message;

namespace Wisedev.Laser.Logic.Message.Auth;

[PiranhaMessage(10101)]
public class LoginMessage : PiranhaMessage
{
    public override void Decode()
    {
        base.Decode();
        //TODO: decode login message
    }

    public override short GetMessageType()
    {
        return 10101;
    }

    public override int GetServiceNodeType()
    {
        return 1;
    }
}
