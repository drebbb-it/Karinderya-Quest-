using UnityEngine;


[RequireComponent(typeof(Collider2D))]
public class IngredientPickup : MonoBehaviour
{
    [SerializeField] private IngredientData _ingredientData;
    [SerializeField] private string _playerTag = "Player";

    private bool _isClaimed;
    private void Reset()
    {
        // Ensure the collider is a trigger by default so this doesn't block movement.
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isClaimed || !other.CompareTag(_playerTag))
        {
            return;
        }

        _isClaimed = true;
        GameEvents.RaiseIngredientsCollected(_ingredientData.IngredientId);
        gameObject.SetActive(false);
    }
    public bool TrySnatch()
    {
        if (_isClaimed)
        {
            return false;
        }

        _isClaimed = true;
        GameEvents.RaiseIngredientStolen(_ingredientData.IngredientId);
        gameObject.SetActive(false);
        return true;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
