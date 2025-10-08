using UnityEngine;
using UnityEngine.Events;

namespace SaiUtils.GameEvents
{
    public class TransformListener : BaseGameEventListener<Transform, TransformEvent, UnityEvent<Transform>> {}
    
}