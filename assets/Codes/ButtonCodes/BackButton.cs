using UnityEngine;

public class BackButton : MonoBehaviour
{
    public GameObject mainMenuScene; // Slot for MainMenuScene
    public GameObject tutorialPanel; // Slot for Tutorial Panel

    public void OnClick()
    {
        if (tutorialPanel != null) tutorialPanel.SetActive(false);
        if (mainMenuScene != null) mainMenuScene.SetActive(true);
    }
}