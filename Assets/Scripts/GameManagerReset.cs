using UnityEngine;

public class GameManagerReset : MonoBehaviour
{
    [Header("Player Setup")]
    public Transform player;
    public Transform spawnPoint;

    [Header("Kill Zone Setup")]
    public string obstacleTag = "KillZone";

    void OnTriggerEnter(Collider other)
    {
        // Only react to the player entering the kill zone
        if (other.CompareTag(obstacleTag) && other.transform == player)
        {
            ResetPlayer();
        }
    }

    public void ResetPlayer()
    {
        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        player.position = spawnPoint.position;
    }
}
