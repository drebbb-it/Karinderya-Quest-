using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// PotIngredientsPaperUI — displays the list of ingredients currently in the cooking pot.
/// Supports both TextMeshProUGUI and legacy Text, wirable directly in the Inspector.
/// </summary>
public class PotIngredientsPaperUI : MonoBehaviour
{
    public static PotIngredientsPaperUI Instance { get; private set; }

    [Header("TextMeshPro UI References")]
    [SerializeField] private TextMeshProUGUI tmpHeaderText;
    [SerializeField] private TextMeshProUGUI tmpListText;

    [Header("Legacy UI References (Fallback)")]
    [SerializeField] private Text headerText;
    [SerializeField] private Text listText;

    [Header("Optional Paper Image")]
    [SerializeField] private Image paperImage;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Auto-find text components if not assigned
        if (tmpListText == null && listText == null)
        {
            var tmps = GetComponentsInChildren<TextMeshProUGUI>(true);
            if (tmps.Length > 0)
            {
                if (tmps.Length >= 2)
                {
                    tmpHeaderText = tmps[0];
                    tmpListText = tmps[1];
                }
                else
                {
                    tmpListText = tmps[0];
                }
            }
        }
    }

    private void Start()
    {
        CookingPot.OnIngredientsChanged += UpdateList;

        if (CookingPot.Instance != null)
            UpdateList(CookingPot.Instance.ingredientsInPot);
        else
            UpdateList(new List<string>());
    }

    private void OnDestroy()
    {
        CookingPot.OnIngredientsChanged -= UpdateList;

        if (Instance == this)
            Instance = null;
    }

    public void UpdateList(List<string> ingredients)
    {
        int count = ingredients != null ? ingredients.Count : 0;
        string headerString = $"IN POT ({count})";

        if (tmpHeaderText != null)
            tmpHeaderText.text = headerString;
        else if (headerText != null)
            headerText.text = headerString;

        bool isEmpty = ingredients == null || ingredients.Count == 0;
        string contentString;

        if (isEmpty)
        {
            contentString = "<i>(Pot is empty)</i>\n\nDrag ingredients\nhere to cook!";
        }
        else
        {
            Dictionary<string, int> counts = new Dictionary<string, int>();
            foreach (string item in ingredients)
            {
                string key = FormatIngredientName(item);
                if (counts.ContainsKey(key))
                    counts[key]++;
                else
                    counts[key] = 1;
            }

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            foreach (var pair in counts)
            {
                if (pair.Value > 1)
                    sb.AppendLine($"• {pair.Key} <b>x{pair.Value}</b>");
                else
                    sb.AppendLine($"• {pair.Key}");
            }
            contentString = sb.ToString().TrimEnd();
        }

        if (tmpListText != null)
        {
            tmpListText.text = contentString;
            tmpListText.color = isEmpty ? new Color(0.48f, 0.40f, 0.32f) : new Color(0.20f, 0.12f, 0.06f);
        }
        else if (listText != null)
        {
            listText.text = contentString;
            listText.color = isEmpty ? new Color(0.48f, 0.40f, 0.32f) : new Color(0.20f, 0.12f, 0.06f);
        }
    }

    private string FormatIngredientName(string rawName)
    {
        if (string.IsNullOrEmpty(rawName)) return rawName;

        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        for (int i = 0; i < rawName.Length; i++)
        {
            if (i > 0 && char.IsUpper(rawName[i]) && !char.IsWhiteSpace(rawName[i - 1]))
                sb.Append(' ');
            sb.Append(rawName[i]);
        }
        return sb.ToString();
    }
}
