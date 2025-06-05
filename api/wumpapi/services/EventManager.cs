using System.Collections;
using System.ComponentModel;
using wumpapi.game;
using wumpapi.game.events;

namespace wumpapi.services;

public class EventManager : IEventManager
{
    // Apparently this is a bst? hopefully it's faster or something
    private SortedSet<IEvent> events = new(Comparer<IEvent>.Create((event1, event2) => Comparer.Default.Compare(event1.InitiatedAt, event2.InitiatedAt)));
    private Dictionary<Type, List<EventListener<IEvent>>> subscribed = new();
    public List<IEvent> GetEvents(long since)
    {
        return events.GetViewBetween(new DummyEvent(since), new DummyEvent(long.MaxValue)).ToList();
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

internal class DummyEvent : IEvent
{
    public DummyEvent(long initiatedAt)
    {
        InitiatedAt = initiatedAt;
    }

    public string Name { get; }
    public long InitiatedAt { get; }
}