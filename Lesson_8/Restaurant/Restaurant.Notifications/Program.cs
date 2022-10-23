using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Restaurant.Notifications.Interfaces;
using Restaurant.Notifications.Extensions;
using Restaurant.Notifications.Services;
using MassTransit.Audit;
using Restaurant.Messaging.Logging;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, config) =>config.ReadFrom.Configuration(context.Configuration));

builder.Services.AddControllers();

builder.Services.AddSingleton<INotifier, Notifier>();
builder.Services.AddSingleton<IMessageAuditStore, AuditStore>();

builder.Services.AddMessageBus(builder.Configuration);

var app = builder.Build();

app.MapMetrics();
app.MapControllers();

await app.RunAsync();
