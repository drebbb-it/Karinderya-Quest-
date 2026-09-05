using UnityEngine;

public class ExitButton : MonoBehaviour
{
    public void OnClick()
    {
        Debug.Log("Game Exited");
        Application.Quit();
    }
}