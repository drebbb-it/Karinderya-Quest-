using UnityEngine;

public class Collectible2D : MonoBehaviour
{
    [SerializeField] private string targetTag = "Player";

    [Header("Animation Settings")]
    [SerializeField] private bool _enableBobbing = true;
    [SerializeField] private float _bobHeight = 0.2f;
    [SerializeField] private float _bobFrequency = 2.2f;

    [SerializeField] private bool _enableSpin = true;
    [SerializeField] private float _spinSpeed = 1.8f;

    private Vector3 _startPosition;
    private Vector3 _baseScale;
    private float _timeOffset;

    private void Awake()
    {
        _startPosition = transform.position;
        _baseScale = transform.localScale;
        _timeOffset = Random.Range(0f, 6.28f);
    }

    private void Update()
    {
        float t = Time.time + _timeOffset;

        if (_enableBobbing)
        {
            float yOffset = Mathf.Sin(t * _bobFrequency) * _bobHeight;
            transform.position = new Vector3(_startPosition.x, _startPosition.y + yOffset, _startPosition.z);
        }

        if (_enableSpin)
        {
            float xCos = Mathf.Cos(t * _spinSpeed);
            transform.localScale = new Vector3(_baseScale.x * xCos, _baseScale.y, _baseScale.z);
        }
    }

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
