using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Transform[] points; // Waypoints to move between
    public float speed = 2f;
    public int startPointIndex = 0;

    private int currentTargetIndex;

    void Start()
    {
        if (points.Length == 0) return;
        currentTargetIndex = startPointIndex % points.Length;
    }

    void Update()
    {
        if (points.Length == 0) return;

        Transform target = points[currentTargetIndex];
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            currentTargetIndex = (currentTargetIndex + 1) % points.Length;
        }
    }
}
