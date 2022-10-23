using System;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Restaurant.Booking;
using Restaurant.Booking.Consumers;
using Restaurant.Booking.DTO;
using Restaurant.Booking.Interfaces;
using Restaurant.Booking.Models;
using Restaurant.Booking.Services;
using Restaurant.Kitchen.Consumers;
using Restaurant.Kitchen.Interfaces;
using Restaurant.Kitchen.Models;
using Restaurant.Kitchen.Services;
using Restaurant.Messaging.Data;
using Restaurant.Messaging.Interfaces;
using Restaurant.Messaging.Repositories;
using Restaurant.Notifications.Consumers;
using Restaurant.Notifications.Interfaces;
using Restaurant.Notifications.Services;

namespace Restaurant.Tests;

public class SagaTests
{
    private ServiceProvider _provider;
    private ITestHarness _harness;

    [OneTimeSetUp]
    public async Task Init()
    {
        _provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg =>
            {
                cfg.AddConsumer<BookingRequestedConsumer>();                
                cfg.AddConsumer<KitchenRequestedConsumer>();                
                cfg.AddConsumer<NotifyConsumer>();
                cfg.AddSagaStateMachine<RestaurantSaga, RestaurantState>();
            })
            .AddLogging()
            .AddSingleton<IInMemoryRepository<BookingRequestModel>, InMemoryRepository<BookingRequestModel>>()
            .AddTransient<ITableBookingService, TableBookingService>()
            .AddSingleton<IInMemoryKeyRepository<KitchenRequestModel>, InMemoryKeyRepository<KitchenRequestModel>>()
            .AddTransient<IKitchenService, KitchenService>()
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
    public async Task Customer_Notify_Booking_Success()
    {
        var orderId = Guid.NewGuid();
        var ClientId = Guid.NewGuid();
        await _harness.Bus.Publish(new BookingRequested()
        {
            OrderId = orderId,
            ClientId = ClientId,
            Dish = MenuRepository.GetDishById(1),
            Seats = 5,
            BookingArrivalTime = 7,
            ActualArrivalTime = 2
        } as IBookingRequested);

        Assert.That(await _harness.Published.Any<IBookingRequested>());
        Assert.That(await _harness.Consumed.Any<IBookingRequested>());

        var sagaHarness = _harness.GetSagaStateMachineHarness<RestaurantSaga, RestaurantState>();
        Assert.That(await sagaHarness.Consumed.Any<IBookingRequested>());
        Assert.That(await sagaHarness.Created.Any(x => x.CorrelationId == orderId));

        var saga = sagaHarness.Created.Contains(orderId);

        Assert.IsNotNull(saga);
        Assert.That(await _harness.Published.Any<ITableBooked>());
        Assert.That(await _harness.Published.Any<IKitchenReady>());
        Assert.That(await _harness.Published.Any<INotify>());

        await _harness.OutputTimeline(TestContext.Out, opt => opt.Now().IncludeAddress());
    }
}