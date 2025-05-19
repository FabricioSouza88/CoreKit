namespace CoreKit.EventHandler.Interfaces;

public interface IEventListenerFactory
{
    IEnumerable<IEventListener> GetListeners(string process, string action);
}
