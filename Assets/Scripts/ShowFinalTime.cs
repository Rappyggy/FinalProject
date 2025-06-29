using UnityEngine;
using TMPro;

public class ShowFinalTime : MonoBehaviour
{
    public TextMeshProUGUI finalTimeText;

    void Start()
    {
        finalTimeText.text = "Time: " + GameTimer.FormatTime(GameTimer.finalTime);
    }
}
