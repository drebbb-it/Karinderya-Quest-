using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Universal Pause Manager for Karinderya Quest gameplay scenes.
/// Handles ESC key and Pause button clicks, freezes time (pausing physics, timers,
/// animations, and coroutines), blocks gameplay raycasts/interactions, and provides
/// Resume, Restart, and Level Selection navigation.
/// </summary>
public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance { get; private set; }

    [Header("UI References (Optional - Auto-created if null)")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button levelSelectButton;

    public static bool IsPaused { get; private set; }

    // ── Bootstrap: runs ONCE at game start, subscribes to every future scene load ──
    // SceneManager.sceneLoaded fires on EVERY scene load (including Restart),
    // unlike [RuntimeInitializeOnLoadMethod(AfterSceneLoad)] which only fires once.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Bootstrap()
    {
        // Reset state at game launch
        IsPaused = false;
        Time.timeScale = 1f;

        // Subscribe once; will fire for every scene from now on
        SceneManager.sceneLoaded -= HandleSceneLoaded;   // prevent double-subscribe on domain reload
        SceneManager.sceneLoaded += HandleSceneLoaded;
    }

    private static void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Always guarantee clean state on every scene load
        IsPaused = false;
        Time.timeScale = 1f;

        // Auto-create PauseManager in every Stage scene if one isn't already there
        if (scene.name.StartsWith("Stage"))
        {
            if (FindAnyObjectByType<PauseManager>() == null)
            {
                GameObject go = new GameObject("PauseManager");
                go.AddComponent<PauseManager>();
            }
        }
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Guarantee clean state (static values survive domain reload in Editor)
        Time.timeScale = 1f;
        IsPaused = false;

        EnsureUI();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Time.timeScale = 1f;
            IsPaused = false;
            Instance = null;
        }
    }

    private void Update()
    {
        // ESC key toggles pause — also closes cookbook first if it is open
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (CookbookUI.IsOpen)
                CookbookUI.Instance?.CloseCookbook();
            else
                TogglePause();
        }
    }

    public void TogglePause()
    {
        if (IsPaused) ResumeGame();
        else           PauseGame();
    }

    public void PauseGame()
    {
        IsPaused = true;
        Time.timeScale = 0f;

        if (pausePanel != null)  pausePanel.SetActive(true);
        if (pauseButton != null) pauseButton.gameObject.SetActive(false);
    }

    public void ResumeGame()
    {
        IsPaused = false;
        Time.timeScale = 1f;

        if (pausePanel != null)  pausePanel.SetActive(false);
        if (pauseButton != null) pauseButton.gameObject.SetActive(true);
    }

    public void RestartLevel()
    {
        // Reset all state before reloading
        IsPaused = false;
        Time.timeScale = 1f;
        string currentScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentScene);
    }

    public void LoadLevelSelect()
    {
        IsPaused = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene("LevelSelection");
    }

    private void EnsureUI()
    {
        if (pausePanel != null)   pausePanel.SetActive(false);

        if (pauseButton != null)
        {
            pauseButton.onClick.RemoveAllListeners();
            pauseButton.onClick.AddListener(PauseGame);
        }

        if (resumeButton != null)
        {
            resumeButton.onClick.RemoveAllListeners();
            resumeButton.onClick.AddListener(ResumeGame);
        }

        if (restartButton != null)
        {
            restartButton.onClick.RemoveAllListeners();
            restartButton.onClick.AddListener(RestartLevel);
        }

        if (levelSelectButton != null)
        {
            levelSelectButton.onClick.RemoveAllListeners();
            levelSelectButton.onClick.AddListener(LoadLevelSelect);
        }

        // Build runtime UI only when nothing was wired in the Inspector
        if (pausePanel == null || pauseButton == null)
        {
            BuildRuntimeUI();
        }
    }

    private void BuildRuntimeUI()
    {
        // Dedicated screen-space overlay canvas — highest sort order so it's always on top
        GameObject canvasObj = new GameObject("PauseScreenOverlayCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode      = RenderMode.ScreenSpaceOverlay;
        canvas.overrideSorting = true;
        canvas.sortingOrder    = 32767;

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode        = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(800, 600);
        scaler.matchWidthOrHeight  = 0f;

        canvasObj.AddComponent<GraphicRaycaster>();

        // Ensure EventSystem exists
        if (FindAnyObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject es = new GameObject("EventSystem");
            es.AddComponent<UnityEngine.EventSystems.EventSystem>();
            es.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }

        Font standardFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf")
                         ?? Resources.GetBuiltinResource<Font>("Arial.ttf");

        // ── Pause Button (top-left) ───────────────────────────────────
        if (pauseButton == null)
        {
            GameObject btnObj = new GameObject("PauseButton",
                typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
            btnObj.transform.SetParent(canvas.transform, false);

            RectTransform rt = btnObj.GetComponent<RectTransform>();
            rt.anchorMin        = new Vector2(0f, 1f);
            rt.anchorMax        = new Vector2(0f, 1f);
            rt.pivot            = new Vector2(0f, 1f);
            rt.anchoredPosition = new Vector2(25f, -25f);
            rt.sizeDelta        = new Vector2(100f, 40f);

            btnObj.GetComponent<Image>().color = new Color(0.15f, 0.15f, 0.2f, 0.85f);

            GameObject textObj = new GameObject("Text",
                typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            textObj.transform.SetParent(btnObj.transform, false);
            RectTransform trt = textObj.GetComponent<RectTransform>();
            trt.anchorMin = Vector2.zero;
            trt.anchorMax = Vector2.one;
            trt.sizeDelta = Vector2.zero;

            Text txt = textObj.GetComponent<Text>();
            txt.font      = standardFont;
            txt.text      = "PAUSE";
            txt.fontSize  = 18;
            txt.fontStyle = FontStyle.Bold;
            txt.color     = Color.white;
            txt.alignment = TextAnchor.MiddleCenter;

            pauseButton = btnObj.GetComponent<Button>();
            pauseButton.onClick.AddListener(PauseGame);
        }

        // ── Pause Modal Panel ─────────────────────────────────────────
        if (pausePanel == null)
        {
            pausePanel = new GameObject("PauseModalPanel",
                typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            pausePanel.transform.SetParent(canvas.transform, false);

            RectTransform prt = pausePanel.GetComponent<RectTransform>();
            prt.anchorMin = Vector2.zero;
            prt.anchorMax = Vector2.one;
            prt.sizeDelta = Vector2.zero;

            pausePanel.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.75f);

            // Dialog box
            GameObject boxObj = new GameObject("DialogBox",
                typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            boxObj.transform.SetParent(pausePanel.transform, false);
            RectTransform boxRt = boxObj.GetComponent<RectTransform>();
            boxRt.anchorMin        = new Vector2(0.5f, 0.5f);
            boxRt.anchorMax        = new Vector2(0.5f, 0.5f);
            boxRt.pivot            = new Vector2(0.5f, 0.5f);
            boxRt.anchoredPosition = Vector2.zero;
            boxRt.sizeDelta        = new Vector2(320f, 320f);
            boxObj.GetComponent<Image>().color = new Color(0.12f, 0.12f, 0.16f, 0.95f);

            // Title
            GameObject titleObj = new GameObject("TitleText",
                typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            titleObj.transform.SetParent(boxObj.transform, false);
            RectTransform titleRt = titleObj.GetComponent<RectTransform>();
            titleRt.anchorMin        = new Vector2(0f, 1f);
            titleRt.anchorMax        = new Vector2(1f, 1f);
            titleRt.pivot            = new Vector2(0.5f, 1f);
            titleRt.anchoredPosition = new Vector2(0f, -25f);
            titleRt.sizeDelta        = new Vector2(0f, 40f);

            Text titleTxt = titleObj.GetComponent<Text>();
            titleTxt.font      = standardFont;
            titleTxt.text      = "PAUSED";
            titleTxt.fontSize  = 28;
            titleTxt.fontStyle = FontStyle.Bold;
            titleTxt.color     = Color.white;
            titleTxt.alignment = TextAnchor.MiddleCenter;

            resumeButton      = CreateDialogButton(boxObj.transform, "ResumeButton",      "RESUME",       new Vector2(0f,  -90f), standardFont, ResumeGame);
            restartButton     = CreateDialogButton(boxObj.transform, "RestartButton",     "RESTART",      new Vector2(0f, -155f), standardFont, RestartLevel);
            levelSelectButton = CreateDialogButton(boxObj.transform, "LevelSelectButton", "LEVEL SELECT", new Vector2(0f, -220f), standardFont, LoadLevelSelect);

            pausePanel.SetActive(false);
        }
    }

    private Button CreateDialogButton(Transform parent, string name, string label,
        Vector2 anchoredPos, Font font, UnityEngine.Events.UnityAction action)
    {
        GameObject btnObj = new GameObject(name,
            typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        btnObj.transform.SetParent(parent, false);

        RectTransform rt = btnObj.GetComponent<RectTransform>();
        rt.anchorMin        = new Vector2(0.5f, 1f);
        rt.anchorMax        = new Vector2(0.5f, 1f);
        rt.pivot            = new Vector2(0.5f, 1f);
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta        = new Vector2(240f, 45f);

        Image img = btnObj.GetComponent<Image>();
        img.color = new Color(0.25f, 0.28f, 0.35f, 1f);

        Button btn = btnObj.GetComponent<Button>();
        ColorBlock cb = btn.colors;
        cb.highlightedColor = new Color(0.35f, 0.4f, 0.5f, 1f);
        cb.pressedColor     = new Color(0.18f, 0.2f, 0.25f, 1f);
        btn.colors = cb;

        GameObject textObj = new GameObject("Text",
            typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        textObj.transform.SetParent(btnObj.transform, false);
        RectTransform trt = textObj.GetComponent<RectTransform>();
        trt.anchorMin = Vector2.zero;
        trt.anchorMax = Vector2.one;
        trt.sizeDelta = Vector2.zero;

        Text txt = textObj.GetComponent<Text>();
        txt.font      = font;
        txt.text      = label;
        txt.fontSize  = 18;
        txt.fontStyle = FontStyle.Bold;
        txt.color     = Color.white;
        txt.alignment = TextAnchor.MiddleCenter;

        btn.onClick.AddListener(action);
        return btn;
    }
}
