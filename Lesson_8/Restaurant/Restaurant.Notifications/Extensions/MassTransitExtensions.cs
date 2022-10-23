using MassTransit;
using MassTransit.Audit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Restaurant.Messaging.Configuration;
using Restaurant.Notifications.Consumers;

namespace Restaurant.Notifications.Extensions;

public static class MassTransitExtensions
{
    public static IServiceCollection AddMessageBus(this IServiceCollection services, IConfiguration configuration)
    {
        var rabbitSettings = configuration.GetSection("RabbitSettings").Get<RabbitSettings>();

        services.AddMassTransit(massTransitConfig =>
        {
            massTransitConfig.AddConsumer<NotifyConsumer>();

            massTransitConfig.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(rabbitSettings.Host, rabbitSettings.Port, "/", h =>
                {
                    h.Username(rabbitSettings.UserName);
                    h.Password(rabbitSettings.Password);
                });
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
