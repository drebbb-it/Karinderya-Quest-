using UnityEngine;

public class LadleDrag : MonoBehaviour
{
    public Sprite emptyLadle;
    public Sprite filledLadle;
    public Transform tip;
    Vector3 startPos;
    Vector3 offset;
    Camera cam;
    SpriteRenderer sr;
    string heldFood = "";

    void Start()
    {
        cam = Camera.main;
        sr = GetComponent<SpriteRenderer>();
        startPos = transform.position;   
    }

    void OnMouseDown()
    {
        offset = transform.position - MouseWorld();
    }

    void OnMouseDrag() => transform.position = MouseWorld() + offset;

    void OnMouseUp()
    {
        Vector2 point = tip != null ? tip.position : transform.position;
        if (heldFood == "")
        {
            var tray = Physics2D.OverlapPoint(point, LayerMask.GetMask("FoodTray"));
            if (tray)
            {
                var foodTray = tray.GetComponent<FoodTray>();
                if (foodTray != null)
                {
                    heldFood = foodTray.foodName;
                    sr.sprite = filledLadle;
                }
            }
        }
        else
        {
            var hit = Physics2D.OverlapPoint(point, LayerMask.GetMask("DropZone"));
            if (hit)
            {
                var plastic = hit.GetComponent<PlasticContainer>();
                if (plastic != null && !plastic.isFilled)
                {
                    plastic.Fill(heldFood);
                    heldFood = "";
                    sr.sprite = emptyLadle;
                }
            }
        }
        transform.position = startPos;
    }

    Vector3 MouseWorld()
    {
        var p = cam.ScreenToWorldPoint(Input.mousePosition);
        p.z = 0;
        return p;
    }
}