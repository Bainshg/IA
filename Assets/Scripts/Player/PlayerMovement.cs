using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 7f;
    private Rigidbody rb;

    public Vector3 Velocity { get; private set; }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    public void Move(Vector3 direction)
    {
        Velocity = direction * speed;

        // Cambiado de linearVelocity a velocity para compatibilidad
        rb.velocity = new Vector3(Velocity.x, rb.velocity.y, Velocity.z);

        if (direction != Vector3.zero)
        {
            transform.forward = direction;
        }
    }
}