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

    [Header("Audio")]
    [SerializeField] private AudioClip cookSuccessSfx;

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
}