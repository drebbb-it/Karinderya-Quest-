using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

/// <summary>
/// Shows the Stage 2-style victory popup when the inventory is complete.
/// </summary>
public class VictoryScreen : MonoBehaviour
{
    [Header("Legacy UI")]
    [SerializeField] private GameObject _victoryPanel;

    [Header("Scenes (must be added to Build Profiles > Scenes In Build)")]
    [SerializeField] private string _nextSceneName = "Stage2Scene";

    private bool _isShown;

    private void Awake()
    {
        Time.timeScale = 1f; // recover from a previous frozen run
        if (_victoryPanel != null) _victoryPanel.SetActive(false);
    }

    private void OnEnable()
    {
        GameEvents.OnInventoryComplete += HandleInventoryComplete;
    }

    private void OnDisable()
    {
        GameEvents.OnInventoryComplete -= HandleInventoryComplete;
        Time.timeScale = 1f;
    }

    private void HandleInventoryComplete()
    {
        if (_isShown) return;
        _isShown = true;

        LevelUnlockManager.UnlockLevel2();
        PauseManager.LockForVictory();
        GameObject victoryPanel = StageVictoryPopup.Create(
            "STAGE CLEARED!",
            "All ingredients have been collected!",
            new[] { "NEXT LEVEL", "LEVEL SELECT", "PLAY AGAIN" },
            new UnityAction[]
            {
                OnNextLevelClicked,
                OnLevelSelectClicked,
                OnRestartClicked
            },
            new[]
            {
                new Color(0.2f, 0.6f, 0.3f),
                new Color(0.25f, 0.35f, 0.55f),
                new Color(0.45f, 0.25f, 0.25f)
            });
        victoryPanel.SetActive(true);
    }

    private void OnNextLevelClicked() => LoadScene(_nextSceneName);

    private void OnLevelSelectClicked() => LoadScene("LevelSelection");

    private void OnRestartClicked() => LoadScene(SceneManager.GetActiveScene().name);

    private void LoadScene(string sceneName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }
}