using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 7f;

    public void Move(Vector3 direction)
    {
       
        transform.Translate(direction * speed * Time.deltaTime,Space.World);


        if (direction != Vector3.zero)
        {
            transform.forward = direction;
        }
    }
}