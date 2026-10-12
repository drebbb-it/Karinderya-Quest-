using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Builds one row per required category and updates its count as items are collected.
/// </summary>
public class InventoryUI : MonoBehaviour
{
    [Serializable]
    private struct CategoryLabel
    {
        public IngredientCategory category;
        public string label;
    }

    [SerializeField] private InventoryController _inventory;
    [SerializeField] private InventoryRowUI _rowPrefab;
    [SerializeField] private Transform _rowContainer;

    [Tooltip("Optional display names, e.g. Fruit -> Prutas. Missing entries use the enum name.")]
    [SerializeField] private List<CategoryLabel> _labelOverrides = new List<CategoryLabel>();

    private readonly Dictionary<IngredientCategory, InventoryRowUI> _rows =
        new Dictionary<IngredientCategory, InventoryRowUI>();

    private void Start()
    {
        if (_inventory == null || _rowPrefab == null || _rowContainer == null)
        {
            Debug.LogError("InventoryUI: assign _inventory, _rowPrefab and _rowContainer.", this);
            return;
        }

        foreach (CategoryRequirement requirement in _inventory.Requirements)
        {
            IngredientCategory category = requirement.category;
            if (_rows.ContainsKey(category)) continue;

            InventoryRowUI row = Instantiate(_rowPrefab, _rowContainer);
            row.Initialize(GetLabel(category), _inventory.GetRequiredCount(category));
            _rows.Add(category, row);
        }
    }

    private void OnEnable()
    {
        GameEvents.OnInventoryProgress += HandleInventoryProgress;
    }

    private void OnDisable()
    {
        GameEvents.OnInventoryProgress -= HandleInventoryProgress;
    }

    private string GetLabel(IngredientCategory category)
    {
        foreach (CategoryLabel entry in _labelOverrides)
        {
            if (entry.category == category && !string.IsNullOrEmpty(entry.label)) return entry.label;
        }
        return category.ToString();
    }

    private void HandleInventoryProgress(IngredientCategory category, int collected, int required)
    {
        if (_rows.TryGetValue(category, out InventoryRowUI row))
        {
            row.SetProgress(collected, required);
        }
    }
}