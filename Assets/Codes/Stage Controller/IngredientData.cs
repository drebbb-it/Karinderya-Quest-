using UnityEngine;

[CreateAssetMenu(fileName = "IngredientData", menuName = "Scriptable Objects/IngredientData")]
public class IngredientData : ScriptableObject
{
    [Tooltip("Unique identifier for inventory tracking. Must be unique across all IngredientData assets ")]
    [SerializeField] private string _ingredientId;

    [SerializeField] private string _displayName;
    [SerializeField] private Sprite _icon;
    [SerializeField] private IngredientCategory _category;

    public string IngredientId => _ingredientId;
    public string DisplayName => _displayName;
    public Sprite Icon => _icon;
    public IngredientCategory Category => _category;

}

public enum IngredientCategory
{
    Fruit,
    Vegetable,
    Meat,
    Fish,
    Rice
}
