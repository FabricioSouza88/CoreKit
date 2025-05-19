using CoreKit.EventHandler.Interfaces;

namespace CoreKit.EventHandler;

public class EventListenerFactory : IEventListenerFactory
{
    private readonly IEnumerable<IEventListenerRegistration> _registrations;

    public EventListenerFactory(IEnumerable<IEventListenerRegistration> registrations)
    {
        _registrations = registrations;
    }

    public IEnumerable<IEventListener> GetListeners(string process, string action)
    {
        return _registrations.Where(r =>
                (r.Options.Processes.Any(e => e.Equals(process, StringComparison.OrdinalIgnoreCase)) || !r.Options.EventTypes.Any()) &&
                (r.Options.EventTypes.Any(p => p.Equals(action, StringComparison.OrdinalIgnoreCase)) || !r.Options.Processes.Any()))
            .Select(r => r.Listener);
    }
}
