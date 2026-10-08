using UnityEngine;

public class NPC : MonoBehaviour
{
    public bool served;

    public bool Serve()
    {
        if (served) return false;
        served = true;
        Debug.Log(name + " was served!");
        return true;
    }
}