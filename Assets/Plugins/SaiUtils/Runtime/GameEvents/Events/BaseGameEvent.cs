using System.Collections.Generic;
using UnityEngine;

namespace SaiUtils.GameEvents
{
    public abstract class BaseGameEvent<T> : ScriptableObject
    {
        // List of listeners that have the IGameEventListener interface that this event will notify if it is raised
        private readonly List<IGameEventListener<T>> eventListeners = new List<IGameEventListener<T>>();

        // Raise the event
        public void Raise(T item)
        {
            for (int i = eventListeners.Count - 1; i >= 0; i--)
            {
                eventListeners[i].OnEventRaised(item); // Call the OnEventRaised method on the listener and pass the item
            }
        }

        public void RegisterListener(IGameEventListener<T> listener)
        {
            // If the listener is not in the list, add it
            if (!eventListeners.Contains(listener))
                eventListeners.Add(listener);
        }

        public void UnregisterListener(IGameEventListener<T> listener)
        {
            // If the listener is in the list, remove it
            if (eventListeners.Contains(listener))
                eventListeners.Remove(listener);
        }
    }
}