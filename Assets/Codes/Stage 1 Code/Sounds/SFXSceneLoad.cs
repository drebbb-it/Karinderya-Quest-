using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SFXSceneLoad : MonoBehaviour
{
    [SerializeField] private float delay = 0.2f;

    private static bool isLoading;

    public void LoadLevelSelection()
    {
        LoadSceneAfterDelay(this, "LevelSelection", delay);
    }

    public static void LoadSceneAfterDelay(MonoBehaviour caller, string sceneName, float delay = 0.2f)
    {
        if (caller == null)
        {
            Debug.LogError("SFXSceneLoad requires a coroutine owner.");
            return;
        }

        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogError("SFXSceneLoad requires a scene name.");
            return;
        }

        if (isLoading) return;

        isLoading = true;
        caller.StartCoroutine(LoadAfterDelay(sceneName, delay));
    }

    private static IEnumerator LoadAfterDelay(string sceneName, float delay)
    {
        yield return new WaitForSecondsRealtime(Mathf.Max(0f, delay));
        isLoading = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }
}