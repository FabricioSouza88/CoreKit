using CoreKit.EventHandler.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace CoreKit.EventHandler;

public class EventHandler : IEventHandler
{
    private readonly IEventListenerFactory _listenerFactory;
    private readonly ILogger<EventHandler> _logger;

    public EventHandler(
        IEventListenerFactory listenerFactory,
        ILogger<EventHandler> logger)
    {
        _listenerFactory = listenerFactory;
        _logger = logger;
    }


    public void PublishEvent(dynamic payload, string process, string action)
    {
        NotifyListeners(payload, process, action);
    }

    void NotifyListeners(dynamic payload, string process, string action)
    {
        try
        {
            var listeners = _listenerFactory.GetListeners(process, action);
            var tasks = new List<Task>();

            foreach (var listener in listeners)
            {
                tasks.Add(listener.OnSendEventAsync(payload, process, action));
            }

            Task.WhenAll(tasks);
        }
        catch (AggregateException ex)
        {
            string jsonEntity = JsonSerializer.Serialize(payload);
            _logger.LogError(ex.Message, ex);
        }
        catch (Exception ex)
        {
            string jsonEntity = JsonSerializer.Serialize(payload);
            _logger.LogError(ex.Message, ex);
        }
    }
}