using UnityEngine;

public class GoalWall : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            LevelManager.Instance.WinGame();
        }
    }
}