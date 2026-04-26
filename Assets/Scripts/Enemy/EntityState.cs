public abstract class EntityState : State<EntityStates>
{
    public EntityState(StateMachine<EntityStates> sm) : base(sm) { }
}