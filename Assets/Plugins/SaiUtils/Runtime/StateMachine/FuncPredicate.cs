using System;
using UnityEngine;

namespace SaiUtils.StateMachine
{
    public class FuncPredicate : IPredicate
    {
        readonly Func<bool> _func; // I'm using a Func since I want to be able to pass in any function that returns a bool
        
        public FuncPredicate(Func<bool> func)
        {
            _func = func;
        }
        
        public bool Evaluate() => _func.Invoke();
    }
}
