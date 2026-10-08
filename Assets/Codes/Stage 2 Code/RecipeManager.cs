using System.Collections.Generic;
using UnityEngine;

public class RecipeManager : MonoBehaviour
{
    public static RecipeManager Instance;

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
            Debug.Log("You made Soup!");
            RecipePopup.Instance.ShowPopup("Soup");
        }
        else if (result == "Carrot,Meat,Tomato")
        {
            Debug.Log("You made Stew!");
            RecipePopup.Instance.ShowPopup("Stew");
        }
        else
        {
            Debug.Log("Failed Recipe!");
            RecipePopup.Instance.ShowPopup("Failed Recipe");
        }
    }
}