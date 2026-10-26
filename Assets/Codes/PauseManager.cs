using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Universal Pause Manager for Karinderya Quest gameplay scenes.
/// Handles ESC key and Pause button clicks, freezes time (pausing physics, timers, animations, and coroutines),
/// blocks gameplay raycasts/interactions, and provides Resume, Restart, and Level Selection navigation.
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

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void OnSceneLoaded()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        // Only run in gameplay scenes
        if (sceneName.StartsWith("Stage"))
        {
            if (FindAnyObjectByType<PauseManager>() == null)
            {
                GameObject pauseGo = new GameObject("PauseManager");
                pauseGo.AddComponent<PauseManager>();
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

        // Ensure clean state when scene starts
        Time.timeScale = 1f;
        IsPaused = false;

        EnsureUI();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            // Restore timeScale if leaving scene while paused
            Time.timeScale = 1f;
            IsPaused = false;
            Instance = null;
        }
    }

    private void Update()
    {
        // ESC key toggles pause
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (IsPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        IsPaused = true;
        Time.timeScale = 0f;

        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
        }

        if (pauseButton != null)
        {
            pauseButton.gameObject.SetActive(false);
        }
    }

    public void ResumeGame()
    {
        IsPaused = false;
        Time.timeScale = 1f;

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        if (pauseButton != null)
        {
            pauseButton.gameObject.SetActive(true);
        }
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        IsPaused = false;
        string currentScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentScene);
    }

    public void LoadLevelSelect()
    {
        Time.timeScale = 1f;
        IsPaused = false;
        SceneManager.LoadScene("LevelSelection");
    }

    private void EnsureUI()
    {
        // If pausePanel or pauseButton are already assigned in inspector, bind their listeners
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

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

        // If no UI was wired up in inspector, build the pause UI dynamically on a Canvas
        if (pausePanel == null || pauseButton == null)
        {
            BuildRuntimeUI();
        }
    }

    private void BuildRuntimeUI()
    {
        // Dedicated screen space overlay canvas with maximum sorting order to guarantee pause is always on top
        GameObject canvasObj = new GameObject("PauseScreenOverlayCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.overrideSorting = true;
        canvas.sortingOrder = 32767;

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(800, 600);
        scaler.matchWidthOrHeight = 0f;

        canvasObj.AddComponent<GraphicRaycaster>();

        // Ensure EventSystem exists
        if (FindAnyObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject es = new GameObject("EventSystem");
            es.AddComponent<UnityEngine.EventSystems.EventSystem>();
            es.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }

        Font standardFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (standardFont == null)
        {
            standardFont = Resources.GetBuiltinResource<Font>("Arial.ttf");
        }

        // 1. Build Top-Right / Top-Left Pause Button if not present
        if (pauseButton == null)
        {
            GameObject btnObj = new GameObject("PauseButton", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
            btnObj.transform.SetParent(canvas.transform, false);

            RectTransform rt = btnObj.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0f, 1f);
            rt.anchorMax = new Vector2(0f, 1f);
            rt.pivot = new Vector2(0f, 1f);
            rt.anchoredPosition = new Vector2(25f, -25f);
            rt.sizeDelta = new Vector2(100f, 40f);

            Image img = btnObj.GetComponent<Image>();
            img.color = new Color(0.15f, 0.15f, 0.2f, 0.85f);

            GameObject textObj = new GameObject("Text", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            textObj.transform.SetParent(btnObj.transform, false);
            RectTransform trt = textObj.GetComponent<RectTransform>();
            trt.anchorMin = Vector2.zero;
            trt.anchorMax = Vector2.one;
            trt.sizeDelta = Vector2.zero;

            Text txt = textObj.GetComponent<Text>();
            txt.font = standardFont;
            txt.text = "PAUSE";
            txt.fontSize = 18;
            txt.fontStyle = FontStyle.Bold;
            txt.color = Color.white;
            txt.alignment = TextAnchor.MiddleCenter;

            pauseButton = btnObj.GetComponent<Button>();
            pauseButton.onClick.AddListener(PauseGame);
        }

        // 2. Build Pause Modal Panel if not present
        if (pausePanel == null)
        {
            pausePanel = new GameObject("PauseModalPanel", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            pausePanel.transform.SetParent(canvas.transform, false);

            RectTransform prt = pausePanel.GetComponent<RectTransform>();
            prt.anchorMin = Vector2.zero;
            prt.anchorMax = Vector2.one;
            prt.sizeDelta = Vector2.zero;

            // Semi-transparent dark overlay
            Image panelImg = pausePanel.GetComponent<Image>();
            panelImg.color = new Color(0f, 0f, 0f, 0.75f);

            // Dialog box
            GameObject boxObj = new GameObject("DialogBox", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            boxObj.transform.SetParent(pausePanel.transform, false);
            RectTransform boxRt = boxObj.GetComponent<RectTransform>();
            boxRt.anchorMin = new Vector2(0.5f, 0.5f);
            boxRt.anchorMax = new Vector2(0.5f, 0.5f);
            boxRt.pivot = new Vector2(0.5f, 0.5f);
            boxRt.anchoredPosition = Vector2.zero;
            boxRt.sizeDelta = new Vector2(320f, 320f);

            Image boxImg = boxObj.GetComponent<Image>();
            boxImg.color = new Color(0.12f, 0.12f, 0.16f, 0.95f);

            // Title
            GameObject titleObj = new GameObject("TitleText", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            titleObj.transform.SetParent(boxObj.transform, false);
            RectTransform titleRt = titleObj.GetComponent<RectTransform>();
            titleRt.anchorMin = new Vector2(0f, 1f);
            titleRt.anchorMax = new Vector2(1f, 1f);
            titleRt.pivot = new Vector2(0.5f, 1f);
            titleRt.anchoredPosition = new Vector2(0f, -25f);
            titleRt.sizeDelta = new Vector2(0f, 40f);

            Text titleTxt = titleObj.GetComponent<Text>();
            titleTxt.font = standardFont;
            titleTxt.text = "PAUSED";
            titleTxt.fontSize = 28;
            titleTxt.fontStyle = FontStyle.Bold;
            titleTxt.color = Color.white;
            titleTxt.alignment = TextAnchor.MiddleCenter;

            // Button helper
            resumeButton = CreateDialogButton(boxObj.transform, "ResumeButton", "RESUME", new Vector2(0f, -90f), standardFont, ResumeGame);
            restartButton = CreateDialogButton(boxObj.transform, "RestartButton", "RESTART", new Vector2(0f, -155f), standardFont, RestartLevel);
            levelSelectButton = CreateDialogButton(boxObj.transform, "LevelSelectButton", "LEVEL SELECT", new Vector2(0f, -220f), standardFont, LoadLevelSelect);

            pausePanel.SetActive(false);
        }
    }

    private Button CreateDialogButton(Transform parent, string name, string label, Vector2 anchoredPos, Font font, UnityEngine.Events.UnityAction action)
    {
        GameObject btnObj = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        btnObj.transform.SetParent(parent, false);

        RectTransform rt = btnObj.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 1f);
        rt.anchorMax = new Vector2(0.5f, 1f);
        rt.pivot = new Vector2(0.5f, 1f);
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = new Vector2(240f, 45f);

        Image img = btnObj.GetComponent<Image>();
        img.color = new Color(0.25f, 0.28f, 0.35f, 1f);

        Button btn = btnObj.GetComponent<Button>();
        ColorBlock cb = btn.colors;
        cb.highlightedColor = new Color(0.35f, 0.4f, 0.5f, 1f);
        cb.pressedColor = new Color(0.18f, 0.2f, 0.25f, 1f);
        btn.colors = cb;

        GameObject textObj = new GameObject("Text", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        textObj.transform.SetParent(btnObj.transform, false);
        RectTransform trt = textObj.GetComponent<RectTransform>();
        trt.anchorMin = Vector2.zero;
        trt.anchorMax = Vector2.one;
        trt.sizeDelta = Vector2.zero;

        Text txt = textObj.GetComponent<Text>();
        txt.font = font;
        txt.text = label;
        txt.fontSize = 18;
        txt.fontStyle = FontStyle.Bold;
        txt.color = Color.white;
        txt.alignment = TextAnchor.MiddleCenter;

        btn.onClick.AddListener(action);
        return btn;
    }
}
