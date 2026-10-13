using UnityEngine;
using System.Collections;

public class NPCManager : MonoBehaviour
{
    public NPC[] npcs;                // ayon sa pagkakasunod-sunod
    public float delayBetween = 1.5f; // hintay bago lumabas ang susunod

    IEnumerator Start()
    {
        foreach (var npc in npcs)
        {
            yield return new WaitForSeconds(delayBetween);
            npc.Show();
            yield return new WaitUntil(() => npc == null);
        }

        Debug.Log("All customers served!");
    }
}