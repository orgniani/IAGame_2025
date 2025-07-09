using UnityEngine.Events;
using Helpers;
using UnityEngine;

namespace StateMachine
{
    public class FiniteStateMachine<T>
    {
        private DoubleEntryTable<FsmState<T>, UnityEvent, FsmState<T>> fsmTable;
        private FsmState<T> currentState;


        public FiniteStateMachine (FsmState<T>[] states, UnityEvent[] transitionEvents, FsmState<T> entryState)
        {
            fsmTable = new DoubleEntryTable<FsmState<T>, UnityEvent, FsmState<T>>(states, transitionEvents);
            currentState = entryState;

            currentState.Enter();
        }

        private void OnTriggerTransition (UnityEvent transitionEvent)
        {
            FsmState<T> targetState = fsmTable[currentState, transitionEvent];

            if (targetState != null)
            {
                Debug.Log($"[FSM] Transitioning from {currentState.GetType().Name} to {targetState.GetType().Name}");

                currentState.Exit();
                targetState.Enter();

                currentState = targetState;
            }
        }

        public void ConfigureTransition (FsmState<T> sourceState, FsmState<T> targetState, UnityEvent transitionEvent)
        {
            fsmTable[sourceState, transitionEvent] = targetState;
            transitionEvent.AddListener(() => OnTriggerTransition(transitionEvent));
        }

        public void Update () => currentState.Update();
    }
}