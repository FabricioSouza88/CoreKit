namespace CoreKit.EventHandler.Interfaces;

public interface IEventListener
{
    Task OnSendEventAsync(dynamic payload, string process, string action);
}