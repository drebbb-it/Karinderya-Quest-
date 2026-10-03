using UnityEngine;

public class Collectible2D : MonoBehaviour
{
    [SerializeField] private string targetTag = "Player";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the object entering the trigger has the "Player" tag
        if (collision.CompareTag(targetTag))
        {
            // Destroy this collectible object
            Destroy(gameObject);
        }
    }
}   