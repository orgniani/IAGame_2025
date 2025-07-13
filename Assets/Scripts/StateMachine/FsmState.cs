namespace StateMachine
{
    public abstract class FsmState<T>
    {
        protected T _owner;

        public void Initialize (T owner)
        {
            _owner = owner;
            OnInitialize();
        }

        protected abstract void OnInitialize ();
        
        public abstract void Enter ();
        public abstract void Exit ();
        public abstract void Update ();
    }
}