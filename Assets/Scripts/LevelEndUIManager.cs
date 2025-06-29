using UnityEngine;
using UnityEngine.EventSystems;

public class LevelEndUIManager : MonoBehaviour
{
    void Start()
    {
        // Ensure mouse is visible and usable
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Ensure time scale is normal
        Time.timeScale = 1f;

        // Ensure EventSystem exists and is enabled
        EventSystem eventSystem = EventSystem.current;
        if (eventSystem == null)
        {
            Debug.LogWarning("No EventSystem found in scene!");
        }
        else if (!eventSystem.enabled)
        {
            eventSystem.enabled = true;
            Debug.Log("Re-enabled disabled EventSystem.");
        }

        // Optional: Debug to see if UI is interactable
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogWarning("No Canvas found in scene.");
        }
    }
}
