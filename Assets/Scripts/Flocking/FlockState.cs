// base de los estados del flock. mismo patron que EntityState pero apoyado en
// FlockAgent en vez de EnemyAI.
public abstract class FlockState : State<FlockStates>
{
    protected readonly FlockAgent _agent;

    public FlockState(FlockAgent agent, StateMachine<FlockStates> sm) : base(sm) => _agent = agent;
}
