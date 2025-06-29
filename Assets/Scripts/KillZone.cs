using UnityEngine;

public class KillZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManagerReset manager = FindFirstObjectByType<GameManagerReset>();
            if (manager != null)
            {
                manager.ResetPlayer();
            }
        }
    }
}
