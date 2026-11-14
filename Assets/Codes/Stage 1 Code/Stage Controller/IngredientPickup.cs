using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class IngredientPickup : MonoBehaviour
{
    [Header("Collectible Settings")]
    [SerializeField] private IngredientData _ingredientData;
    [SerializeField] private string _playerTag = "Player";
    [SerializeField] private AudioClip _collectSound;

    [Header("Animation Settings")]
    [Tooltip("Enable gentle bobbing (floating up and down)")]
    [SerializeField] private bool _enableBobbing = true;
    [Tooltip("Distance moved up and down")]
    [SerializeField] private float _bobHeight = 0.2f;
    [Tooltip("Speed of the bobbing cycle")]
    [SerializeField] private float _bobFrequency = 2.2f;

    [Tooltip("Enable smooth spin / flip animation")]
    [SerializeField] private bool _enableSpin = true;
    [Tooltip("Speed of horizontal spin (revolutions/second multiplier)")]
    [SerializeField] private float _spinSpeed = 1.8f;

    private bool _isClaimed;
    private Vector3 _startPosition;
    private Vector3 _baseScale;
    private float _timeOffset;

    private void Reset()
    {
        // Ensure the collider is a trigger by default so this doesn't block movement.
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void Awake()
    {
        _startPosition = transform.position;
        _baseScale = transform.localScale;
        // Randomize slight offset so multiple items on screen don't bob/spin in robotic lockstep
        _timeOffset = Random.Range(0f, 6.28f);
    }

    private void Update()
    {
        float t = Time.time + _timeOffset;

        // 1. Bobbing movement (slight up and down)
        if (_enableBobbing)
        {
            float yOffset = Mathf.Sin(t * _bobFrequency) * _bobHeight;
            transform.position = new Vector3(_startPosition.x, _startPosition.y + yOffset, _startPosition.z);
        }

        // 2. Spinning animation (smooth 2D horizontal flip)
        if (_enableSpin)
        {
            // Cosine creates a natural pseudo-3D coin-like spin around the vertical axis
            float xCos = Mathf.Cos(t * _spinSpeed);
            transform.localScale = new Vector3(_baseScale.x * xCos, _baseScale.y, _baseScale.z);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isClaimed || !other.CompareTag(_playerTag))
        {
            return;
        }
        if (_ingredientData == null)
        {
            Debug.LogError($"IngredientPickup {name} has no IngredientData assigned.");
            return;
        }
        _isClaimed = true;
        if (_collectSound != null)
        {
            AudioSource playerAudioSource = other.GetComponentInParent<AudioSource>();
            if (playerAudioSource != null)
            {
                playerAudioSource.PlayOneShot(_collectSound);
            }
            else
            {
                Debug.LogError($"IngredientPickup {name} could not find an AudioSource on the collecting player.", other);
            }
        }
        GameEvents.RaiseIngredientsCollected(_ingredientData);
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
}
