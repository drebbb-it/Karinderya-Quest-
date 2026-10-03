using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class TrafficHazard : MonoBehaviour
{
    [Header("Movement")]
    [Tooltip("Speed the vehicle travels along its lane")]
    [SerializeField] private float _moveSpeed = 4f;

    [Tooltip("How far the vehicle travels from its start position before reversing.")]
    [SerializeField] private float _travelDistance = 6f;

    [Tooltip("If true, vehicle starts moving in the negative X direction.")]
    [SerializeField] private bool _startMovingLeft;

    [Header("Collision")]
    [SerializeField] private string _playerTag = "Player";
    private Rigidbody2D _rigidbody;
    private Vector2 _startPosition;
    private int _direction;
    
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _rigidbody.bodyType = RigidbodyType2D.Kinematic;
    }
    
    void Start()
    {
        _startPosition = transform.position;
        _direction = _startMovingLeft ? -1 : 1;
    }

    private void FixedUpdate()
    {
        float distanceFromStart = Vector2.Distance(_rigidbody.position, _startPosition);
        if (distanceFromStart >= _travelDistance)
        {
            _direction *= -1; // Reverse direction
            Vector2 clampedPosition = _startPosition  + new  Vector2(_travelDistance * Mathf.Sign(_rigidbody.position.x - _startPosition.x), 0f);
            _rigidbody.position = clampedPosition; // Clamp position to travel distance
        }
        Vector2 nextPosition = _rigidbody.position + new Vector2(_direction * _moveSpeed  * Time.fixedDeltaTime, 0f);
        _rigidbody.MovePosition(nextPosition);

    }

    private void OnTriggerEnter2d(Collider2D other)
    {
        if(!other.CompareTag(_playerTag)) 
        {
            return;
        }
        GameEvents.RaisePlayerHitByVehicle();   
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
