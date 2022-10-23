using MassTransit;
using MassTransit.Audit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Restaurant.Kitchen.Consumers;
using Restaurant.Messaging.Configuration;
using Restaurant.Messaging.Exceptions;

namespace Restaurant.Kitchen.Extensions;

public static class MassTransitExtensions
{
    public static IServiceCollection AddMessageBus(this IServiceCollection services, IConfiguration configuration)
    {
        var rabbitSettings = configuration.GetSection("RabbitSettings").Get<RabbitSettings>();

        services.AddMassTransit(massTransitConfig =>
        {
            massTransitConfig.AddDelayedMessageScheduler();

            massTransitConfig.AddConsumer<KitchenRequestedConsumer>(configuration =>
            {
                configuration.UseScheduledRedelivery(r =>
                    r.Intervals(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(7), TimeSpan.FromSeconds(9)));

                configuration.UseMessageRetry(r =>
                {
                    r.Incremental(3, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2));
                    r.Handle<KitchenException>();
                });
            });

            massTransitConfig.AddConsumer<KitchenCancelRequested>();
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
