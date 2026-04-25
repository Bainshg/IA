using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 7f;

    // Propiedad pública: el enemigo puede LEERLA, pero solo este script puede MODIFICARLA
    public Vector3 Velocity { get; private set; }

    public void Move(Vector3 direction)
    {
        // Calculamos la velocidad actual (dirección * magnitud)
        // Si no hay input, la velocidad será Vector3.zero
        Velocity = direction * speed;

        // Movimiento en espacio del mundo
        transform.Translate(Velocity * Time.deltaTime, Space.World);

        if (direction != Vector3.zero)
        {
            transform.forward = direction;
        }
    }
}