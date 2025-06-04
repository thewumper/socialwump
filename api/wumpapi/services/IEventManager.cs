using wumpapi.game.events;

namespace wumpapi.services;

public interface IEventManager
{
    List<IEvent> GetEvents(long since);
    void SendEvent(IEvent @event);
    void Subscribe<T>(EventListener<T> callback) where T : IEvent; 
}
public delegate void EventListener<in T>(T @event) where T : IEvent;
