using UnityEngine;

namespace SaiUtils.StateMachine
{    
    public interface IState
    {
        void OnEnter();
        void Update();
        void FixedUpdate();
        void OnExit();

    }

    
}

