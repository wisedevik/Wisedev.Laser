namespace Wisedev.Laser.Logic.Message;

[AttributeUsage(AttributeTargets.Class)]
public class PiranhaMessageAttribute(int messageType) : Attribute
{
    public int MessageType { get; } = messageType;
}
