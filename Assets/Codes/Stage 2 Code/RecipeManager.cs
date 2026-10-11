using System.Collections.Generic;
using UnityEngine;

public class RecipeManager : MonoBehaviour
{
    public static RecipeManager Instance;

    [Header("Dish Sprites")]
    public Sprite afritadaSprite;
    public Sprite ginilingSprite;
    public Sprite failedSprite;

    private void Awake()
    {
        Instance = this;
    }

    public void CheckRecipe(List<string> ingredients)
    {
        ingredients.Sort();

        string result = string.Join(",", ingredients);

        Debug.Log("Recipe: " + result);

        if (result == "Carrot,Tomato")
        {
            Debug.Log("You made Afritada!");
            RecipePopup.Instance.ShowPopup(
                "Afritada",
                afritadaSprite
            );
        }
        else if (result == "Carrot,Meat,Tomato")
        {
            Debug.Log("You made Giniling!");
            RecipePopup.Instance.ShowPopup(
                "Giniling",
                ginilingSprite
            );
        }
        else
        {
            Debug.Log("Failed Recipe!");
            RecipePopup.Instance.ShowPopup(
                "Failed Recipe",
                failedSprite
            );
        }
    }
}