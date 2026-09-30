using System.Collections.Generic;
using UnityEngine;

public class CookingPot : MonoBehaviour
{
    public List<string> ingredientsInPot = new List<string>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        Ingredient ingredient = other.GetComponent<Ingredient>();

        if (ingredient != null)
        {
            ingredientsInPot.Add(ingredient.ingredientName);

            Debug.Log("Added: " + ingredient.ingredientName);

            Destroy(other.gameObject);
        }
    }

    private void OnMouseDown()
    {
        MixIngredients();
    }

    void MixIngredients()
    {
        RecipeManager.Instance.CheckRecipe(ingredientsInPot);

        ingredientsInPot.Clear();
    }
}