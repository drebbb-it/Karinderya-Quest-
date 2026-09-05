using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    [Tooltip("Number of lives the player starts with")]
    [SerializeField] private int _startingLives = 3;

    [Tooltip("Seconds of invincibility after being hit, to prevent multiple hits in quick succession")]
    [SerializeField] private float _hitInvincibilityDuration = 1f;

    private int _currentLives;
    private float _invincibilityTimer;
    private bool _isInvincible;
    public int CurrentLives => _currentLives;
    
    private void OnEnable()
    {
        GameEvents.OnPlayerHitByVehicle += HandleVehicleHit;

    }
    private void OnDisable()
    {
        GameEvents.OnPlayerHitByVehicle -= HandleVehicleHit;
    }
    private void Start()
    {
        _currentLives = _startingLives;
    }

    // Update is called once per frame
    private void Update()
    {
        if (!_isInvincible)
        {
            return;
        }
        _invincibilityTimer -= Time.deltaTime;
        if (_invincibilityTimer <= 0f)
        {
            _isInvincible = false;
        }
    }
    private void HandleVehicleHit()
    {
        if (_isInvincible || _currentLives <= 0)
        {
            return;
        }

        _currentLives--;
        _isInvincible = true;
        _invincibilityTimer = _hitInvincibilityDuration;

        if (_currentLives <= 0)
        {
            GameEvents.RaisePlayerOutOfLives();
        }
        
    }
}
