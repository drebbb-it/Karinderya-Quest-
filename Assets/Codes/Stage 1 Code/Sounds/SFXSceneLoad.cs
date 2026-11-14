using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SFXSceneLoad : MonoBehaviour
{
    [SerializeField] private float delay = 0.2f;

    public void LoadLevelSelection()
    {
        StartCoroutine(LoadAfterDelay("LevelSelection"));
    }

    private IEnumerator LoadAfterDelay(string sceneName)
    {
        yield return new WaitForSecondsRealtime(delay);
        SceneManager.LoadScene(sceneName);
    }
}