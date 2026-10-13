using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Shows the Victory panel when the inventory is complete, freezes gameplay,
/// and routes the Continue / Exit buttons to their scenes.
/// </summary>
public class VictoryScreen : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject _victoryPanel;
    [SerializeField] private Button _continueButton;
    [SerializeField] private Button _exitButton;

    [Header("Scenes (must be added to Build Profiles > Scenes In Build)")]
    [SerializeField] private string _nextSceneName = "LevelSelection";
    [SerializeField] private string _mainMenuSceneName = "MainMenu";

    private bool _isShown;

    private void Awake()
    {
        Time.timeScale = 1f; // recover from a previous frozen run
        _victoryPanel.SetActive(false);
    }

    private void OnEnable()
    {
        GameEvents.OnInventoryComplete += HandleInventoryComplete;
        _continueButton.onClick.AddListener(OnContinueClicked);
        _exitButton.onClick.AddListener(OnExitClicked);
    }

    private void OnDisable()
    {
        GameEvents.OnInventoryComplete -= HandleInventoryComplete;
        _continueButton.onClick.RemoveListener(OnContinueClicked);
        _exitButton.onClick.RemoveListener(OnExitClicked);
        Time.timeScale = 1f;
    }

    private void HandleInventoryComplete()
    {
        if (_isShown) return;
        _isShown = true;

        _victoryPanel.SetActive(true);
        Time.timeScale = 0f; // freezes physics and vehicles; UI buttons still work
    }

    private void OnContinueClicked() => LoadScene(_nextSceneName);

    private void OnExitClicked() => LoadScene(_mainMenuSceneName);

    private void LoadScene(string sceneName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }
}