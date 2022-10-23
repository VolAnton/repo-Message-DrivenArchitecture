using System;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Restaurant.Booking.Consumers;
using Restaurant.Booking.DTO;
using Restaurant.Booking.Interfaces;
using Restaurant.Booking.Models;
using Restaurant.Booking.Services;
using Restaurant.Messaging.Data;
using Restaurant.Messaging.Interfaces;
using Restaurant.Messaging.Repositories;

namespace Restaurant.Tests;

public class BookingConsumerTests
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
                cfg.AddConsumer<BookingCancelRequested>();
            })
            .AddLogging()
            .AddSingleton<IInMemoryRepository<BookingRequestModel>, InMemoryRepository<BookingRequestModel>>()
            .AddTransient<ITableBookingService, TableBookingService>()
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
    public async Task Any_Booking_Request_Consumed()
    {
        await _harness.Bus.Publish(new BookingRequested()
        {
            OrderId = Guid.NewGuid(),
            ClientId = Guid.NewGuid(),
            Dish = MenuRepository.GetDishById(1),
            Seats = 5,
            BookingArrivalTime = 7,
            ActualArrivalTime = 2
        } as IBookingRequested);

        Assert.That(await _harness.Consumed.Any<IBookingRequested>());        
    }

    [Test]
    public async Task Booking_Consume_BookingRequestedMessage_Publish_TableBookedMessage()
    {
        var orderId = Guid.NewGuid();

        await _harness.Bus.Publish<IBookingRequested>(new BookingRequested()
        {
            OrderId = orderId,
            ClientId = Guid.NewGuid(),
            Dish = MenuRepository.GetDishById(1),
            Seats = 5,
            BookingArrivalTime = 7,
            ActualArrivalTime = 2
        });

        Assert.That(await _harness.Consumed.Any<IBookingRequested>(item => item.Context.Message.OrderId == orderId), Is.True);
        Assert.That(await _harness.Published.Any<ITableBooked>(item => item.Context.Message.OrderId == orderId), Is.True);        
    }

    [Test]
    public async Task Booking_Consume_BookingCancelRequested()
    {
        var orderId = Guid.NewGuid();

        await _harness.Bus.Publish<IBookingCancelRequested>(new BookingCancel()
        {
            OrderId = orderId
        });

        Assert.That(await _harness.Consumed.Any<IBookingCancelRequested>(item => item.Context.Message.OrderId == orderId), Is.True);        
    }

}