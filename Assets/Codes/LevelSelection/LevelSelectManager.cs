using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LevelSelectManager : MonoBehaviour
{
    [Header("Buttons")]
    public Button level1Button;
    public Button level2Button;
    public Button level3Button;

    [Header("Reset Confirmation")]
    [SerializeField] private Button resetButton;
    [SerializeField] private Button backButton;

    [Header("Button Images")]
    public Image level1Image;
    public Image level2Image;
    public Image level3Image;

    [Header("Sprites")]
    public Sprite level1Sprite;

    public Sprite level2Sprite;
    public Sprite level2LockedSprite;

    public Sprite level3Sprite;
    public Sprite level3LockedSprite;

    private Button[] selectionButtons;
    private GameObject resetConfirmationPanel;

    private void Start()
    {
        UpdateLevelButtons();
        selectionButtons = new[] { level1Button, level2Button, level3Button, resetButton, backButton };
        resetConfirmationPanel = StageVictoryPopup.Create(
            "RESET PROGRESS?",
            "Are you sure you want to reset your unlocked stages?",
            new[] { "CONFIRM RESET", "CANCEL" },
            new UnityAction[] { ConfirmResetProgress, CancelResetProgress },
            new[]
            {
                new Color(0.65f, 0.18f, 0.18f),
                new Color(0.25f, 0.4f, 0.55f)
            });
    }

    public void UpdateLevelButtons()
    {
        bool level2Unlocked =
            PlayerPrefs.GetInt("Level2Unlocked", 0) == 1;

        bool level3Unlocked =
            PlayerPrefs.GetInt("Level3Unlocked", 0) == 1;

        // Level 1
        level1Image.sprite = level1Sprite;
        level1Button.interactable = true;

        // Level 2
        level2Image.sprite =
            level2Unlocked
            ? level2Sprite
            : level2LockedSprite;

        level2Button.interactable = level2Unlocked;

        // Level 3
        level3Image.sprite =
            level3Unlocked
            ? level3Sprite
            : level3LockedSprite;

        level3Button.interactable = level3Unlocked;
    }

    public void OpenLevel1()
    {
        SFXSceneLoad.LoadSceneAfterDelay(this, "Stage1Scene");
    }

    public void OpenLevel2()
    {
        if (PlayerPrefs.GetInt("Level2Unlocked", 0) == 1)
        {
            SFXSceneLoad.LoadSceneAfterDelay(this, "Stage2Scene");
        }
    }

    public void OpenLevel3()
    {
        if (PlayerPrefs.GetInt("Level3Unlocked", 0) == 1)
        {
            SFXSceneLoad.LoadSceneAfterDelay(this, "Stage3Scene");
        }
    }
    public void ResetProgress()
    {
        if (resetConfirmationPanel == null)
        {
            Debug.LogError("LevelSelectManager could not create the reset confirmation popup.", this);
            return;
        }

        SetSelectionButtonsActive(false);
        resetConfirmationPanel.SetActive(true);
    }

    private void ConfirmResetProgress()
    {
        PlayerPrefs.DeleteKey("Level2Unlocked");
        PlayerPrefs.DeleteKey("Level3Unlocked");
        PlayerPrefs.Save();

        UpdateLevelButtons();
        CloseResetConfirmation();

        Debug.Log("Progress Reset!");
    }

    private void CancelResetProgress()
    {
        CloseResetConfirmation();
    }

    private void CloseResetConfirmation()
    {
        resetConfirmationPanel.SetActive(false);
        SetSelectionButtonsActive(true);
    }

    private void SetSelectionButtonsActive(bool isActive)
    {
        foreach (Button button in selectionButtons)
        {
            if (button != null)
            {
                button.gameObject.SetActive(isActive);
            }
        }
    }

    public void BackToMainMenu()
    {
        SFXSceneLoad.LoadSceneAfterDelay(this, "MainMenu");
    }
}