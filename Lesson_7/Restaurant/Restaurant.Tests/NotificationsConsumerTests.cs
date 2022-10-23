using System;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Restaurant.Booking.DTO;
using Restaurant.Messaging.Data;
using Restaurant.Messaging.Interfaces;
using Restaurant.Notifications.Consumers;
using Restaurant.Notifications.Interfaces;
using Restaurant.Notifications.Services;

namespace Restaurant.Tests;

public class NotificationsConsumerTests
{
    private ServiceProvider _provider;
    private ITestHarness _harness;

    [OneTimeSetUp]
    public async Task Init()
    {
        _provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg =>
            {
                cfg.AddConsumer<NotifyConsumer>();
            })
            .AddLogging()
            .AddTransient<INotifier, Notifier>()
            .BuildServiceProvider();

        _harness = _provider.GetTestHarness();

        await _harness.Start();
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
        await _provider.DisposeAsync();
    }

    [Test]
    public async Task Any_Notificatios_Request_Consumed()
    {
        await _harness.Bus.Publish<INotify>(new Notify()
        {
            OrderId = Guid.NewGuid(),
            ClientId = Guid.NewGuid(),
            Message = "OK"
        });

        Assert.That(await _harness.Consumed.Any<INotify>());
    }

    [Test]
    public async Task Notificatios_Consume_NotifyMessage()
    {
        var orderId = Guid.NewGuid();

        await _harness.Bus.Publish<INotify>(new Notify()
        {
            OrderId = orderId,
            ClientId = Guid.NewGuid(),
            Message = "OK"
        });

        Assert.That(await _harness.Consumed.Any<INotify>(item => item.Context.Message.OrderId == orderId), Is.True);        
    }
}