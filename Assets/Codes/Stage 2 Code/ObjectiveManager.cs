using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ObjectiveManager : MonoBehaviour
{
    public static ObjectiveManager Instance;

    [System.Serializable]
    public class Objective
    {
        public string dishName;
        public int targetAmount;

        [HideInInspector]
        public int currentAmount;
    }

    [Header("Objectives")]
    public List<Objective> objectives = new List<Objective>();

    [Header("Randomizer Settings")]
    [Tooltip("If true, generates random dish objectives on start.")]
    [SerializeField] private bool randomizeObjectives = true;
    [Tooltip("Minimum dishes selected to cook (must be >= 1).")]
    [SerializeField] private int minDishesToCook = 1;
    [Tooltip("Maximum dishes selected to cook (cannot exceed available dishes).")]
    [SerializeField] private int maxDishesToCook = 3;
    [Tooltip("Minimum target amount per selected dish.")]
    [SerializeField] private int minTargetAmount = 1;
    [Tooltip("Maximum target amount per selected dish (capped at 5).")]
    [SerializeField] private int maxTargetAmount = 5;

    [Header("UI")]
    public TextMeshProUGUI objectiveText;

    [Header("Victory Popup (Optional - Auto-created if null)")]
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private Button nextLevelButton;
    [SerializeField] private Button levelSelectButton;
    [SerializeField] private Button restartButton;

    private bool isLevelCompleted = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (randomizeObjectives)
        {
            GenerateRandomObjectives();
        }

        UpdateUI();

        // If inspector didn't assign a custom victory panel, build a fallback runtime UI
        if (victoryPanel == null)
        {
            BuildRuntimeVictoryPopup();
        }
        else
        {
            victoryPanel.SetActive(false);
            WireVictoryButtons();
        }
    }

    /// <summary>
    /// Randomizes the objectives ensuring target amounts go no higher than 5.
    /// </summary>
    public void GenerateRandomObjectives()
    {
        // Pool of valid dishes matching RecipeManager
        List<string> availableDishes = new List<string> { "Afritada", "Giniling", "Menudo", "Adobo" };

        // Shuffle available dishes using Fisher-Yates
        for (int i = 0; i < availableDishes.Count; i++)
        {
            int rnd = Random.Range(i, availableDishes.Count);
            string temp = availableDishes[i];
            availableDishes[i] = availableDishes[rnd];
            availableDishes[rnd] = temp;
        }

        // Clamp parameters so targets never exceed 5
        int minDishCount = Mathf.Clamp(minDishesToCook, 1, availableDishes.Count);
        int maxDishCount = Mathf.Clamp(maxDishesToCook, minDishCount, availableDishes.Count);
        int countToPick = Random.Range(minDishCount, maxDishCount + 1);

        int minAmt = Mathf.Clamp(minTargetAmount, 1, 5);
        int maxAmt = Mathf.Clamp(maxTargetAmount, minAmt, 5); // Capped at 5

        objectives.Clear();

        for (int i = 0; i < countToPick; i++)
        {
            Objective obj = new Objective
            {
                dishName = availableDishes[i],
                targetAmount = Random.Range(minAmt, maxAmt + 1),
                currentAmount = 0
            };
            objectives.Add(obj);
        }
    }

    public void DishCooked(string dishName)
    {
        foreach (Objective objective in objectives)
        {
            if (objective.dishName == dishName)
            {
                objective.currentAmount++;

                if (objective.currentAmount > objective.targetAmount)
                {
                    objective.currentAmount = objective.targetAmount;
                }

                UpdateUI();

                if (AllObjectivesCompleted() && !isLevelCompleted)
                {
                    OnAllObjectivesCompleted();
                }

                return;
            }
        }
    }

    private void OnAllObjectivesCompleted()
    {
        isLevelCompleted = true;
        Debug.Log("Level 2 Complete! All objectives cleared.");

        LevelUnlockManager.UnlockLevel3();

        ShowVictoryPopup();
    }

    private void ShowVictoryPopup()
    {
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
        }
        Time.timeScale = 0f; // Freeze game scene on completion
    }

    public void OnNextLevelClicked()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Stage3Scene");
    }

    public void OnLevelSelectClicked()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("LevelSelection");
    }

    public void OnRestartClicked()
    {
        Time.timeScale = 1f;
        string currentScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentScene);
    }

    private void WireVictoryButtons()
    {
        if (nextLevelButton != null)
        {
            nextLevelButton.onClick.RemoveAllListeners();
            nextLevelButton.onClick.AddListener(OnNextLevelClicked);
        }

        if (levelSelectButton != null)
        {
            levelSelectButton.onClick.RemoveAllListeners();
            levelSelectButton.onClick.AddListener(OnLevelSelectClicked);
        }

        if (restartButton != null)
        {
            restartButton.onClick.RemoveAllListeners();
            restartButton.onClick.AddListener(OnRestartClicked);
        }
    }

    private void UpdateUI()
    {
        if (objectiveText == null) return;

        objectiveText.text = "";

        foreach (Objective objective in objectives)
        {
            bool completed = objective.currentAmount >= objective.targetAmount;
            string status = completed ? "✅" : "❌";

            objectiveText.text +=
                $"{status} {objective.currentAmount}/{objective.targetAmount} {objective.dishName}\n";
        }
    }

    public bool AllObjectivesCompleted()
    {
        if (objectives == null || objectives.Count == 0) return false;

        foreach (Objective objective in objectives)
        {
            if (objective.currentAmount < objective.targetAmount)
            {
                return false;
            }
        }

        return true;
    }

    private void BuildRuntimeVictoryPopup()
    {
        // Overlay canvas with high sorting order
        GameObject canvasObj = new GameObject("Stage2VictoryOverlayCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.overrideSorting = true;
        canvas.sortingOrder = 32000;

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1280f, 720f);
        scaler.matchWidthOrHeight = 0.5f;

        canvasObj.AddComponent<GraphicRaycaster>();

        Font standardFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf")
                         ?? Resources.GetBuiltinResource<Font>("Arial.ttf");

        // Dim background
        victoryPanel = new GameObject("VictoryPanel", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        victoryPanel.transform.SetParent(canvasObj.transform, false);

        RectTransform prt = victoryPanel.GetComponent<RectTransform>();
        prt.anchorMin = Vector2.zero;
        prt.anchorMax = Vector2.one;
        prt.sizeDelta = Vector2.zero;
        victoryPanel.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.75f);

        // Dialog box
        GameObject boxObj = new GameObject("VictoryBox", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        boxObj.transform.SetParent(victoryPanel.transform, false);

        RectTransform boxRt = boxObj.GetComponent<RectTransform>();
        boxRt.anchorMin = new Vector2(0.5f, 0.5f);
        boxRt.anchorMax = new Vector2(0.5f, 0.5f);
        boxRt.pivot = new Vector2(0.5f, 0.5f);
        boxRt.anchoredPosition = Vector2.zero;
        boxRt.sizeDelta = new Vector2(460f, 380f);
        boxObj.GetComponent<Image>().color = new Color(0.12f, 0.12f, 0.15f, 0.96f);

        // Header Title
        GameObject titleObj = new GameObject("TitleText", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        titleObj.transform.SetParent(boxObj.transform, false);
        RectTransform titleRt = titleObj.GetComponent<RectTransform>();
        titleRt.anchorMin = new Vector2(0f, 1f);
        titleRt.anchorMax = new Vector2(1f, 1f);
        titleRt.pivot = new Vector2(0.5f, 1f);
        titleRt.anchoredPosition = new Vector2(0f, -25f);
        titleRt.sizeDelta = new Vector2(0f, 45f);

        Text titleTxt = titleObj.GetComponent<Text>();
        titleTxt.font = standardFont;
        titleTxt.text = "STAGE CLEARED!";
        titleTxt.fontSize = 32;
        titleTxt.fontStyle = FontStyle.Bold;
        titleTxt.color = new Color(1f, 0.85f, 0.3f);
        titleTxt.alignment = TextAnchor.MiddleCenter;

        // Subtitle message
        GameObject subObj = new GameObject("SubtitleText", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        subObj.transform.SetParent(boxObj.transform, false);
        RectTransform subRt = subObj.GetComponent<RectTransform>();
        subRt.anchorMin = new Vector2(0f, 1f);
        subRt.anchorMax = new Vector2(1f, 1f);
        subRt.pivot = new Vector2(0.5f, 1f);
        subRt.anchoredPosition = new Vector2(0f, -75f);
        subRt.sizeDelta = new Vector2(0f, 35f);

        Text subTxt = subObj.GetComponent<Text>();
        subTxt.font = standardFont;
        subTxt.text = "All required dishes have been cooked!";
        subTxt.fontSize = 18;
        subTxt.fontStyle = FontStyle.Normal;
        subTxt.color = Color.white;
        subTxt.alignment = TextAnchor.MiddleCenter;

        // Action Buttons
        nextLevelButton = CreatePopupButton(boxObj.transform, "NextLevelButton", "NEXT LEVEL", new Vector2(0f, -140f), standardFont, new Color(0.2f, 0.6f, 0.3f), OnNextLevelClicked);
        levelSelectButton = CreatePopupButton(boxObj.transform, "LevelSelectButton", "LEVEL SELECT", new Vector2(0f, -210f), standardFont, new Color(0.25f, 0.35f, 0.55f), OnLevelSelectClicked);
        restartButton = CreatePopupButton(boxObj.transform, "RestartButton", "COOK AGAIN", new Vector2(0f, -280f), standardFont, new Color(0.45f, 0.25f, 0.25f), OnRestartClicked);

        victoryPanel.SetActive(false);
    }

    private Button CreatePopupButton(Transform parent, string name, string label, Vector2 anchoredPos, Font font, Color baseColor, UnityEngine.Events.UnityAction action)
    {
        GameObject btnObj = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        btnObj.transform.SetParent(parent, false);

        RectTransform rt = btnObj.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 1f);
        rt.anchorMax = new Vector2(0.5f, 1f);
        rt.pivot = new Vector2(0.5f, 1f);
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = new Vector2(300f, 50f);

        Image img = btnObj.GetComponent<Image>();
        img.color = baseColor;

        Button btn = btnObj.GetComponent<Button>();
        ColorBlock cb = btn.colors;
        cb.highlightedColor = baseColor * 1.25f;
        cb.pressedColor = baseColor * 0.8f;
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
