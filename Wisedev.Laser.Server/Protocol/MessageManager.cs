using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Immutable;
using System.Reflection;
using Wisedev.Laser.Server.Protocol.Attributes;
using Wisedev.Laser.Server.Protocol.Handlers;
using Wisedev.Laser.Titan.Message;

namespace Wisedev.Laser.Server.Protocol;

internal class MessageManager
{
    private static readonly ImmutableDictionary<int, Type> s_handlerServiceTypes;

    static MessageManager()
    {
        var builder = ImmutableDictionary.CreateBuilder<int, Type>();

        IEnumerable<Type> types = Assembly.GetExecutingAssembly().GetTypes()
                                  .Where(t => t.GetCustomAttribute<ServiceNodeAttribute>() != null);

        foreach (Type type in types)
        {
            int serviceNodeType = type.GetCustomAttribute<ServiceNodeAttribute>()!.ServiceNodeType;
            builder.Add(serviceNodeType, type);
        }

        s_handlerServiceTypes = builder.ToImmutable();
    }

    private readonly ILogger _logger;
    private readonly IServiceProvider _serviceProvider;

    public MessageManager(IServiceProvider serviceProvider, ILogger<MessageManager> logger)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public async Task ReceiveMessage(PiranhaMessage message)
    {
        int serviceNodeType = message.GetServiceNodeType();

        if (s_handlerServiceTypes.TryGetValue(serviceNodeType, out Type? handlerType))
        {
            MessageHandlerBase handler = (_serviceProvider.GetRequiredService(handlerType) as MessageHandlerBase)!;

            if (!await handler.HandleMessage(message))
            {
                _logger.LogWarning("Handler for message {type} not implemented in {svcName}", message.GetMessageType(), handler.GetType().Name);
            }
        }
        else
        {
            _logger.LogWarning("Handler for service node type {svcType} is not defined!", serviceNodeType);
        }
    }
}
