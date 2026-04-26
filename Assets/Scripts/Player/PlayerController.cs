using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerMovement _movement;
    private PlayerCombat _combat;

    public bool IsWalking { get; private set; }

    void Awake()
    {
        _movement = GetComponent<PlayerMovement>();
        _combat = GetComponent<PlayerCombat>();
    }

    void Update()
    {
        // Input manual para evitar "fantasmas" de joysticks
        float h = 0;
        float v = 0;

        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) h = 1;
        else if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) h = -1;

        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) v = 1;
        else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) v = -1;

        Vector3 inputDir = new Vector3(h, 0, v);
        if (inputDir.sqrMagnitude > 0.01f)
        {
            IsWalking = true;
            _movement.Move(inputDir.normalized);
        }
        else
        {
            IsWalking = false;
            _movement.Move(Vector3.zero);
        }

        // Ejecutar ataque
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _combat.PerformAttack();
        }
    }
}