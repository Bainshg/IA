using System.Collections.Generic;

public class StateMachine<T>
{
    private IState _currentState;
    private Dictionary<T, IState> _states = new Dictionary<T, IState>();

    public IState CurrentState => _currentState;

    public void AddState(T key, IState state) => _states.Add(key, state);

    public void SetCurrent(IState state)
    {
        _currentState = state;
        _currentState.Awake();
    }

    public void Update() => _currentState?.Execute();

    public void ChangeState(T key)
    {
        // Solo cambiamos si el estado es diferente al que ya tenemos
        if (_states.TryGetValue(key, out IState newState))
        {
            if (_currentState == newState) return; 

            _currentState?.Sleep();
            _currentState = newState;
            _currentState.Awake();
        }
    }
}