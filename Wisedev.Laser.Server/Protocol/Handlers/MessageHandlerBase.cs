using System.Collections.Immutable;
using System.Reflection;
using Wisedev.Laser.Server.Protocol.Attributes;
using Wisedev.Laser.Titan.Message;

namespace Wisedev.Laser.Server.Protocol.Handlers;

internal class MessageHandlerBase
{
    private readonly ImmutableDictionary<int, MethodInfo> _handlerMethods;

    public MessageHandlerBase()
    {
        var builder = ImmutableDictionary.CreateBuilder<int, MethodInfo>();

        foreach (var method in GetType().GetMethods())
        {
            MessageHandlerAttribute? attribute = method.GetCustomAttribute<MessageHandlerAttribute>();
            if (attribute == null) continue;

            builder.Add(attribute.MessageType, method);
        }

        _handlerMethods = builder.ToImmutable();
    }

    public async Task<bool> HandleMessage(PiranhaMessage message)
    {
        if (_handlerMethods.TryGetValue(message.GetMessageType(), out var method))
        {
            await (Task)method.Invoke(this, new object[] { message })!;
            return true;
        }

        return false;
    }
}
