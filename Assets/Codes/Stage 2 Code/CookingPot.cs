    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CookingPot : MonoBehaviour
{
    [Header("Ingredients")]
    public List<string> ingredientsInPot = new List<string>();

    [Header("Cooking Settings")]
    [SerializeField] private float cookingTime = 5f;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI timerText;

    private bool isCooking = false;

    private void Start()
    {
        if (timerText != null)
        {
            timerText.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
{
    Ingredient ingredient = other.GetComponent<Ingredient>();

    if (ingredient != null)
    {
        ingredientsInPot.Add(ingredient.ingredientName);

        Debug.Log("Added: " + ingredient.ingredientName);

        IngredientSpawner spawner = other.GetComponent<IngredientSpawner>();

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

        float timeRemaining = cookingTime;

        if (timerText != null)
        {
            timerText.gameObject.SetActive(true);
        }

        while (timeRemaining > 0)
        {
            if (timerText != null)
            {
                timerText.text = "Cooking: " + Mathf.Ceil(timeRemaining) + "s";
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

        yield return new WaitForSeconds(2f);

        if (timerText != null)
        {
            timerText.gameObject.SetActive(false);
        }

        isCooking = false;
    }
}