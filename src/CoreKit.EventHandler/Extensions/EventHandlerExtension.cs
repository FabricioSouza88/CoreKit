using CoreKit.EventHandler.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace CoreKit.EventHandler.Extensions
{
    public static class EventHandlerExtension
    {
        public static void AddEventHandler(this IServiceCollection services)
        {
            services.AddScoped<IEventHandler, EventHandler>();
            services.AddScoped<IEventListenerFactory, EventListenerFactory>();
        }
    }
}
