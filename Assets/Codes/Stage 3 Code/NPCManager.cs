using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class NPCManager : MonoBehaviour
{
    public NPC[] npcs;                // ayon sa pagkakasunod-sunod
    public float delayBetween = 1.5f; // hintay bago lumabas ang susunod
    private bool completionShown;

    IEnumerator Start()
    {
        foreach (var npc in npcs)
        {
            yield return new WaitForSeconds(delayBetween);
            npc.Show();
            yield return new WaitUntil(() => npc == null);
        }

        Debug.Log("All customers served!");
        ShowVictoryPopup();
    }

    private void ShowVictoryPopup()
    {
        if (completionShown) return;
        completionShown = true;

        PauseManager.LockForVictory();

        GameObject popup = StageVictoryPopup.Create(
            "STAGE CLEARED!",
            "All customers have been served!",
            new[] { "LEVEL SELECT", "MAIN MENU", "SERVE AGAIN" },
            new UnityAction[]
            {
                LoadLevelSelection,
                LoadMainMenu,
                RestartStage
            },
            new[]
            {
                new Color(0.2f, 0.6f, 0.3f),
                new Color(0.25f, 0.35f, 0.55f),
                new Color(0.45f, 0.25f, 0.25f)
            });
        popup.SetActive(true);
    }

    private void LoadLevelSelection()
    {
        LoadScene("LevelSelection");
    }

    private void LoadMainMenu()
    {
        LoadScene("MainMenu");
    }

    private void RestartStage()
    {
        LoadScene(SceneManager.GetActiveScene().name);
    }

    private void LoadScene(string sceneName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }
}