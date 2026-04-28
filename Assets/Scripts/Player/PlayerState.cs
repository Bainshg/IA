using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerState : State<PlayerStates>
{
    protected PlayerController _controller;

    // Pasamos el controlador por constructor para que todos los estados tengan acceso
    public PlayerState(PlayerController controller, StateMachine<PlayerStates> sm) : base(sm)
    {
        _controller = controller;
    }
}