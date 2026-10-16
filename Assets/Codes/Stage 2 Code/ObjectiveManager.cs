using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ObjectiveManager : MonoBehaviour
{
    public static ObjectiveManager Instance;

    [System.Serializable]
    public class Objective
    {
        public string dishName;
        public int targetAmount;

        [HideInInspector]
        public int currentAmount;
    }

    [Header("Objectives")]
    public List<Objective> objectives = new List<Objective>();

    [Header("UI")]
    public TextMeshProUGUI objectiveText;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        UpdateUI();
    }

    public void DishCooked(string dishName)
    {
        foreach (Objective objective in objectives)
        {
            if (objective.dishName == dishName)
            {
                objective.currentAmount++;

                if (objective.currentAmount > objective.targetAmount)
                {
                    objective.currentAmount = objective.targetAmount;
                }

                UpdateUI();

                // NEW
                if (AllObjectivesCompleted())
                {
                    Debug.Log("Level Complete!");

                    LevelUnlockManager.UnlockLevel3();
                }

                return;
            }
        }
    }

    private void UpdateUI()
    {
        objectiveText.text = "";

        foreach (Objective objective in objectives)
        {
            bool completed =
                objective.currentAmount >= objective.targetAmount;

            string status = completed ? "✅" : "❌";

            objectiveText.text +=
                $"{status} {objective.currentAmount}/{objective.targetAmount} {objective.dishName}\n";
        }
    }

    public bool AllObjectivesCompleted()
    {
        foreach (Objective objective in objectives)
        {
            if (objective.currentAmount < objective.targetAmount)
            {
                return false;
            }
        }

        return true;
    }
}