using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteHighlight : MonoBehaviour
{
    [SerializeField] private Color highlightColor = new Color(1f, 1f, 0.6f, 1f);
    [SerializeField] private float hoverScale = 1.1f;
    [SerializeField] private SpriteRenderer[] highlightRenderers;

    private SpriteRenderer sr;
    private Color[] originalColors;
    private Vector3[] originalScales;
    private bool isHighlighted;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
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

    void OnMouseEnter()
    {
        if (PauseManager.IsPaused)
        {
            ResetHighlight();
            return;
        }

        for (int i = 0; i < highlightRenderers.Length; i++)
        {
            SpriteRenderer renderer = highlightRenderers[i];
            if (renderer == null) continue;

            renderer.color = highlightColor;
            renderer.transform.localScale = originalScales[i] * hoverScale;
        }
        isHighlighted = true;
    }

    void OnMouseExit()
    {
        ResetHighlight();
    }

    void Update()
    {
        if (PauseManager.IsPaused)
        {
            ResetHighlight();
        }
    }

    void OnDisable()
    {
        ResetHighlight();
    }

    private void ResetHighlight()
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