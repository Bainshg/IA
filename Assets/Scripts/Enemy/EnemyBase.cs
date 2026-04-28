using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    [Header("Configuracion de Daño")]
    [SerializeField] private float contactDamage = 10f; 

    private void OnCollisionEnter(Collision collision)
    {
        
        if (collision.gameObject.CompareTag("Player"))
        {
            if (collision.gameObject.TryGetComponent(out IDamageable playerHealth))
            {
                playerHealth.TakeDamage(contactDamage);
            }
        }
    }
}