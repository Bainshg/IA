using UnityEngine;

public class PatrolBehaviour : MonoBehaviour
{
    [SerializeField] private Transform[] waypoints;
    private int _currentIndex = 0;
    private int _direction = 1;

    public Vector3 GetCurrentWaypoint()
    {
        if (waypoints == null || waypoints.Length == 0) return transform.position;
        return waypoints[_currentIndex].position;
    }

    public void UpdateToNextPoint()
    {
        if (waypoints.Length <= 1) return;

        _currentIndex += _direction;

        // Lógica de sentido inverso (A-B-C-B-A)
        if (_currentIndex >= waypoints.Length)
        {
            _direction = -1;
            _currentIndex = waypoints.Length - 2;
        }
        else if (_currentIndex < 0)
        {
            _direction = 1;
            _currentIndex = 1;
        }
    }

    public bool HasReachedPoint(float threshold = 1.2f)
    {
        return Vector3.Distance(transform.position, GetCurrentWaypoint()) < threshold;
    }
}