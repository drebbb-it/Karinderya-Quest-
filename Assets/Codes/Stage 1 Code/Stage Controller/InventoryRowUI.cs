using TMPro;
using UnityEngine;

/// <summary>
/// One checklist row: category label on the left, "collected/required" on the right.
/// </summary>
public class InventoryRowUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _countText;
    [Tooltip("Optional. Shown when the requirement is complete.")]
    [SerializeField] private GameObject _checkMark;

    [Header("Look")]
    [SerializeField] private Color _pendingColor = Color.white;
    [SerializeField] private Color _completeColor = new Color(0.6f, 1f, 0.6f, 1f);
    [SerializeField] private bool _useStrikethrough = true;

    public void Initialize(string categoryLabel, int required)
    {
        _nameText.text = categoryLabel;
        SetProgress(0, required);
    }

    public void SetProgress(int collected, int required)
    {
        bool isComplete = collected >= required;
        Color color = isComplete ? _completeColor : _pendingColor;

        _countText.text = $"{collected}/{required}";
        _countText.color = color;
        _nameText.color = color;

        if (_useStrikethrough && isComplete) _nameText.fontStyle |= FontStyles.Strikethrough;
        else _nameText.fontStyle &= ~FontStyles.Strikethrough;

        if (_checkMark != null) _checkMark.SetActive(isComplete);
    }
}