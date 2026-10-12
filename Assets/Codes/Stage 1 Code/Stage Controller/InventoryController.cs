using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>One win requirement: collect <see cref="amount"/> items of a category.</summary>
[Serializable]
public struct CategoryRequirement
{
    public IngredientCategory category;
    [Min(1)] public int amount;

    public int RequiredAmount => Mathf.Max(1, amount);
}

/// <summary>
/// Tracks how many items of each required category were collected and raises completion
/// once every requirement is met.
/// </summary>
public class InventoryController : MonoBehaviour
{
    [SerializeField] private List<CategoryRequirement> _requirements = new List<CategoryRequirement>
    {
        new CategoryRequirement { category = IngredientCategory.Fruit, amount = 3 },
        new CategoryRequirement { category = IngredientCategory.Vegetable, amount = 3 },
        new CategoryRequirement { category = IngredientCategory.Meat, amount = 3 }
    };

    private readonly Dictionary<IngredientCategory, int> _requiredCounts = new Dictionary<IngredientCategory, int>();
    private readonly Dictionary<IngredientCategory, int> _collectedCounts = new Dictionary<IngredientCategory, int>();
    private bool _isComplete;

    /// <summary>Configured requirements (read-only, for UI).</summary>
    public IReadOnlyList<CategoryRequirement> Requirements => _requirements;

    public int GetRequiredCount(IngredientCategory category) =>
        _requiredCounts.TryGetValue(category, out int count) ? count : 0;

    public int GetCollectedCount(IngredientCategory category) =>
        _collectedCounts.TryGetValue(category, out int count) ? count : 0;

    private void Awake()
    {
        // Duplicate categories in the list are summed.
        foreach (CategoryRequirement requirement in _requirements)
        {
            _requiredCounts.TryGetValue(requirement.category, out int existing);
            _requiredCounts[requirement.category] = existing + requirement.RequiredAmount;
        }
    }

    private void OnEnable()
    {
        GameEvents.OnIngredientCollected += HandleIngredientCollected;
    }

    private void OnDisable()
    {
        GameEvents.OnIngredientCollected -= HandleIngredientCollected;
    }

    private void HandleIngredientCollected(IngredientData data)
    {
        if (_isComplete || !_requiredCounts.TryGetValue(data.Category, out int required)) return;

        int collected = GetCollectedCount(data.Category);
        if (collected >= required) return; // already have enough of this category

        collected++;
        _collectedCounts[data.Category] = collected;
        GameEvents.RaiseInventoryProgress(data.Category, collected, required);

        foreach (KeyValuePair<IngredientCategory, int> entry in _requiredCounts)
        {
            if (GetCollectedCount(entry.Key) < entry.Value) return;
        }

        _isComplete = true;
        GameEvents.RaiseInventoryComplete();
    }
}