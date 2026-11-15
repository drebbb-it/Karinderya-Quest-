using UnityEngine;

public class StartButton : MonoBehaviour
{
    public void OnClick()
    {
        SFXSceneLoad.LoadSceneAfterDelay(this, "LevelSelection");
    }
}