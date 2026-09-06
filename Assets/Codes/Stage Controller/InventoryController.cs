using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    [Tooltip("The full shopping list required to clear this stage")]
    [SerializeField] private List<IngredientData>  _requiredIngredients = new List<IngredientData>();
    
    private readonly HashSet<string> _collectedIds = new HashSet<string>();
    private readonly HashSet<string> _lostIds = new HashSet<string>();
    public IReadOnlyCollection<string> CollectedIds => _collectedIds;
    public IReadOnlyCollection<string> LostIds => _lostIds;

    private void OnEnable()
    {
        GameEvents.OnIngredientCollected += HandleIngredientCollected;
        GameEvents.OnIngredientStolen += HandleIngredientStolen;

    }
    private void OnDisable()
    {
        GameEvents.OnIngredientCollected -= HandleIngredientCollected;
        GameEvents.OnIngredientStolen -= HandleIngredientStolen;
    }

    private void HandleIngredientCollected(string ingredientId)
    {
        if (!isRequiredIngredient(ingredientId) || _collectedIds.Contains(ingredientId))
        {
            return;
        }

        _collectedIds.Add(ingredientId);
        CheckCompletion();   
    }

    private void HandleIngredientStolen(string ingredientId)
    {
        if (!isRequiredIngredient(ingredientId) || _collectedIds.Contains(ingredientId))
        {
            return;
        }
        // lost ingredients 
        _lostIds.Add(ingredientId);
    }
    private bool isRequiredIngredient(string ingredientId)
    {
        for(int i = 0; i < _requiredIngredients.Count; i++)
        {
            if(_requiredIngredients[i].IngredientId == ingredientId)
            {
             return true;;
            }
        }
        return false;
    }
    private void CheckCompletion()
    {
        if(_collectedIds.Count >= _requiredIngredients.Count)
        {
            GameEvents.RaiseInventoryComplete();
        }
    }
}   