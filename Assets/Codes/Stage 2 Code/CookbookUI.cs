using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// CookbookUI — self-building recipe book for Scene 2.
///
/// When open, only the cookbook panel is interactive.
/// Ingredients cannot be dragged and the cooking pot cannot be clicked
/// (DraggableIngredient and CookingPot check CookbookUI.IsOpen).
/// The game keeps running (time is NOT stopped).
///
/// No Inspector wiring needed — the script spawns itself via SceneManager.sceneLoaded
/// and builds all UI at runtime.  Sprites are auto-pulled from RecipeManager.
/// You can still assign sprites manually on the component in the Inspector.
/// </summary>
public class CookbookUI : MonoBehaviour
{
    // ─────────────────────────────────────────────────────────────────
    //  Static state — readable by DraggableIngredient / CookingPot
    // ─────────────────────────────────────────────────────────────────

    /// <summary>True while the cookbook panel is open.</summary>
    public static bool IsOpen { get; private set; }

    public static CookbookUI Instance { get; private set; }

    // ─────────────────────────────────────────────────────────────────
    //  Auto-spawn in Stage 2 on every scene load
    // ─────────────────────────────────────────────────────────────────

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Bootstrap()
    {
        IsOpen = false;
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        IsOpen = false;   // always reset when a new scene loads

        // Only spawn in Stage 2
        bool isStage2 = scene.name.Contains("2") && scene.name.ToLower().Contains("stage");
        if (!isStage2) return;

        if (FindAnyObjectByType<CookbookUI>() == null)
        {
            GameObject go = new GameObject("CookbookUI");
            go.AddComponent<CookbookUI>();
        }
    }

    // ─────────────────────────────────────────────────────────────────
    //  Sprites (auto-pulled from RecipeManager; override in Inspector)
    // ─────────────────────────────────────────────────────────────────

    [Header("Dish Sprites (auto-pulled from RecipeManager if left empty)")]
    public Sprite afritadaSprite;
    public Sprite ginilingSprite;
    public Sprite menudoSprite;
    public Sprite adoboSprite;

    // ─────────────────────────────────────────────────────────────────
    //  Internal
    // ─────────────────────────────────────────────────────────────────

    private GameObject cookbookPanel;

    // Active-tab highlight references
    private Image _activeTabImg;
    private Text  _activeTabLbl;

    private class RecipeEntry
    {
        public string             DishName;
        public Sprite             DishSprite;
        public List<List<string>> Variants = new List<List<string>>();
    }

    private List<RecipeEntry> recipes;

    // ─────────────────────────────────────────────────────────────────
    //  Unity lifecycle
    // ─────────────────────────────────────────────────────────────────

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        IsOpen   = false;
    }

    private void Start()
    {
        // Pull sprites from RecipeManager after all Awakes have run
        if (RecipeManager.Instance != null)
        {
            if (afritadaSprite == null) afritadaSprite = RecipeManager.Instance.afritadaSprite;
            if (ginilingSprite == null) ginilingSprite = RecipeManager.Instance.ginilingSprite;
            if (menudoSprite   == null) menudoSprite   = RecipeManager.Instance.menudoSprite;
            if (adoboSprite    == null) adoboSprite    = RecipeManager.Instance.adoboSprite;
        }

        BuildRecipeList();
        BuildRuntimeUI();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            IsOpen   = false;
            Instance = null;
        }
    }

    // ─────────────────────────────────────────────────────────────────
    //  Recipe data (mirrors RecipeManager.cs, failed recipe excluded)
    // ─────────────────────────────────────────────────────────────────

    private void BuildRecipeList()
    {
        recipes = new List<RecipeEntry>
        {
            new RecipeEntry
            {
                DishName   = "Afritada",
                DishSprite = afritadaSprite,
                Variants   = new List<List<string>>
                {
                    new List<string> { "Chicken", "Carrot", "Potato", "Bell Pepper", "Tomato Sauce" },
                    new List<string> { "Pork",    "Carrot", "Potato", "Bell Pepper", "Tomato Sauce" }
                }
            },
            new RecipeEntry
            {
                DishName   = "Giniling",
                DishSprite = ginilingSprite,
                Variants   = new List<List<string>>
                {
                    new List<string> { "Ground Pork", "Carrot", "Potato", "Onion", "Garlic", "Bay Leaves", "Patis" }
                }
            },
            new RecipeEntry
            {
                DishName   = "Menudo",
                DishSprite = menudoSprite,
                Variants   = new List<List<string>>
                {
                    new List<string> { "Pork", "Carrot", "Potato", "Tomato Sauce" }
                }
            },
            new RecipeEntry
            {
                DishName   = "Adobo",
                DishSprite = adoboSprite,
                Variants   = new List<List<string>>
                {
                    new List<string> { "Chicken", "Garlic", "Bay Leaves", "Soy Sauce", "Vinegar" },
                    new List<string> { "Pork",    "Garlic", "Bay Leaves", "Soy Sauce", "Vinegar" }
                }
            }
        };
    }

    // ─────────────────────────────────────────────────────────────────
    //  Runtime UI construction
    // ─────────────────────────────────────────────────────────────────

    private void BuildRuntimeUI()
    {
        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf")
                 ?? Resources.GetBuiltinResource<Font>("Arial.ttf");

        // ── Canvas (sorting order below PauseManager so pause always wins) ──
        GameObject canvasGO = new GameObject("CookbookCanvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode      = RenderMode.ScreenSpaceOverlay;
        canvas.overrideSorting = true;
        canvas.sortingOrder    = 30000;

        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode         = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution  = new Vector2(1280f, 720f);
        scaler.matchWidthOrHeight   = 0.5f;

        canvasGO.AddComponent<GraphicRaycaster>();
        EnsureEventSystem();

        // ── 📖 Cookbook button — bottom-right corner ──────────────────
        GameObject btnGO = new GameObject("CookbookButton",
            typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        btnGO.transform.SetParent(canvasGO.transform, false);

        RectTransform brt = btnGO.GetComponent<RectTransform>();
        brt.anchorMin        = new Vector2(1f, 0f);
        brt.anchorMax        = new Vector2(1f, 0f);
        brt.pivot            = new Vector2(1f, 0f);
        brt.anchoredPosition = new Vector2(-20f, 20f);
        brt.sizeDelta        = new Vector2(130f, 45f);

        btnGO.GetComponent<Image>().color = new Color(0.55f, 0.30f, 0.10f, 0.92f);

        GameObject btnLbl = MakeText(btnGO.transform, "BtnLabel", "\U0001F4D6  Cookbook",
            font, 15, FontStyle.Bold, Color.white, TextAnchor.MiddleCenter);
        StretchFill(btnLbl.GetComponent<RectTransform>());

        Button openBtn = btnGO.GetComponent<Button>();
        ColorBlock ocb = openBtn.colors;
        ocb.highlightedColor = new Color(0.72f, 0.42f, 0.16f, 1f);
        ocb.pressedColor     = new Color(0.38f, 0.20f, 0.06f, 1f);
        openBtn.colors = ocb;
        openBtn.onClick.AddListener(ToggleCookbook);

        // ── Cookbook modal panel ───────────────────────────────────────
        cookbookPanel = new GameObject("CookbookPanel",
            typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        cookbookPanel.transform.SetParent(canvasGO.transform, false);

        // Full-screen dim overlay (blocks clicks on game world)
        RectTransform prt = cookbookPanel.GetComponent<RectTransform>();
        prt.anchorMin = Vector2.zero;
        prt.anchorMax = Vector2.one;
        prt.sizeDelta = Vector2.zero;
        cookbookPanel.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.60f);

        // Dialog box
        GameObject box = new GameObject("DialogBox",
            typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        box.transform.SetParent(cookbookPanel.transform, false);

        RectTransform boxRt = box.GetComponent<RectTransform>();
        boxRt.anchorMin        = new Vector2(0.5f, 0.5f);
        boxRt.anchorMax        = new Vector2(0.5f, 0.5f);
        boxRt.pivot            = new Vector2(0.5f, 0.5f);
        boxRt.anchoredPosition = Vector2.zero;
        boxRt.sizeDelta        = new Vector2(640f, 460f);
        box.GetComponent<Image>().color = new Color(0.14f, 0.09f, 0.04f, 0.97f);

        // Title
        GameObject titleGO = MakeText(box.transform, "Title", "\U0001F4D6  Cookbook",
            font, 22, FontStyle.Bold, new Color(1f, 0.88f, 0.55f), TextAnchor.MiddleCenter);
        RectTransform titleRt = titleGO.GetComponent<RectTransform>();
        titleRt.anchorMin        = new Vector2(0f, 1f);
        titleRt.anchorMax        = new Vector2(1f, 1f);
        titleRt.pivot            = new Vector2(0.5f, 1f);
        titleRt.anchoredPosition = new Vector2(0f, -16f);
        titleRt.sizeDelta        = new Vector2(0f, 40f);

        // ✕ Close button
        GameObject closeGO = new GameObject("CloseBtn",
            typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        closeGO.transform.SetParent(box.transform, false);
        RectTransform crt = closeGO.GetComponent<RectTransform>();
        crt.anchorMin        = new Vector2(1f, 1f);
        crt.anchorMax        = new Vector2(1f, 1f);
        crt.pivot            = new Vector2(1f, 1f);
        crt.anchoredPosition = new Vector2(-8f, -8f);
        crt.sizeDelta        = new Vector2(36f, 36f);
        closeGO.GetComponent<Image>().color = new Color(0.75f, 0.15f, 0.10f, 0.9f);
        GameObject closeLbl = MakeText(closeGO.transform, "X", "\u2715",
            font, 18, FontStyle.Bold, Color.white, TextAnchor.MiddleCenter);
        StretchFill(closeLbl.GetComponent<RectTransform>());
        closeGO.GetComponent<Button>().onClick.AddListener(CloseCookbook);

        // ── Tab row ────────────────────────────────────────────────────
        GameObject tabRow = new GameObject("TabRow",
            typeof(RectTransform), typeof(HorizontalLayoutGroup));
        tabRow.transform.SetParent(box.transform, false);

        RectTransform tabRt = tabRow.GetComponent<RectTransform>();
        tabRt.anchorMin        = new Vector2(0f, 1f);
        tabRt.anchorMax        = new Vector2(1f, 1f);
        tabRt.pivot            = new Vector2(0.5f, 1f);
        tabRt.anchoredPosition = new Vector2(0f, -60f);
        tabRt.sizeDelta        = new Vector2(0f, 38f);

        HorizontalLayoutGroup hlg = tabRow.GetComponent<HorizontalLayoutGroup>();
        hlg.spacing               = 5f;
        hlg.childAlignment        = TextAnchor.MiddleCenter;
        hlg.childControlWidth     = true;
        hlg.childForceExpandWidth = true;
        hlg.padding               = new RectOffset(10, 10, 0, 0);

        // ── Detail area ────────────────────────────────────────────────

        // Dish image (left column)
        GameObject imgGO = new GameObject("DishImage",
            typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        imgGO.transform.SetParent(box.transform, false);
        RectTransform imgRt = imgGO.GetComponent<RectTransform>();
        imgRt.anchorMin = new Vector2(0f,    0f);
        imgRt.anchorMax = new Vector2(0.42f, 1f);
        imgRt.offsetMin = new Vector2( 16f,  16f);
        imgRt.offsetMax = new Vector2( -6f, -106f);
        Image dishImg = imgGO.GetComponent<Image>();
        dishImg.color          = Color.white;
        dishImg.preserveAspect = true;

        // Dish name (right column, top)
        GameObject dishNameGO = new GameObject("DishName",
            typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        dishNameGO.transform.SetParent(box.transform, false);
        RectTransform dnRt = dishNameGO.GetComponent<RectTransform>();
        dnRt.anchorMin        = new Vector2(0.42f, 1f);
        dnRt.anchorMax        = new Vector2(1f,    1f);
        dnRt.pivot            = new Vector2(0.5f,  1f);
        dnRt.anchoredPosition = new Vector2(0f, -104f);
        dnRt.sizeDelta        = new Vector2(0f,  36f);
        Text dnText           = dishNameGO.GetComponent<Text>();
        dnText.font      = font;
        dnText.fontSize  = 20;
        dnText.fontStyle = FontStyle.Bold;
        dnText.color     = new Color(1f, 0.85f, 0.40f);
        dnText.alignment = TextAnchor.MiddleLeft;

        // Ingredients (right column, body)
        GameObject ingGO = new GameObject("Ingredients",
            typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        ingGO.transform.SetParent(box.transform, false);
        RectTransform ingRt = ingGO.GetComponent<RectTransform>();
        ingRt.anchorMin = new Vector2(0.42f, 0f);
        ingRt.anchorMax = new Vector2(1f,    1f);
        ingRt.offsetMin = new Vector2( 10f,  16f);
        ingRt.offsetMax = new Vector2(-16f, -142f);
        Text ingText           = ingGO.GetComponent<Text>();
        ingText.font           = font;
        ingText.fontSize       = 15;
        ingText.color          = new Color(0.95f, 0.90f, 0.75f);
        ingText.alignment      = TextAnchor.UpperLeft;
        ingText.supportRichText = true;

        // ── Build one tab per recipe ───────────────────────────────────
        bool firstTab = true;
        foreach (RecipeEntry entry in recipes)
        {
            RecipeEntry captured = entry;

            GameObject tabGO = new GameObject("Tab_" + entry.DishName,
                typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
            tabGO.transform.SetParent(tabRow.transform, false);

            Image tabImg = tabGO.GetComponent<Image>();
            tabImg.color = new Color(0.30f, 0.18f, 0.06f, 1f);

            GameObject tabLblGO = MakeText(tabGO.transform, "Lbl", entry.DishName,
                font, 13, FontStyle.Bold, new Color(0.9f, 0.80f, 0.55f), TextAnchor.MiddleCenter);
            StretchFill(tabLblGO.GetComponent<RectTransform>());
            Text tabLbl = tabLblGO.GetComponent<Text>();

            Button tabBtn = tabGO.GetComponent<Button>();
            ColorBlock tcb = tabBtn.colors;
            tcb.highlightedColor = new Color(0.50f, 0.30f, 0.10f, 1f);
            tcb.pressedColor     = new Color(0.22f, 0.12f, 0.04f, 1f);
            tabBtn.colors = tcb;

            Image capturedImg = tabImg;
            Text  capturedLbl = tabLbl;
            tabBtn.onClick.AddListener(() =>
            {
                ShowRecipe(captured, dishImg, ingText, dnText);
                HighlightTab(capturedImg, capturedLbl);
            });

            if (firstTab)
            {
                firstTab = false;
                // Prime first dish display (called when panel is first opened)
                tabBtn.onClick.Invoke();
            }
        }

        cookbookPanel.SetActive(false);
    }

    // ─────────────────────────────────────────────────────────────────
    //  Helpers
    // ─────────────────────────────────────────────────────────────────

    private void HighlightTab(Image img, Text lbl)
    {
        if (_activeTabImg != null) _activeTabImg.color = new Color(0.30f, 0.18f, 0.06f, 1f);
        if (_activeTabLbl != null) _activeTabLbl.color = new Color(0.9f, 0.80f, 0.55f);

        img.color = new Color(0.70f, 0.42f, 0.14f, 1f);
        lbl.color = Color.white;

        _activeTabImg = img;
        _activeTabLbl = lbl;
    }

    private void ShowRecipe(RecipeEntry entry, Image dishImg, Text ingText, Text dishNameText)
    {
        if (dishNameText != null) dishNameText.text = entry.DishName;

        if (dishImg != null)
        {
            dishImg.sprite  = entry.DishSprite;
            dishImg.enabled = entry.DishSprite != null;
        }

        if (ingText != null)
        {
            var sb = new System.Text.StringBuilder();
            if (entry.Variants.Count == 1)
            {
                foreach (string ing in entry.Variants[0])
                    sb.AppendLine("\u2022 " + ing);
            }
            else
            {
                for (int i = 0; i < entry.Variants.Count; i++)
                {
                    sb.AppendLine($"<b>Option {i + 1}:</b>");
                    foreach (string ing in entry.Variants[i])
                        sb.AppendLine("  \u2022 " + ing);
                    if (i < entry.Variants.Count - 1)
                        sb.AppendLine();
                }
            }
            ingText.text = sb.ToString().TrimEnd();
        }
    }

    private static GameObject MakeText(Transform parent, string name, string text,
        Font font, int size, FontStyle style, Color color, TextAnchor alignment)
    {
        GameObject go = new GameObject(name,
            typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        go.transform.SetParent(parent, false);
        Text t            = go.GetComponent<Text>();
        t.font            = font;
        t.text            = text;
        t.fontSize        = size;
        t.fontStyle       = style;
        t.color           = color;
        t.alignment       = alignment;
        t.supportRichText = true;
        return go;
    }

    private static void StretchFill(RectTransform rt)
    {
        rt.anchorMin        = Vector2.zero;
        rt.anchorMax        = Vector2.one;
        rt.sizeDelta        = Vector2.zero;
        rt.anchoredPosition = Vector2.zero;
    }

    private static void EnsureEventSystem()
    {
        if (FindAnyObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject es = new GameObject("EventSystem");
            es.AddComponent<UnityEngine.EventSystems.EventSystem>();
            es.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }
    }

    // ─────────────────────────────────────────────────────────────────
    //  Public API
    // ─────────────────────────────────────────────────────────────────

    /// <summary>
    /// Toggle the cookbook open or closed.
    /// Does NOT touch PauseManager — game time keeps running.
    /// DraggableIngredient and CookingPot check CookbookUI.IsOpen to block input.
    /// </summary>
    public void ToggleCookbook()
    {
        if (IsOpen) CloseCookbook();
        else        OpenCookbook();
    }

    public void OpenCookbook()
    {
        IsOpen = true;
        cookbookPanel.SetActive(true);
    }

    /// <summary>Close the cookbook and re-enable gameplay input.</summary>
    public void CloseCookbook()
    {
        IsOpen = false;
        cookbookPanel.SetActive(false);
    }
}
