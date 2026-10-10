using UnityEngine;

public class IngredientSpawner : MonoBehaviour
{
    public GameObject ingredientPrefab;

    private Vector3 spawnPosition;
    private Quaternion spawnRotation;
    private Vector3 spawnScale;
    private Transform originalParent;

    private void Start()
    {
        spawnPosition = transform.localPosition;
        spawnRotation = transform.localRotation;
        spawnScale = transform.localScale;
        originalParent = transform.parent;
    }

    public void RespawnIngredient()
    {
        GameObject newIngredient =
            Instantiate(ingredientPrefab, originalParent);

        newIngredient.transform.localPosition = spawnPosition;
        newIngredient.transform.localRotation = spawnRotation;
        newIngredient.transform.localScale = spawnScale;
    }
}