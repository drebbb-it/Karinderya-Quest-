using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// BackgroundCameraAligner — fits the orthographic camera exactly to the
/// Stage2 background sprite (Stage2_bg1.png, 1920×1080) with no blank bars.
///
/// Strategy:
///   • Find Stage2_bg1_0's SpriteRenderer at runtime to get its world-space bounds.
///   • Set orthographic size = bgHeight / 2  (fills the screen top-to-bottom).
///   • Set camera Y     = bounds.center.y    (aligns vertically to background).
///   • Clamp camera X within background left/right edges (keeps game view in-bounds).
///
/// Falls back to hardcoded scene-calibrated values when the sprite cannot be found.
/// Runs once in Start() and again each LateUpdate() to remain stable.
/// </summary>
[RequireComponent(typeof(Camera))]
public class BackgroundCameraAligner : MonoBehaviour
{
    // ─── Hardcoded fallback (measured from scene data) ───────────────────────
    // Background world center and half-extents, pre-computed from Stage2Scene.unity:
    //   canvas scale 0.021356113, sprite 1920×1080 px, PPU 100
    //   bg parent anchoredPosition (579,254) in canvas  → world centre ≈ (0.0505, 0.1009)
    //   bg world size ≈ 26.06 × 10.60  → half-extents (13.031, 5.300)
    private static readonly Vector2 FallbackBgCenter    = new Vector2(0.0505f, 0.1009f);
    private static readonly Vector2 FallbackBgHalfSize  = new Vector2(13.031f, 5.300f);

    [SerializeField] private SpriteRenderer backgroundRenderer;

    private Camera cam;
    private bool    aligned;

    // ─── Auto-attach ─────────────────────────────────────────────────────────
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void AutoAttach()
    {
        string scene = SceneManager.GetActiveScene().name;
        if (!scene.ToLower().Contains("stage") || !scene.Contains("2")) return;

        Camera main = Camera.main;
        if (main != null && main.GetComponent<BackgroundCameraAligner>() == null)
            main.gameObject.AddComponent<BackgroundCameraAligner>();
    }

    // ─── Unity lifecycle ─────────────────────────────────────────────────────
    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    private void Start()
    {
        FindBackground();
        Align();
    }

    private void LateUpdate()
    {
        // Re-align every frame until locked in (guards against canvas layout delays)
        if (!aligned)
        {
            FindBackground();
            Align();
        }
    }

    // ─── Helpers ─────────────────────────────────────────────────────────────
    private void FindBackground()
    {
        if (backgroundRenderer != null) return;

        // Try exact name first
        GameObject bgObj = GameObject.Find("Stage2_bg1_0");
        if (bgObj != null)
            backgroundRenderer = bgObj.GetComponent<SpriteRenderer>();

        // Fallback: scan all SpriteRenderers for one whose name contains "stage2_bg"
        if (backgroundRenderer == null)
        {
            foreach (var sr in FindObjectsByType<SpriteRenderer>(FindObjectsInactive.Exclude))
            {
                if (sr.gameObject.name.ToLower().Contains("stage2_bg"))
                {
                    backgroundRenderer = sr;
                    break;
                }
            }
        }
    }

    public void Align()
    {
        if (cam == null) return;

        // ── 1. Determine background bounds ───────────────────────────────────
        float bgCenterX, bgCenterY, bgHalfW, bgHalfH;

        if (backgroundRenderer != null)
        {
            Bounds b = backgroundRenderer.bounds;

            // Reject degenerate bounds (canvas not laid out yet)
            if (b.size.x < 0.01f || b.size.y < 0.01f)
                return;

            bgCenterX = b.center.x;
            bgCenterY = b.center.y;
            bgHalfW   = b.extents.x;
            bgHalfH   = b.extents.y;
        }
        else
        {
            // Use pre-computed fallback values
            bgCenterX = FallbackBgCenter.x;
            bgCenterY = FallbackBgCenter.y;
            bgHalfW   = FallbackBgHalfSize.x;
            bgHalfH   = FallbackBgHalfSize.y;
        }

        // ── 2. Orthographic size = exact background height / 2 ───────────────
        float targetOrtho = bgHalfH;   // bgHeight / 2

        // ── 3. Camera position ───────────────────────────────────────────────
        float screenAspect  = (float)Screen.width / Mathf.Max(1, Screen.height);
        float camHalfWidth  = targetOrtho * screenAspect;

        // Y: exactly the background center (fills top-to-bottom with no blank bars)
        float targetY = bgCenterY;

        // X: keep current framing but clamp so camera never spills past bg edges
        float minX = bgCenterX - bgHalfW + camHalfWidth;
        float maxX = bgCenterX + bgHalfW - camHalfWidth;
        float targetX;
        if (minX > maxX)
            targetX = bgCenterX;                                   // bg narrower than screen: centre it
        else
            targetX = Mathf.Clamp(transform.position.x, minX, maxX);

        // ── 4. Apply ─────────────────────────────────────────────────────────
        cam.orthographic     = true;
        cam.orthographicSize = targetOrtho;
        transform.position   = new Vector3(targetX, targetY, transform.position.z);

        aligned = true;    // stop LateUpdate loop once successfully aligned
    }
}
