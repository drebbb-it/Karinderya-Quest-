using UnityEngine;

public class    raggableIngredient : MonoBehaviour
{
    private Vector3 offset;
    private Camera mainCam;
    private Rigidbody2D rb;

    void Start()
    {
        mainCam = Camera.main;
        rb = GetComponent<Rigidbody2D>();

        // Start with no gravity
        rb.gravityScale = 0;
    }

    void OnMouseDown()
    {
        if (PauseManager.IsPaused) return;
        if (CookbookUI.IsOpen)     return;
        if (RecipePopup.IsOpen)    return;
        rb.gravityScale = 0;
        offset = transform.position - GetMouseWorldPosition();
    }

    void OnMouseUp()
    {
        if (PauseManager.IsPaused) return;
        if (CookbookUI.IsOpen)     return;
        if (RecipePopup.IsOpen)    return;
        rb.gravityScale = 1;
    }

    void OnMouseDrag()
    {
        if (PauseManager.IsPaused) return;
        if (CookbookUI.IsOpen)     return;
        if (RecipePopup.IsOpen)    return;
        transform.position = GetMouseWorldPosition() + offset;
    }

    Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10f;

        return mainCam.ScreenToWorldPoint(mousePos);
    }
}
