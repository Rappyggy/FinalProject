using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    public TextMeshProUGUI timerText;

    private float timer = 0f;
    private bool isRunning = true;

    public static float finalTime; // <-- Add this

    void Update()
    {
        if (!isRunning) return;

        timer += Time.deltaTime;

        int hours = Mathf.FloorToInt(timer / 3600f);
        int minutes = Mathf.FloorToInt((timer % 3600f) / 60f);
        int seconds = Mathf.FloorToInt(timer % 60f);
        int milliseconds = Mathf.FloorToInt((timer * 1000f) % 1000f);

        timerText.text = $"{hours:00}:{minutes:00}:{seconds:00}:{milliseconds:00}";
    }

    public void StopTimer()
    {
        isRunning = false;
        finalTime = timer; // Save for next scene
    }

    public float GetFinalTime()
    {
        return timer;
    }

    public static string FormatTime(float time)
    {
        int hours = Mathf.FloorToInt(time / 3600f);
        int minutes = Mathf.FloorToInt((time % 3600f) / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        int milliseconds = Mathf.FloorToInt((time * 1000f) % 1000f);

        return $"{hours:00}:{minutes:00}:{seconds:00}:{milliseconds:000}";
    }
}
