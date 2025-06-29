using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Goal : MonoBehaviour
{
    public GameTimer timer;

    [Header("Next Scene (Build-Compatible)")]
    public string nextSceneName;

#if UNITY_EDITOR
    [Header("Next Scene (Editor-Only)")]
    public SceneAsset nextSceneAsset;
#endif

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (timer != null)
            {
                timer.StopTimer();
            }

            LoadNextScene();
        }
    }

    private void LoadNextScene()
    {
#if UNITY_EDITOR
        if (nextSceneAsset != null)
        {
            string scenePath = AssetDatabase.GetAssetPath(nextSceneAsset);
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            SceneManager.LoadScene(sceneName);
            return;
        }
#endif
        // Fallback for build
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogWarning("No next scene assigned in Goal script.");
        }
    }
}
