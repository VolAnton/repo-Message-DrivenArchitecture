using System;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Restaurant.Booking.DTO;
using Restaurant.Kitchen.Consumers;
using Restaurant.Kitchen.Interfaces;
using Restaurant.Kitchen.Models;
using Restaurant.Kitchen.Services;
using Restaurant.Messaging.Data;
using Restaurant.Messaging.Interfaces;
using Restaurant.Messaging.Repositories;

namespace Restaurant.Tests;

public class KitchenConsumerTests
{
    private ServiceProvider _provider;
    private ITestHarness _harness;

    [OneTimeSetUp]
    public async Task Init()
    {
        _provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg =>
            {
                cfg.AddConsumer<KitchenRequestedConsumer>();
                cfg.AddConsumer<KitchenCancelRequested>();
            })
            .AddLogging()
            .AddTransient<IKitchenService, KitchenService>()
            .AddSingleton<IInMemoryKeyRepository<KitchenRequestModel>, InMemoryKeyRepository<KitchenRequestModel>>()
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
    public async Task Any_Kitchen_Request_Consumed()
    {
        await _harness.Bus.Publish<ITableBooked>(new TableBooked()
        {
            OrderId = Guid.NewGuid(),
            ClientId = Guid.NewGuid(),
            Dish = MenuRepository.GetDishById(1)
        });

        Assert.That(await _harness.Consumed.Any<ITableBooked>());
    }

    [Test]
    public async Task Kitchen_Consume_TableBookedMessage_Publish_KitchenReadyMessage()
    {
        var orderId = Guid.NewGuid();

        await _harness.Bus.Publish<ITableBooked>(new TableBooked()
        {
            OrderId = orderId,
            ClientId = Guid.NewGuid(),
            Dish = MenuRepository.GetDishById(1)
        });

        Assert.That(await _harness.Consumed.Any<ITableBooked>(item => item.Context.Message.OrderId == orderId), Is.True);
        Assert.That(await _harness.Published.Any<IKitchenReady>(item => item.Context.Message.OrderId == orderId), Is.True);        
    }

    [Test]
    public async Task Booking_Consume_KitchenCancelRequested()
    {
        var orderId = Guid.NewGuid();

        await _harness.Bus.Publish<IKitchenCancelRequested>(new KitchenCancel()
        {
            OrderId = orderId
        });

        Assert.That(await _harness.Consumed.Any<IKitchenCancelRequested>(item => item.Context.Message.OrderId == orderId), Is.True);        
    }
}