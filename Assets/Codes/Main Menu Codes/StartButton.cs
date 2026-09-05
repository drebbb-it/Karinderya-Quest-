using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{
    // Make sure "GameScene" matches your actual gameplay scene name in Build Settings
    public string sceneToLoad = "GameScene";

    public void OnClick()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}