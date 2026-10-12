using UnityEngine;

public class FoodTray : MonoBehaviour
{
    public string foodName;
    void OnMouseEnter()
    {
        HoverTooltip.Show(foodName);
    }
    void OnMouseExit()
    {
        HoverTooltip.Hide();
    }
}
