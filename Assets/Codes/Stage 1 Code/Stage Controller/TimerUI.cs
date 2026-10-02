using UnityEngine;
using TMPro;
public class TimerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _timerText;
    private void OnEnable()
    {
        GameEvents.OnTimerTick += HandleTick;
        GameEvents.OnTimerExpired += HandleExpired;
    }
    private void OnDisable()
    {
        GameEvents.OnTimerTick -= HandleTick;
        GameEvents.OnTimerExpired -= HandleExpired;
    }
    private void HandleTick(float secondsRemaining)
    {
        int seconds = Mathf.CeilToInt(secondsRemaining);
        _timerText.text = $"Time: {seconds}";
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
