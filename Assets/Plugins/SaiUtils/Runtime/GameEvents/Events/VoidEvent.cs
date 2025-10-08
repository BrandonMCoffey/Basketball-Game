using UnityEngine;

namespace SaiUtils.GameEvents
{
    [CreateAssetMenu(fileName = "New Void Event", menuName = "SaiUtils/GameEvents/VoidEvent")]
    public class VoidEvent : BaseGameEvent<Void>
    {
        // this is a simple class that inherits from BaseGameEvent and uses the Void struct as the generic parameter
        public void Raise() => Raise(new Void());
    }
}