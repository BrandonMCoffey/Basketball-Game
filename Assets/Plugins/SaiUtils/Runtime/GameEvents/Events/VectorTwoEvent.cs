using UnityEngine;

namespace SaiUtils.GameEvents
{
    [CreateAssetMenu(fileName = "New Vector2 Event", menuName = "SaiUtils/GameEvents/Vector2Event")]
    public class VectorTwoEvent : BaseGameEvent<UnityEngine.Vector2> {}

    // I don't need to add a Raise method here because it is already implemented in the BaseGameEvent class
    // because the T in the BaseGameEvent class is now an int, the Raise method will now automatically take an int as a parameter
}