using TMPro;
using UnityEngine;

public class RecipePopup : MonoBehaviour
{
    public static RecipePopup Instance;

    [SerializeField] private GameObject popupPanel;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI dishNameText;

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

    public void ShowPopup(string dishName)
    {
        popupPanel.SetActive(true);

        titleText.text = "Cooked";
        dishNameText.text = dishName;

        popupOpen = true;
    }
}