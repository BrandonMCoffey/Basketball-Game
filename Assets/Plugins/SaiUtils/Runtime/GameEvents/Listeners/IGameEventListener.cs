using UnityEngine;

namespace SaiUtils.GameEvents
{
    public interface IGameEventListener<T>
    {
        // This method will be called when the event is raised
        void OnEventRaised(T item);
    }
}
