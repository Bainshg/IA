using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerMovement _movement;
    private PlayerCombat _combat;

    //public bool IsWalking { get; private set; }
    private StateMachine<PlayerStates> _sm;
    // los PlayerState acceden a estas variables
    public Vector3 CurrentInput { get; private set; }
    public PlayerMovement Movement => _movement;
    public PlayerCombat Combat => _combat;

    void Awake()
    {
        _movement = GetComponent<PlayerMovement>();
        _combat = GetComponent<PlayerCombat>();

        //inicializamos la FSM del player
        _sm = new StateMachine<PlayerStates>();
        _sm.AddState(PlayerStates.Idle, new PlayerIdleState(this, _sm));
        _sm.AddState(PlayerStates.Walk, new PlayerWalkState(this, _sm));

        // arrancamos en Idle
        _sm.SetCurrent(new PlayerIdleState(this, _sm));
    }

    void Update()
    {
        // Lectura de Input (Flechas y WASD)
        float h = 0;
        float v = 0;

        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) h = 1;
        else if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) h = -1;

        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) v = 1;
        else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) v = -1;

        // Guardamos el input normalizado para los estados
        CurrentInput = new Vector3(h, 0, v).normalized;

        // Actualizamos la FSM
        _sm.Update();
    }
}