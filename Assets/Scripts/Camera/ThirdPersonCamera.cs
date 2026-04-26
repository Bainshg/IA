using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Configuración de Seguimiento")]
    [SerializeField] private Transform target; // Arrastrá al Player acá
    [SerializeField] private Vector3 offset = new Vector3(0, 5, -7); // Distancia cámara-jugador
    [SerializeField] private float smoothSpeed = 0.125f; // Suavizado (Lerp)

    [Header("Configuración de Rotación")]
    [SerializeField] private bool lookAtTarget = true;

    void LateUpdate()
    {
        if (target == null) return;

        // Calculamos la posición deseada sumando el offset a la posición del player
        Vector3 desiredPosition = target.position + offset;

        // Interpolación lineal para que el movimiento sea fluido y no seco
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        transform.position = smoothedPosition;

        if (lookAtTarget)
        {
            // Hace que la cámara siempre apunte al centro del jugador
            transform.LookAt(target);
        }
    }
}