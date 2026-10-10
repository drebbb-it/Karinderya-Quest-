using UnityEngine;

public class DeathZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        IngredientSpawner spawner =
            other.GetComponent<IngredientSpawner>();

        if (spawner != null)
        {
            spawner.RespawnIngredient();
        }

        Destroy(other.gameObject);
    }
}