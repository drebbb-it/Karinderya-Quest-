using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CookingPot : MonoBehaviour
{
    public static CookingPot Instance { get; private set; }

    /// <summary>
    /// Event invoked whenever ingredients in the pot are modified (added or cleared).
    /// Passes the current read-only list of ingredient names.
    /// </summary>
    public static event Action<List<string>> OnIngredientsChanged;

    [Header("Ingredients")]
    public List<string> ingredientsInPot = new List<string>();

    [Header("Pot Sprites")]
    [SerializeField] private GameObject potClosed;
    [SerializeField] private GameObject potOpen;

    [Header("Cooking Settings")]
    [SerializeField] private float cookingTime = 5f;
    [SerializeField] private AudioClip cookingSfx;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("Effects")]
    [SerializeField] private GameObject stoveFire;

    private bool isCooking = false;
    private SpriteHighlight spriteHighlight;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        spriteHighlight = GetComponent<SpriteHighlight>();
        if (spriteHighlight == null)
        {
            spriteHighlight = gameObject.AddComponent<SpriteHighlight>();
        }

        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void SetCookbookOpen(bool isOpen)
    {
        if (spriteHighlight != null)
        {
            spriteHighlight.enabled = !isOpen && !isCooking;
        }
    }

    private void Start()
    {
        if (timerText != null)
        {
            timerText.gameObject.SetActive(false);
        }

        if (stoveFire != null)
        {
            stoveFire.SetActive(false);
        }

        if (potClosed != null)
        {
            potClosed.SetActive(true);
        }

        if (potOpen != null)
        {
            potOpen.SetActive(false);
        }

        NotifyIngredientsChanged();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isCooking) return;

        Ingredient ingredient = other.GetComponent<Ingredient>();

        if (ingredient != null)
        {
            ingredientsInPot.Add(ingredient.ingredientName);

            Debug.Log("Added: " + ingredient.ingredientName);

            NotifyIngredientsChanged();

            IngredientSpawner spawner =
                other.GetComponent<IngredientSpawner>();

            if (spawner != null)
            {
                spawner.RespawnIngredient();
            }

            Destroy(other.gameObject);
        }
    }

    private void OnMouseDown()
    {
        if (PauseManager.IsPaused) return;
        if (CookbookUI.IsOpen)     return;
        if (RecipePopup.IsOpen)    return;
        if (!isCooking && ingredientsInPot.Count > 0)
        {
            StartCoroutine(CookFood());
        }
    }

    private IEnumerator CookFood()
    {
        isCooking = true;
        spriteHighlight.enabled = false;
        ButtonSFXUtility.PlayOneShot(cookingSfx);
        
        if (potClosed != null)
        {
            potClosed.SetActive(false);
        }

        if (potOpen != null)
        {
            potOpen.SetActive(true);
        }

        // Turn fire ON
        if (stoveFire != null)
        {
            stoveFire.SetActive(true);
        }

        float timeRemaining = cookingTime;

        if (timerText != null)
        {
            timerText.gameObject.SetActive(true);
        }

        while (timeRemaining > 0)
        {
            if (timerText != null)
            {
                timerText.text =
                    "Cooking: " + Mathf.Ceil(timeRemaining) + "s";
            }

            timeRemaining -= Time.deltaTime;

            yield return null;
        }

        if (timerText != null)
        {
            timerText.text = "Done!";
        }

        RecipeManager.Instance.CheckRecipe(ingredientsInPot);

        ingredientsInPot.Clear();
        NotifyIngredientsChanged();

        // Turn fire OFF
        if (stoveFire != null)
        {
            stoveFire.SetActive(false);
        }

        float waitTime = 2f;
        while (waitTime > 0f)
        {
            waitTime -= Time.deltaTime;
            yield return null;
        }
        
        if (potClosed != null)
        {
            potClosed.SetActive(true);
        }

        if (potOpen != null)
        {
            potOpen.SetActive(false);
        }
            
        if (timerText != null)
        {
            timerText.gameObject.SetActive(false);
        }

        isCooking = false;
        spriteHighlight.enabled = !CookbookUI.IsOpen;
    }

    public void NotifyIngredientsChanged()
    {
        OnIngredientsChanged?.Invoke(ingredientsInPot);
    }
}
