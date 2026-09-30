using UnityEngine;

public class DraggableIngredient : MonoBehaviour
{
    private Vector3 offset;
    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;
    }

    void OnMouseDown()
    {
        offset = transform.position - GetMouseWorldPosition();
    }

    void OnMouseDrag()
    {
        transform.position = GetMouseWorldPosition() + offset;
    }

    Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10f;

        return mainCam.ScreenToWorldPoint(mousePos);
    }
}