using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CookingPot : MonoBehaviour
{
    [Header("Ingredients")]
    public List<string> ingredientsInPot = new List<string>();
    [Header("Pot Sprites")]
    [SerializeField] private GameObject potClosed;
    [SerializeField] private GameObject potOpen;

    [Header("Cooking Settings")]
    [SerializeField] private float cookingTime = 5f;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("Effects")]
    [SerializeField] private GameObject stoveFire;

    private bool isCooking = false;

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
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Ingredient ingredient = other.GetComponent<Ingredient>();

        if (ingredient != null)
        {
            ingredientsInPot.Add(ingredient.ingredientName);

            Debug.Log("Added: " + ingredient.ingredientName);

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
        if (!isCooking && ingredientsInPot.Count > 0)
        {
            StartCoroutine(CookFood());
        }
    }

    private IEnumerator CookFood()
    {
        isCooking = true;
        
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

        // Turn fire OFF
        if (stoveFire != null)
        {
            stoveFire.SetActive(false);
        }

        yield return new WaitForSeconds(2f);
        
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
    }
}