using UnityEngine;

public class IngredientSpawner : MonoBehaviour
{
    public GameObject ingredientPrefab;

    private Vector3 spawnPosition;
    private Quaternion spawnRotation;

    private void Start()
    {
        spawnPosition = transform.position;
        spawnRotation = transform.rotation;
    }

    public void RespawnIngredient()
    {
        Instantiate(ingredientPrefab, spawnPosition, spawnRotation);
    }
}