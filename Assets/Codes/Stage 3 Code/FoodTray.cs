using UnityEngine;

public class FoodTray : MonoBehaviour
{
    public string foodName;

    void Awake()
    {
        if (GetComponent<SpriteHighlight>() == null)
        {
            gameObject.AddComponent<SpriteHighlight>();
        }
    }

    void OnMouseEnter()
    {
        if (PauseManager.IsPaused)
        {
            HoverTooltip.Hide();
            return;
        }

        HoverTooltip.Show(foodName);
    }
    void OnMouseExit()
    {
        HoverTooltip.Hide();
    }
}
