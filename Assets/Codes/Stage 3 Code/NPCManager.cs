using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NPCManager : MonoBehaviour
{
    private const int StartingLives = 3;
    private const int MaximumCustomersAtOnce = 3;
    private const float CustomerSlotGap = 0.25f;

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
    private Vector3 customerSpawnCenter;

    private void Awake()
    {
        Instance = this;
        currentLives = StartingLives;

        List<NPC> allNpcs = new List<NPC>();
        if (npcs != null)
        {
            foreach (NPC npc in npcs)
            {
                if (npc != null && !allNpcs.Contains(npc))
                {
                    allNpcs.Add(npc);
                }
            }
        }

        foreach (NPC npc in FindObjectsByType<NPC>(FindObjectsInactive.Include))
        {
            if (npc != null && !allNpcs.Contains(npc))
            {
                allNpcs.Add(npc);
            }
        }

        if (allNpcs.Count > 0)
        {
            foreach (NPC npc in allNpcs)
            {
                customerSpawnCenter += npc.InitialShownPosition;
            }

            customerSpawnCenter /= allNpcs.Count;
        }

        npcs = allNpcs.ToArray();
        foreach (NPC npc in npcs)
        {
            npc.gameObject.SetActive(false);
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

        int maximumCount = Mathf.Min(MaximumCustomersAtOnce, randomizedNpcs.Count);
        int customerCount = Random.Range(1, maximumCount + 1);

        for (int i = 0; i < randomizedNpcs.Count; i++)
        {
            int swapIndex = Random.Range(i, randomizedNpcs.Count);
            NPC temp = randomizedNpcs[i];
            randomizedNpcs[i] = randomizedNpcs[swapIndex];
            randomizedNpcs[swapIndex] = temp;
        }

        List<NPC> selectedNpcs = randomizedNpcs.GetRange(0, customerCount);
        float slotSpacing = GetCustomerSlotSpacing(selectedNpcs);
        yield return new WaitForSeconds(delayBetween);

        for (int i = 0; i < selectedNpcs.Count; i++)
        {
            NPC npc = selectedNpcs[i];
            if (npc == null) continue;

            float slotOffset = (i - (customerCount - 1) * 0.5f) * slotSpacing;
            npc.gameObject.SetActive(true);
            npc.ShowAt(customerSpawnCenter + Vector3.right * slotOffset);
        }

        yield return new WaitUntil(() => AreAllCustomersServed(selectedNpcs));

        Debug.Log("All customers served!");
        ShowVictoryPopup();
    }

    private float GetCustomerSlotSpacing(List<NPC> selectedNpcs)
    {
        float widestCustomer = 0f;
        foreach (NPC npc in selectedNpcs)
        {
            if (npc == null) continue;

            float horizontalScale = Mathf.Abs(npc.transform.lossyScale.x);
            SpriteRenderer spriteRenderer = npc.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null && spriteRenderer.sprite != null)
            {
                widestCustomer = Mathf.Max(
                    widestCustomer,
                    spriteRenderer.sprite.bounds.size.x * horizontalScale);
            }

            BoxCollider2D collider = npc.GetComponent<BoxCollider2D>();
            if (collider != null)
            {
                widestCustomer = Mathf.Max(
                    widestCustomer,
                    collider.size.x * horizontalScale);
            }
        }

        return Mathf.Max(1f, widestCustomer + CustomerSlotGap);
    }

    private bool AreAllCustomersServed(List<NPC> selectedNpcs)
    {
        foreach (NPC npc in selectedNpcs)
        {
            if (npc != null) return false;
        }

        return true;
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