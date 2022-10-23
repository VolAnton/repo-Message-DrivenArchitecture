using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using MassTransit.Audit;
using Restaurant.Messaging.Logging;
using Prometheus;
using Restaurant.Messaging.Interfaces;
using Restaurant.Messaging.Repositories;
using Restaurant.Kitchen.Services;
using Restaurant.Kitchen.Interfaces;
using Restaurant.Kitchen.Extensions;
using Restaurant.Kitchen.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, config) =>config.ReadFrom.Configuration(context.Configuration));

builder.Services.AddControllers();

builder.Services.AddSingleton<IKitchenService, KitchenService>();
builder.Services.AddSingleton<IInMemoryKeyRepository<KitchenRequestModel>, InMemoryKeyRepository<KitchenRequestModel>>();
builder.Services.AddSingleton<IMessageAuditStore, AuditStore>();
builder.Services.AddMessageBus(builder.Configuration);

var app = builder.Build();

app.MapMetrics();
app.MapControllers();

await app.RunAsync();
