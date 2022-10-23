using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serilog;
using MassTransit.Audit;
using Restaurant.Messaging.Logging;
using Prometheus;
using Restaurant.Booking;
using Restaurant.Booking.Services;
using Restaurant.Messaging.Configuration;
using Restaurant.Booking.Extensions;
using Restaurant.Booking.Interfaces;
using Restaurant.Booking.Models;
using Restaurant.Messaging.Interfaces;
using Restaurant.Messaging.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, config) =>config.ReadFrom.Configuration(context.Configuration));

builder.Services.AddControllers();

builder.Services.AddSingleton(p => p.GetRequiredService<IOptions<RabbitSettings>>().Value);
builder.Services.AddSingleton<Worker>();
builder.Services.AddSingleton<IInMemoryRepository<BookingRequestModel>, InMemoryRepository<BookingRequestModel>>();
builder.Services.AddSingleton<ITableBookingService, TableBookingService>();
builder.Services.AddSingleton<IMessageAuditStore, AuditStore>();
builder.Services.AddMessageBus(builder.Configuration);

builder.Services.AddHostedService<Worker>();

var app = builder.Build();

app.MapMetrics();
app.MapControllers();

await app.RunAsync();
