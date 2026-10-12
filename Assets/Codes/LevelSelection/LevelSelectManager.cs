using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSelectManager : MonoBehaviour
{
    [Header("Buttons")]
    public Button level1Button;
    public Button level2Button;
    public Button level3Button;

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

    private void Start()
    {
        UpdateLevelButtons();
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
        SceneManager.LoadScene("Stage1Scene");
    }

    public void OpenLevel2()
    {
        if (PlayerPrefs.GetInt("Level2Unlocked", 0) == 1)
        {
            SceneManager.LoadScene("Stage2Scene");
        }
    }

    public void OpenLevel3()
    {
        if (PlayerPrefs.GetInt("Level3Unlocked", 0) == 1)
        {
            SceneManager.LoadScene("Stage3Scene");
        }
    }
    public void ResetProgress()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        UpdateLevelButtons();

        Debug.Log("Progress Reset!");
    }
}