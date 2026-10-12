using UnityEngine;

public class LevelUnlockManager : MonoBehaviour
{
    public static void UnlockLevel2()
    {
        PlayerPrefs.SetInt("Level2Unlocked", 1);
        PlayerPrefs.Save();
    }

    public static void UnlockLevel3()
    {
        PlayerPrefs.SetInt("Level3Unlocked", 1);
        PlayerPrefs.Save();
    }
}