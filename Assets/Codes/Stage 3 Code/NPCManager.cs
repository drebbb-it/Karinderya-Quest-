using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NPCManager : MonoBehaviour
{
    private const int StartingLives = 3;

    public static NPCManager Instance { get; private set; }

    public NPC[] npcs;                // ayon sa pagkakasunod-sunod
    public float delayBetween = 1.5f; // hintay bago lumabas ang susunod
    [SerializeField] private Sprite heartSprite;
    [SerializeField] private Sprite emptyHeartSprite;
    [SerializeField] private Image[] heartImages;
    private bool completionShown;
    private bool gameOverShown;
    private int currentLives;
    [SerializeField] private AudioClip levelUpSound;

    private void Awake()
    {
        Instance = this;
        currentLives = StartingLives;

        if (npcs != null)
        {
            foreach (NPC npc in npcs)
            {
                if (npc != null)
                {
                    npc.gameObject.SetActive(false);
                }
            }
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    IEnumerator Start()
    {
        UpdateLivesDisplay();

        List<NPC> randomizedNpcs = new List<NPC>();
        if (npcs != null)
        {
            foreach (NPC npc in npcs)
            {
                if (npc == null || randomizedNpcs.Contains(npc)) continue;
                if (!npc.HasPossibleOrders)
                {
                    Debug.LogError("NPCManager found an NPC with no possible orders.", npc);
                    npc.gameObject.SetActive(false);
                    continue;
                }

                randomizedNpcs.Add(npc);
            }
        }

        if (randomizedNpcs.Count == 0)
        {
            Debug.LogError("NPCManager has no valid NPCs assigned.", this);
            yield break;
        }

        int minimumCount = Mathf.Min(2, randomizedNpcs.Count);
        int maximumCount = Mathf.Min(3, randomizedNpcs.Count);
        int customerCount = Random.Range(minimumCount, maximumCount + 1);

        for (int i = 0; i < randomizedNpcs.Count; i++)
        {
            int swapIndex = Random.Range(i, randomizedNpcs.Count);
            NPC temp = randomizedNpcs[i];
            randomizedNpcs[i] = randomizedNpcs[swapIndex];
            randomizedNpcs[swapIndex] = temp;
        }

        for (int i = customerCount; i < randomizedNpcs.Count; i++)
        {
            randomizedNpcs[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < customerCount; i++)
        {
            yield return new WaitForSeconds(delayBetween);
            NPC npc = randomizedNpcs[i];
            if (npc == null) continue;
            npc.gameObject.SetActive(true);
            npc.Show();
            yield return new WaitUntil(() => npc == null);
        }

        Debug.Log("All customers served!");
        ShowVictoryPopup();
    }

    private void ShowVictoryPopup()
    {
        if (completionShown || gameOverShown) return;
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
        ButtonSFXUtility.PlayOneShot(levelUpSound);
    }

    public void RegisterWrongOrder()
    {
        if (currentLives <= 0 || gameOverShown) return;

        currentLives--;
        UpdateLivesDisplay();

        if (currentLives == 0)
        {
            ShowGameOverPopup();
        }
    }

    private void UpdateLivesDisplay()
    {
        if (heartImages == null) return;

        for (int i = 0; i < heartImages.Length; i++)
        {
            if (heartImages[i] != null)
            {
                heartImages[i].sprite = i < currentLives ? heartSprite : emptyHeartSprite;
            }
        }
    }

    private void ShowGameOverPopup()
    {
        if (gameOverShown) return;

        gameOverShown = true;
        StopAllCoroutines();
        PauseManager.LockForGameOver();

        GameObject defeatPanel = StageVictoryPopup.Create(
            "GAME OVER!",
            "You lost all three hearts after incorrect orders.",
            new[] { "PLAY AGAIN", "LEVEL SELECT" },
            new UnityAction[]
            {
                RestartStage,
                LoadLevelSelection
            },
            new[]
            {
                new Color(0.45f, 0.25f, 0.25f),
                new Color(0.25f, 0.35f, 0.55f)
            });
        defeatPanel.SetActive(true);
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
        SFXSceneLoad.LoadSceneAfterDelay(this, sceneName);
    }
}