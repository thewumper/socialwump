using System.Collections;
using System.ComponentModel;
using wumpapi.game;
using wumpapi.game.events;

namespace wumpapi.services;

public class EventManager : IEventManager
{
    private List<IEvent> events = new List<IEvent>();
    private readonly Dictionary<Type, List<EventListener<IEvent>>> subscribed = new();
    public List<IEvent> GetEvents(long since)
    {
        return events.Where(e => e.InitiatedAt > since).ToList();
    }
    
    public void SendEvent(IEvent @event)
    {
        if (subscribed.ContainsKey(@event.GetType()))
        {
            foreach (EventListener<IEvent> listener in subscribed[@event.GetType()])
            {
                listener(@event);
            }
        }
        events.Add(@event);
        @event.InitiatedAt = events.Count;
    }

    public void Subscribe<T>(EventListener<T> callback) where T : IEvent
    {
        EventListener<IEvent> wrapped = (IEvent @event) => callback((T)@event);
        if (subscribed.ContainsKey(typeof(T)))
        {
            subscribed[typeof(T)].Add(wrapped);
        }
        else
        {
            subscribed.Add(typeof(T), [wrapped]);
        }
    }
}
