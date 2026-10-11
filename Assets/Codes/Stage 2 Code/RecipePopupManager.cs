using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecipePopup : MonoBehaviour
{
    public static RecipePopup Instance;

    [SerializeField] private GameObject popupPanel;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI dishNameText;
    [SerializeField] private Image dishImage;

    private bool popupOpen;

    private void Awake()
    {
        Instance = this;
        popupPanel.SetActive(false);
    }

    private void Update()
    {
        if (popupOpen && Input.GetMouseButtonDown(0))
        {
            popupPanel.SetActive(false);
            popupOpen = false;
        }
    }

    public void ShowPopup(string dishName, Sprite dishSprite)
    {
        popupPanel.SetActive(true);

        titleText.text = "Cooked:";
        dishNameText.text = dishName;
        dishImage.sprite = dishSprite;

        popupOpen = true;
    }
}