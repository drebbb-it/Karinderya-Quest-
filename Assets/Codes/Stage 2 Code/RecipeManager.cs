using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RecipeManager : MonoBehaviour
{
    public static RecipeManager Instance;

    private const int StartingLives = 3;

    [Header("Dish Sprites")]
    public Sprite afritadaSprite;
    public Sprite ginilingSprite;
    public Sprite menudoSprite;
    public Sprite adoboSprite;
    public Sprite failedSprite;

    [Header("Lives")]
    [SerializeField] private Sprite heartSprite;
    [SerializeField] private Sprite emptyHeartSprite;
    [SerializeField] private Image[] heartImages;

    [Header("Audio")]
    [SerializeField] private AudioClip cookSuccessSfx;

    private int currentLives;
    private bool pendingGameOver;
    private bool gameOverShown;

    private void Awake()
    {
        Instance = this;
        currentLives = StartingLives;
    }

    private void OnEnable()
    {
        RecipePopup.PopupDismissed += HandleRecipePopupDismissed;
    }

    private void OnDisable()
    {
        RecipePopup.PopupDismissed -= HandleRecipePopupDismissed;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void Start()
    {
        UpdateLivesDisplay();
    }

    public void CheckRecipe(List<string> ingredients)
    {
        ingredients.Sort();

        string result = string.Join(",", ingredients);

        Debug.Log("Recipe: " + result);

        // AFRITADA (Chicken)
        if (result == "BellPepper,Carrot,Chicken,Potato,TomatoSauce")
        {
            Debug.Log("You made Afritada!");
            ShowSuccessfulRecipe("Afritada", afritadaSprite);
        }

        // AFRITADA (Pork)
        else if (result == "BellPepper,Carrot,Pork,Potato,TomatoSauce")
        {
            Debug.Log("You made Afritada!");
            ShowSuccessfulRecipe("Afritada", afritadaSprite);
        }

        // GINILING
        else if (result == "BayLeaves,Carrot,Garlic,GroundPork,Onion,Patis,Potato")
        {
            Debug.Log("You made Giniling!");
            ShowSuccessfulRecipe("Giniling", ginilingSprite);
        }

        // MENUDO
        else if (result == "Carrot,Pork,Potato,TomatoSauce")
        {
            Debug.Log("You made Menudo!");
            ShowSuccessfulRecipe("Menudo", menudoSprite);
        }

        // ADOBO (Chicken)
        else if (result == "BayLeaves,Chicken,Garlic,SoySauce,Vinegar")
        {
            Debug.Log("You made Adobo!");
            ShowSuccessfulRecipe("Adobo", adoboSprite);
        }

        // ADOBO (Pork)
        else if (result == "BayLeaves,Garlic,Pork,SoySauce,Vinegar")
        {
            Debug.Log("You made Adobo!");
            ShowSuccessfulRecipe("Adobo", adoboSprite);
        }

        else
        {
            Debug.Log("Failed Recipe!");
            LoseHeart();

            RecipePopup.Instance.ShowPopup(
                "Failed Recipe",
                failedSprite
            );
        }
    }

    private void ShowSuccessfulRecipe(string dishName, Sprite dishSprite)
    {
        ButtonSFXUtility.PlayOneShot(cookSuccessSfx);
        RecipePopup.Instance.ShowPopup(dishName, dishSprite);
        ObjectiveManager.Instance.DishCooked(dishName);
    }

    private void LoseHeart()
    {
        if (currentLives <= 0 || gameOverShown) return;

        currentLives--;
        UpdateLivesDisplay();
        pendingGameOver = currentLives == 0;
    }

    private void UpdateLivesDisplay()
    {
        if (heartImages == null) return;

        for (int i = 0; i < heartImages.Length; i++)
        {
            if (heartImages[i] != null)
            {
                heartImages[i].sprite = i < currentLives ? heartSprite : emptyHeartSprite;
            }
        }
    }

    private void HandleRecipePopupDismissed()
    {
        if (!pendingGameOver || gameOverShown) return;

        pendingGameOver = false;
        gameOverShown = true;
        PauseManager.LockForGameOver();

        GameObject defeatPanel = StageVictoryPopup.Create(
            "GAME OVER!",
            "You lost all three hearts after failed dishes.",
            new[] { "PLAY AGAIN", "LEVEL SELECT" },
            new UnityAction[]
            {
                RestartStage,
                LoadLevelSelection
            },
            new[]
            {
                new Color(0.45f, 0.25f, 0.25f),
                new Color(0.25f, 0.35f, 0.55f)
            });
        defeatPanel.SetActive(true);
    }

    private void RestartStage()
    {
        SFXSceneLoad.LoadSceneAfterDelay(this, SceneManager.GetActiveScene().name);
    }

    private void LoadLevelSelection()
    {
        SFXSceneLoad.LoadSceneAfterDelay(this, "LevelSelection");
    }
}