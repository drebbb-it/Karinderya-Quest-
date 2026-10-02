using UnityEngine;

public class TimerController : MonoBehaviour
{
    [Tooltip("Stage time limit in seconds")]
    [SerializeField] private float _stageDurationSeconds = 120f;
    private float _secondsRemaining;
    private bool _isRunning;

    private void OnEnable()
    {
        GameEvents.OnInventoryComplete += HandleInventoryComplete;

    }
    private void OnDisable()
    {
        GameEvents.OnInventoryComplete -= HandleInventoryComplete;
    }
    private void Start()
    {
        _secondsRemaining = _stageDurationSeconds;
        _isRunning = true;
        GameEvents.RaiseTimerTick(_secondsRemaining);
    }

    // Update is called once per frame
    private void Update()
    {
        if(!_isRunning)
        {
            return;
        }
        _secondsRemaining -= Time.deltaTime;
        if (_secondsRemaining <= 0f)
        {
            _secondsRemaining = 0f;
            _isRunning = false;
            GameEvents.RaiseTimerTick(_secondsRemaining);
            GameEvents.RaiseTimerExpired();
            return;

        }
        GameEvents.RaiseTimerTick(_secondsRemaining);
    }
    private void HandleInventoryComplete()
    {
        _isRunning = false;
    }
}
