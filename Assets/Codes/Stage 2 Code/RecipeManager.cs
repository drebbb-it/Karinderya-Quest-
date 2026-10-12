using System.Collections.Generic;
using UnityEngine;

public class RecipeManager : MonoBehaviour
{
    public static RecipeManager Instance;

    [Header("Dish Sprites")]
    public Sprite afritadaSprite;
    public Sprite ginilingSprite;
    public Sprite menudoSprite;
    public Sprite adoboSprite;
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

        // AFRITADA (Chicken)
        if (result == "BellPepper,Carrot,Chicken,Potato,TomatoSauce")
        {
            Debug.Log("You made Afritada!");

            RecipePopup.Instance.ShowPopup(
                "Afritada",
                afritadaSprite
            );

            ObjectiveManager.Instance.DishCooked("Afritada");
        }

        // AFRITADA (Pork)
        else if (result == "BellPepper,Carrot,Pork,Potato,TomatoSauce")
        {
            Debug.Log("You made Afritada!");

            RecipePopup.Instance.ShowPopup(
                "Afritada",
                afritadaSprite
            );

            ObjectiveManager.Instance.DishCooked("Afritada");
        }

        // GINILING
        else if (result == "BayLeaves,Carrot,Garlic,GroundPork,Onion,Patis,Potato")
        {
            Debug.Log("You made Giniling!");

            RecipePopup.Instance.ShowPopup(
                "Giniling",
                ginilingSprite
            );

            ObjectiveManager.Instance.DishCooked("Giniling");
        }

        // MENUDO
        else if (result == "Carrot,Pork,Potato,TomatoSauce")
        {
            Debug.Log("You made Menudo!");

            RecipePopup.Instance.ShowPopup(
                "Menudo",
                menudoSprite
            );

            ObjectiveManager.Instance.DishCooked("Menudo");
        }

        // ADOBO (Chicken)
        else if (result == "BayLeaves,Chicken,Garlic,SoySauce,Vinegar")
        {
            Debug.Log("You made Adobo!");

            RecipePopup.Instance.ShowPopup(
                "Adobo",
                adoboSprite
            );

            ObjectiveManager.Instance.DishCooked("Adobo");
        }

        // ADOBO (Pork)
        else if (result == "BayLeaves,Garlic,Pork,SoySauce,Vinegar")
        {
            Debug.Log("You made Adobo!");

            RecipePopup.Instance.ShowPopup(
                "Adobo",
                adoboSprite
            );

            ObjectiveManager.Instance.DishCooked("Adobo");
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