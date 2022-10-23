using MassTransit;
using MassTransit.Audit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Restaurant.Booking.Consumers;
using Restaurant.Messaging.Configuration;

namespace Restaurant.Booking.Extensions;

public static class MassTransitExtensions
{
    public static IServiceCollection AddMessageBus(this IServiceCollection services, IConfiguration configuration)
    {
        var rabbitSettings = configuration.GetSection("RabbitSettings").Get<RabbitSettings>();

        services.AddMassTransit(massTransitConfig =>
        {
            massTransitConfig.AddConsumer<BookingRequestedConsumer>();
            massTransitConfig.AddConsumer<BookingCancelRequested>();
            massTransitConfig.AddSagaStateMachine<RestaurantSaga, RestaurantState>().
                InMemoryRepository();

            massTransitConfig.AddDelayedMessageScheduler();
            massTransitConfig.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(rabbitSettings.Host, rabbitSettings.Port, "/", h =>
                {
                    h.Username(rabbitSettings.UserName);
                    h.Password(rabbitSettings.Password);
                });
                cfg.UseDelayedMessageScheduler();
                cfg.UseInMemoryOutbox();
                cfg.ConfigureEndpoints(context);

                var provider = services.BuildServiceProvider();
                var auditStore = provider.GetService<IMessageAuditStore>();

                cfg.ConnectSendAuditObservers(auditStore);
                cfg.ConnectConsumeAuditObserver(auditStore);
            });
        });

        services.AddOptions<MassTransitHostOptions>().Configure(options =>
        {
            options.WaitUntilStarted = true;
        });

        return services;
    }
}
