using UnityEngine;

public class LadleDrag : MonoBehaviour
{
    Vector3 startPos;
    Vector3 offset;
    Camera cam;

    void Start()
    {
        cam = Camera.main;
        startPos = transform.position;   // uses the position you set in the Scene
    }

    void OnMouseDown()
    {
        offset = transform.position - MouseWorld();
    }

    void OnMouseDrag() => transform.position = MouseWorld() + offset;

    void OnMouseUp()
    {
        var hit = Physics2D.OverlapPoint(transform.position, LayerMask.GetMask("DropZone"));
        if (hit)
        {
            var plastic = hit.GetComponent<PlasticContainer>();
            if (plastic != null) plastic.Fill();
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