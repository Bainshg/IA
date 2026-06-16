using UnityEngine;

public class SteeringAgent : MonoBehaviour
{
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float maxForce = 10f;
    [SerializeField] private float mass = 1f;
    private Vector3 _velocity;

    public void ApplySteering(Vector3 steeringForce)
    {
        steeringForce.y = 0f;
        steeringForce = Vector3.ClampMagnitude(steeringForce, maxForce);
        Vector3 acceleration = steeringForce / mass;

        _velocity += acceleration * Time.deltaTime;
        _velocity.y = 0f;
        _velocity = Vector3.ClampMagnitude(_velocity, maxSpeed);

        MoveWithWallSlide();

        if (_velocity.sqrMagnitude > 0.1f)
            transform.forward = _velocity.normalized;
    }

    // choca con una pared? resbala a lo largo en vez de rebotar o frenar en seco.
    // antes habia un rebote (Vector3.Reflect) que empujaba lejos de la pared y peleaba
    // con el Seek al waypoint, mas un freno duro (velocity=0) que dejaba a los NPC
    // trabados en las esquinas. el slide por ejes saca los dos.
    private void MoveWithWallSlide()
    {
        Vector3 delta = _velocity * Time.deltaTime;
        Vector3 pos = transform.position;

        // sin grid no hay como chequear caminable -> me muevo directo
        if (GridManager.Instance == null)
        {
            transform.position = pos + delta;
            return;
        }

        // paso completo si el destino es caminable
        if (IsWalkable(pos + delta))
        {
            transform.position = pos + delta;
            return;
        }

        // si no, me muevo por un solo eje (el que siga caminable) y resbalo por la pared
        Vector3 slideX = pos + new Vector3(delta.x, 0f, 0f);
        Vector3 slideZ = pos + new Vector3(0f, 0f, delta.z);

        if (IsWalkable(slideX))
        {
            transform.position = slideX;
            _velocity.z = 0f;
        }
        else if (IsWalkable(slideZ))
        {
            transform.position = slideZ;
            _velocity.x = 0f;
        }
        else
        {
            // esquina sin salida real: aca si freno
            _velocity = Vector3.zero;
        }
    }

    private bool IsWalkable(Vector3 worldPos)
    {
        Node node = GridManager.Instance.NodeFromWorldPoint(worldPos);
        return node != null && node.walkable;
    }
    public void StopAgent()
    {
        _velocity = Vector3.zero;
    }
    public Vector3 Velocity => _velocity;
    public float MaxSpeed => maxSpeed;
}