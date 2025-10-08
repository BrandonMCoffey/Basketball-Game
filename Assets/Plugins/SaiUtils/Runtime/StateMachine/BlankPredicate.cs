using UnityEngine;

namespace SaiUtils.StateMachine
{
    public class BlankPredicate : IPredicate
    {
        public bool Evaluate()
        {
            return false;
        }
    }
}

