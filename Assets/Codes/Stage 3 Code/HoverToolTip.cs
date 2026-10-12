using UnityEngine;
using TMPro;

public class HoverTooltip : MonoBehaviour
{
    public Vector3 offset = new Vector3(0, 0.6f, 0);  

    static HoverTooltip instance;
    TextMeshPro text;
    Camera cam;

    void Awake()
    {
        instance = this;
        text = GetComponent<TextMeshPro>();
        cam = Camera.main;
        text.enabled = false;
    }

    void Update()
    {
        if (!text.enabled) return;
        Vector3 p = cam.ScreenToWorldPoint(Input.mousePosition);
        p.z = 0;
        transform.position = p + offset;
    }

    public static void Show(string message)
    {
        if (instance == null) return;
        instance.text.text = message;
        instance.text.enabled = true;
    }

    public static void Hide()
    {
        if (instance == null) return;
        instance.text.enabled = false;
    }
}