namespace Wisedev.Laser.Titan.Message;

public abstract class MessageFactory
{
    public MessageFactory()
    {
        ;
    }

    public abstract PiranhaMessage? CreateMessageByType(int messageType);
}