using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public static class StageVictoryPopup
{
    public static GameObject Create(
        string title,
        string subtitle,
        string[] buttonLabels,
        UnityAction[] buttonActions,
        Color[] buttonColors)
    {
        if (buttonLabels == null || buttonActions == null || buttonColors == null ||
            buttonLabels.Length != buttonActions.Length ||
            buttonLabels.Length != buttonColors.Length ||
            (buttonLabels.Length != 2 && buttonLabels.Length != 3))
        {
            throw new ArgumentException("A stage result popup requires two or three matching button labels, actions, and colors.");
        }

        GameObject canvasObject = new GameObject("StageVictoryOverlayCanvas");
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.overrideSorting = true;
        canvas.sortingOrder = 32000;

        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1280f, 720f);
        scaler.matchWidthOrHeight = 0.5f;

        canvasObject.AddComponent<GraphicRaycaster>();

        Font standardFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf")
                         ?? Resources.GetBuiltinResource<Font>("Arial.ttf");

        GameObject victoryPanel = new GameObject("VictoryPanel", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        victoryPanel.transform.SetParent(canvas.transform, false);

        RectTransform panelTransform = victoryPanel.GetComponent<RectTransform>();
        panelTransform.anchorMin = Vector2.zero;
        panelTransform.anchorMax = Vector2.one;
        panelTransform.sizeDelta = Vector2.zero;
        victoryPanel.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.75f);

        GameObject box = new GameObject("VictoryBox", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        box.transform.SetParent(victoryPanel.transform, false);

        RectTransform boxTransform = box.GetComponent<RectTransform>();
        boxTransform.anchorMin = new Vector2(0.5f, 0.5f);
        boxTransform.anchorMax = new Vector2(0.5f, 0.5f);
        boxTransform.pivot = new Vector2(0.5f, 0.5f);
        boxTransform.anchoredPosition = Vector2.zero;
        boxTransform.sizeDelta = new Vector2(460f, buttonLabels.Length == 2 ? 320f : 380f);
        box.GetComponent<Image>().color = new Color(0.12f, 0.12f, 0.15f, 0.96f);

        CreateText(box.transform, "TitleText", title, standardFont, 32, FontStyle.Bold,
            new Color(1f, 0.85f, 0.3f), new Vector2(0f, -25f), 45f);
        CreateText(box.transform, "SubtitleText", subtitle, standardFont, 18, FontStyle.Normal,
            Color.white, new Vector2(0f, -75f), 35f);

        Vector2[] buttonPositions = buttonLabels.Length == 2
            ? new[]
            {
                new Vector2(0f, -155f),
                new Vector2(0f, -225f)
            }
            : new[]
        {
            new Vector2(0f, -140f),
            new Vector2(0f, -210f),
            new Vector2(0f, -280f)
        };
        string[] buttonNames = { "PrimaryButton", "SecondaryButton", "TertiaryButton" };

        for (int i = 0; i < buttonLabels.Length; i++)
        {
            CreateButton(box.transform, buttonNames[i], buttonLabels[i], buttonPositions[i],
                standardFont, buttonColors[i], buttonActions[i]);
        }

        victoryPanel.SetActive(false);
        return victoryPanel;
    }

    private static void CreateText(
        Transform parent,
        string name,
        string content,
        Font font,
        int fontSize,
        FontStyle fontStyle,
        Color color,
        Vector2 anchoredPosition,
        float height)
    {
        GameObject textObject = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        textObject.transform.SetParent(parent, false);

        RectTransform textTransform = textObject.GetComponent<RectTransform>();
        textTransform.anchorMin = new Vector2(0f, 1f);
        textTransform.anchorMax = new Vector2(1f, 1f);
        textTransform.pivot = new Vector2(0.5f, 1f);
        textTransform.anchoredPosition = anchoredPosition;
        textTransform.sizeDelta = new Vector2(0f, height);

        Text text = textObject.GetComponent<Text>();
        text.font = font;
        text.text = content;
        text.fontSize = fontSize;
        text.fontStyle = fontStyle;
        text.color = color;
        text.alignment = TextAnchor.MiddleCenter;
    }

    private static void CreateButton(
        Transform parent,
        string name,
        string label,
        Vector2 anchoredPosition,
        Font font,
        Color baseColor,
        UnityAction action)
    {
        GameObject buttonObject = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        buttonObject.transform.SetParent(parent, false);

        RectTransform buttonTransform = buttonObject.GetComponent<RectTransform>();
        buttonTransform.anchorMin = new Vector2(0.5f, 1f);
        buttonTransform.anchorMax = new Vector2(0.5f, 1f);
        buttonTransform.pivot = new Vector2(0.5f, 1f);
        buttonTransform.anchoredPosition = anchoredPosition;
        buttonTransform.sizeDelta = new Vector2(300f, 50f);

        buttonObject.GetComponent<Image>().color = baseColor;

        Button button = buttonObject.GetComponent<Button>();
        ColorBlock colors = button.colors;
        colors.highlightedColor = baseColor * 1.25f;
        colors.pressedColor = baseColor * 0.8f;
        button.colors = colors;

        GameObject textObject = new GameObject("Text", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        textObject.transform.SetParent(buttonObject.transform, false);

        RectTransform textTransform = textObject.GetComponent<RectTransform>();
        textTransform.anchorMin = Vector2.zero;
        textTransform.anchorMax = Vector2.one;
        textTransform.sizeDelta = Vector2.zero;

        Text text = textObject.GetComponent<Text>();
        text.font = font;
        text.text = label;
        text.fontSize = 18;
        text.fontStyle = FontStyle.Bold;
        text.color = Color.white;
        text.alignment = TextAnchor.MiddleCenter;

        button.onClick.AddListener(action);
        ButtonSFXUtility.RegisterButton(button);
    }
}
