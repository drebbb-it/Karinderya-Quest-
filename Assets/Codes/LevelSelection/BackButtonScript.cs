using UnityEngine;
using UnityEngine.SceneManagement;
public class BackButtonScript : MonoBehaviour
{
    public void BackToLevelSelection()
    {
        SceneManager.LoadScene("LevelSelection");
    }
}
