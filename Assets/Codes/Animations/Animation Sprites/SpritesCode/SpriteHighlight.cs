using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteHighlight : MonoBehaviour
{
    [SerializeField] private Color highlightColor = new Color(1f, 1f, 0.6f, 1f);
    [SerializeField] private float hoverScale = 1.1f;
    [SerializeField] private SpriteRenderer[] highlightRenderers;

    private SpriteRenderer sr;
    private Collider2D[] hoverColliders;
    private Camera mainCamera;
    private Color[] originalColors;
    private Vector3[] originalScales;
    private bool isHighlighted;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        hoverColliders = GetComponents<Collider2D>();
        mainCamera = Camera.main;
        if (highlightRenderers == null || highlightRenderers.Length == 0)
        {
            highlightRenderers = new[] { sr };
        }

        originalColors = new Color[highlightRenderers.Length];
        originalScales = new Vector3[highlightRenderers.Length];
        for (int i = 0; i < highlightRenderers.Length; i++)
        {
            SpriteRenderer renderer = highlightRenderers[i];
            if (renderer == null)
            {
                Debug.LogError($"SpriteHighlight on {name} has a missing target renderer.", this);
                continue;
            }

            originalColors[i] = renderer.color;
            originalScales[i] = renderer.transform.localScale;
        }
    }

    void Update()
    {
        if (PauseManager.IsPaused || !IsPointerOverTarget())
        {
            ResetHighlight();
            return;
        }

        ApplyHighlight();
    }

    private bool IsPointerOverTarget()
    {
        if (mainCamera == null) mainCamera = Camera.main;
        if (mainCamera == null) return false;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (hoverColliders != null)
        {
            foreach (Collider2D hoverCollider in hoverColliders)
            {
                if (hoverCollider == null || !hoverCollider.enabled) continue;

                Plane colliderPlane = new Plane(Vector3.forward, hoverCollider.bounds.center);
                if (colliderPlane.Raycast(ray, out float distance) &&
                    distance >= 0f &&
                    hoverCollider.OverlapPoint(ray.GetPoint(distance)))
                {
                    return true;
                }
            }
        }

        foreach (SpriteRenderer renderer in highlightRenderers)
        {
            if (renderer == null) continue;

            Bounds bounds = renderer.bounds;
            Plane spritePlane = new Plane(Vector3.forward, bounds.center);
            if (spritePlane.Raycast(ray, out float distance) &&
                distance >= 0f &&
                bounds.Contains(ray.GetPoint(distance)))
            {
                return true;
            }
        }

        return false;
    }

    private void ApplyHighlight()
    {
        if (isHighlighted) return;

        for (int i = 0; i < highlightRenderers.Length; i++)
        {
            SpriteRenderer renderer = highlightRenderers[i];
            if (renderer == null) continue;

            renderer.color = highlightColor;
            renderer.transform.localScale = originalScales[i] * hoverScale;
        }

        isHighlighted = true;
    }

    void OnDisable()
    {
        ResetHighlight();
    }

    public void ResetHighlight()
    {
        if (!isHighlighted) return;

        for (int i = 0; i < highlightRenderers.Length; i++)
        {
            SpriteRenderer renderer = highlightRenderers[i];
            if (renderer == null) continue;

            renderer.color = originalColors[i];
            renderer.transform.localScale = originalScales[i];
        }
        isHighlighted = false;
    }
}