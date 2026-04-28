using UnityEngine;

public class PlayerWalkState : PlayerState
{
    public PlayerWalkState(PlayerController controller, StateMachine<PlayerStates> sm) : base(controller, sm) { }

    public override void Awake()
    {
        // Cuando se mueve, el rango de ataque es menor
        _controller.Combat.SetAttackRange(2.0f);
    }

    public override void Execute()
    {
        // input de ataque 
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _controller.Combat.PerformAttack();
        }
        
        // le pasamos el input al componente de movimiento
        _controller.Movement.Move(_controller.CurrentInput);

        // si no hay input de movimiento, volvemos a Idle
        if (_controller.CurrentInput.sqrMagnitude <= 0.01f)
        {
            _sm.ChangeState(PlayerStates.Idle);
        }
    }
}