using System.Collections.Immutable;
using System.Reflection;
using Wisedev.Laser.Logic.Message;
using Wisedev.Laser.Titan.Message;

namespace Wisedev.Laser.Logic;

public class LaserMessageFactory : MessageFactory
{
    private readonly ImmutableDictionary<int, Type> s_types;

    public LaserMessageFactory() : base()
    {
        this.s_types = CreateMessageMap();
    }

    public override PiranhaMessage? CreateMessageByType(int messageType)
    {
        return this.s_types.TryGetValue(messageType, out Type? type) ?
            Activator.CreateInstance(type) as PiranhaMessage : null;
    }

    private static ImmutableDictionary<int, Type> CreateMessageMap()
    {
        var builder = ImmutableDictionary.CreateBuilder<int, Type>();

        IEnumerable<Type> types = Assembly.GetExecutingAssembly().GetTypes()
                                  .Where(t => t.GetCustomAttribute<PiranhaMessageAttribute>() != null);

        foreach (var type in types)
        {
            PiranhaMessageAttribute attribute = type.GetCustomAttribute<PiranhaMessageAttribute>()!;

            if (!builder.TryAdd(attribute.MessageType, type))
                throw new Exception($"Piranha message with type {attribute.MessageType} defined twice!");
        }

        return builder.ToImmutable();
    }
}
