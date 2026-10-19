using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{
    public void OnClick()
    {
        SceneManager.LoadScene("LevelSelection");
    }
}