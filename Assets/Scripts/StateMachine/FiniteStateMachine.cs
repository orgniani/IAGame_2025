using UnityEngine.Events;
using Helpers;
using UnityEngine;

namespace StateMachine
{
    public class FiniteStateMachine<T>
    {
        private DoubleEntryTable<FsmState<T>, UnityEvent, FsmState<T>> fsmTable;
        private FsmState<T> _currentState;

        public FiniteStateMachine (FsmState<T>[] states, UnityEvent[] transitionEvents, FsmState<T> entryState)
        {
            fsmTable = new DoubleEntryTable<FsmState<T>, UnityEvent, FsmState<T>>(states, transitionEvents);
            _currentState = entryState;

            _currentState.Enter();
        }

        private void OnTriggerTransition (UnityEvent transitionEvent)
        {
            FsmState<T> targetState = fsmTable[_currentState, transitionEvent];

            if (targetState != null)
            {
                Debug.Log($"[FSM] Transitioning from {_currentState.GetType().Name} to {targetState.GetType().Name}");

                _currentState.Exit();
                targetState.Enter();

                _currentState = targetState;
            }
        }

        public void ConfigureTransition (FsmState<T> sourceState, FsmState<T> targetState, UnityEvent transitionEvent)
        {
            fsmTable[sourceState, transitionEvent] = targetState;
            transitionEvent.AddListener(() => OnTriggerTransition(transitionEvent));
        }

        public void Update () => _currentState.Update();
    }
}