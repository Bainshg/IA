using UnityEngine;

public class PlayerIdleState : PlayerState
{
    public PlayerIdleState(PlayerController controller, StateMachine<PlayerStates> sm) : base(controller, sm) { }

    public override void Awake()
    {
        // Detener el movimiento físico por si acaso
        _controller.Movement.Move(Vector3.zero);
        
        _controller.Combat.SetAttackRange(2.5f);
    }

    public override void Execute()
    {
        // input de ataque
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _controller.Combat.PerformAttack();
        }

        // Si detectamos input del jugador, cambiamos al estado Walk
        if (_controller.CurrentInput.sqrMagnitude > 0.01f)
        {
            _sm.ChangeState(PlayerStates.Walk);
        }
    }
}