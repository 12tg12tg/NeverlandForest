using UnityEngine.Events;
using System.Collections.Generic;

public class EventBus<T>
{
    private static readonly IDictionary<T, UnityEvent<object[]>> Events
        = new Dictionary<T, UnityEvent<object[]>>();

    public static void Subscribe(T eventType, UnityAction<object[]> listener)
    {
        UnityEvent<object[]> thisEvent;

        if (Events.TryGetValue(eventType, out thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new UnityEvent<object[]>();
            thisEvent.AddListener(listener);
            Events.Add(eventType, thisEvent);
        }
    }

    public static void Unsubscribe(T type, UnityAction<object[]> listener)
    {
        UnityEvent<object[]> thisEvent;

        if (Events.TryGetValue(type, out thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }

    public static void Publish(T type, params object[] vals)
    {
        UnityEvent<object[]> thisEvent;

        if (Events.TryGetValue(type, out thisEvent))
        {
            thisEvent.Invoke(vals);
        }
    }

    public static void ResetEventBus()
    {
        Events.Clear();
    }
}
