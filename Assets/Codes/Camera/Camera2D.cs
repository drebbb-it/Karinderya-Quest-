using UnityEngine;

public class Camera2D : MonoBehaviour
{   
    [Header("Target")]
    [SerializeField] private Transform _target;

    [Header("Follow Settings")]
    [SerializeField] private float _smoothTime = 0.15f;
    [SerializeField] private Vector2 _offset = Vector2.zero;

    [Header ("Bounds (Optional)")]
    [SerializeField] private bool _useBounds = false;
    [SerializeField] private Vector2 _minBounds;
    [SerializeField] private Vector2 _maxBounds;

    private Vector3 _velocity = Vector3.zero;
    private float _fixedZ;

    private void Awake()
    {
        _fixedZ = transform.position.z;
        if (_target == null)
        {
            Debug.LogWarning($"{nameof(Camera2D)}: No target assigned. Camera will not follow anything.");
        }
    }
    private void LateUpdate()
    {
        if (_target == null) return;

        Vector3 desiredPosition = new Vector3(
            _target.position.x + _offset.x,
            _target.position.y + _offset.y,
            _fixedZ
        );

        if (_useBounds)
        {
            desiredPosition.x = Mathf.Clamp(desiredPosition.x, _minBounds.x, _maxBounds.x);
            desiredPosition.y = Mathf.Clamp(desiredPosition.y, _minBounds.y, _maxBounds.y);
        }
        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPosition,
            ref _velocity,
            _smoothTime
        );
    }
    public void SetTarget(Transform newTarget)
    {
        _target = newTarget;
    }
}
