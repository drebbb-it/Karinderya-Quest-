using UnityEngine;
using TMPro;
public class TimerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _timerText;
    [SerializeField] private AudioClip _tenSecondTimerSound;

    private AudioSource _timerWarningAudio;
    private bool _tenSecondWarningPlayed;
    private bool _inventoryCompleted;

    private void Awake()
    {
        _timerWarningAudio = GetComponent<AudioSource>();
        if (_timerWarningAudio == null)
        {
            _timerWarningAudio = gameObject.AddComponent<AudioSource>();
        }

        _timerWarningAudio.playOnAwake = false;
        _timerWarningAudio.spatialBlend = 0f;
    }

    private void OnEnable()
    {
        GameEvents.OnTimerTick += HandleTick;
        GameEvents.OnTimerExpired += HandleExpired;
        GameEvents.OnInventoryComplete += HandleInventoryComplete;
    }
    private void OnDisable()
    {
        GameEvents.OnTimerTick -= HandleTick;
        GameEvents.OnTimerExpired -= HandleExpired;
        GameEvents.OnInventoryComplete -= HandleInventoryComplete;
    }
    private void HandleTick(float secondsRemaining)
    {
        if (_inventoryCompleted) return;

        int seconds = Mathf.CeilToInt(secondsRemaining);
        _timerText.text = $"Time: {seconds}";

        if (!_tenSecondWarningPlayed && seconds == 10)
        {
            _tenSecondWarningPlayed = true;
            if (_tenSecondTimerSound == null)
            {
                Debug.LogError("TimerUI: assign a ten-second warning sound.", this);
                return;
            }

            _timerWarningAudio.PlayOneShot(_tenSecondTimerSound);
        }
    }   
    private void HandleInventoryComplete()
    {
        _inventoryCompleted = true;
        _timerWarningAudio.Stop();
    }

    private void HandleExpired()
    {
        _timerText.text = "Time's Up!";
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
