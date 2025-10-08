using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SaiUtils.StateMachine
{
    public class StateMachine
    {
        // a StateNode is a Wrapper Class that holds a state and all its transitions
        StateNode _currentStateNode;

        // dictionary of all states in the state machine
        Dictionary<Type, StateNode> _stateNodes = new();

        // transitions that can be taken from any state 
        HashSet<ITransition> _anyTransitions = new(); 


        public void Update() 
        {
            var transition = GetTransition();

            if (transition != null) 
            {

                // if a transition is found, change the state to the target state of the transition
                ChangeState(transition.TargetState); 
            }

            _currentStateNode.State?.Update(); 
        }

        public void FixedUpdate() 
        {
            _currentStateNode.State?.FixedUpdate(); 
        }

        // This method sets a custom state for the state machine assuming that the state is already in the _stateNodes dictionary
        public void SetState(IState state)
        {
            _currentStateNode = _stateNodes[state.GetType()];

            // if it doesn't exist, add it to the dictionary
            if (_currentStateNode == null)
            {
                _currentStateNode = new StateNode(state);
                _stateNodes.Add(state.GetType(), _currentStateNode);
            }

            _currentStateNode.State?.OnEnter(); 
        }

        public void ChangeState(IState state)
        {
            // if the state is the same as the current state, do nothing
            if (state == _currentStateNode.State) return; 

            // store the current state as the previous state and find the next state in the dictionary
            var previousState = _currentStateNode;
            var nextState = _stateNodes[state.GetType()].State;

            // call the OnExit method of the previous state and the OnEnter method of the next state
            previousState.State?.OnExit(); 
            nextState?.OnEnter();

            // set the current state to the next state
            _currentStateNode = _stateNodes[state.GetType()]; 
        }

        IEnumerator ChangeStateWithDelayCoroutine(IState state, float delay)
        {
            // wait for a delay
            yield return new WaitForSeconds(delay);

            // change the state
            ChangeState(state);
        }

        ITransition GetTransition()
        {
            // first look through _anyTransitions to see if any of them are true
            foreach (var transition in _anyTransitions) 
                if (transition.Condition.Evaluate()) 
                    return transition;
                
            // then look through the transitions of the current state to see if any of them are true
            foreach (var transition in _currentStateNode.Transitions) 
                if (transition.Condition.Evaluate()) 
                    return transition;

            // if no transitions are true, return null
            return null;
        }

        public void AddTransition(IState from, IState to, IPredicate condition)
        {
            // add a transition from one state to another with a condition and encapsulate both states in StateNodes
            GetOrAddNode(from).AddTransition(GetOrAddNode(to).State, condition);
        }

        public void AddAnyTransition(IState to, IPredicate condition)
        {
            // add a transition from any state to another with a condition and encapsulate the to state in a StateNode
            _anyTransitions.Add(new Transition(GetOrAddNode(to).State, condition));
        }

        StateNode GetOrAddNode(IState state)
        {
            // if the state is already in the dictionary, return it otherwise return null
            var node = _stateNodes.GetValueOrDefault(state.GetType());

            // if the node is null it means that the state is not in the dictionary so we add it
            if (node == null) 
            {
                node = new StateNode(state);
                _stateNodes.Add(state.GetType(), node);
            }

            // return the node
            return node;
        }


#region StateNode Definition
        class StateNode 
        {
            public IState State { get; }

            // I'm using a HashSet here to avoid adding the same transition twice
            public HashSet<ITransition> Transitions { get; }

            public StateNode(IState state)
            {
                State = state;
                Transitions = new HashSet<ITransition>();
            }

            // this method adds a transition to the state when given a to state and a condition
            public void AddTransition(IState to, IPredicate condition) 
            {
                Transitions.Add(new Transition(to, condition));
            }
        }
#endregion
    
    }
    
}
