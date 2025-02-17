using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Wisedev.Laser.Server;
using Wisedev.Laser.Server.Network;
using Wisedev.Laser.Server.Network.Connection;
using Wisedev.Laser.Server.Network.TCP;
using Wisedev.Laser.Server.Options;
using Wisedev.Laser.Server.Protocol;
using Wisedev.Laser.Titan.Message;
using Wisedev.Laser.Logic;
using Wisedev.Laser.Server.Protocol.Extensions;
using Wisedev.Laser.Titan.Debug;
using Wisedev.Laser.Server.Debugging;

var builder = new HostApplicationBuilder(args);

builder.Services.Configure<GatewayOptions>(builder.Configuration.GetSection("Gateway"));
builder.Services.Configure<EnvironmentOptions>(builder.Configuration.GetSection("Environment"));

builder.Services.AddHandlers();
builder.Services.AddSingleton<IDebuggerListener, ServerDebuggerListener>();
builder.Services.AddSingleton<IServerGateway, TCPGateway>();
builder.Services.AddSingleton<IGatewayEventListener, ClientConnectionManager>();
builder.Services.AddSingleton<MessageFactory, LaserMessageFactory>();
builder.Services.AddScoped<ClientConnection>();
builder.Services.AddScoped<IConnectionListener, Messaging>();
builder.Services.AddScoped<MessageManager>();

builder.Services.AddScoped<ClientConnection>();

builder.Services.AddHostedService<LaserServer>();

IHost host = builder.Build();

IOptions<EnvironmentOptions> envOptions = host.Services.GetRequiredService<IOptions<EnvironmentOptions>>();

Console.Title = $"laser server | {envOptions.Value.Environment}";

ILogger logger = host.Services.GetRequiredService<ILogger<Program>>();

host.Services.GetRequiredService<IApplicationLifetime>().ApplicationStarted.Register(() =>
{
    logger.LogInformation("Server started! Have a nice game!");
});

await host.RunAsync();
