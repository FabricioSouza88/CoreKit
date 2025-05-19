using CoreKit.EventHandler.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace CoreKit.EventHandler.Extensions;

public static class EventListenerRegistrationExtensions
{
    public static IServiceCollection AddEventListener<TListener>(
        this IServiceCollection services,
        Action<EventListenerOptions> configure)
        where TListener : class, IEventListener
    {
        services.AddTransient<TListener>();

        services.AddSingleton<IEventListenerRegistration>(sp =>
        {
            var listener = sp.GetRequiredService<TListener>();
            return new EventListenerRegistration<TListener>(listener, configure);
        });

        return services;
    }
}
