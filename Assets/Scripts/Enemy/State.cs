public abstract class State<T> : IState
{
    protected StateMachine<T> _sm;
    public State(StateMachine<T> sm) => _sm = sm;

    public virtual void Awake() { }
    public virtual void Execute() { }
    public virtual void Sleep() { }
}