using UnityEngine;

public class TutorialButton : MonoBehaviour
{
    public GameObject mainMenuScene; // Slot for MainMenuScene
    public GameObject tutorialPanel; // Slot for Tutorial Panel

    public void OnClick()
    {
        if (mainMenuScene != null) mainMenuScene.SetActive(false);
        if (tutorialPanel != null) tutorialPanel.SetActive(true);
    }
}