using UnityEngine;
using System.Collections;

public class NPC : MonoBehaviour
{
    public bool served;
    public float hideOffset = 4f;   // how far below the final spot it starts
    public float moveTime = 0.5f;   // rise and sink duration
    public float stayTime = 0.8f;   // wait after being served before leaving

    bool canServe;
    Vector3 shownPos, hiddenPos;

    void Start()
    {
        shownPos = transform.position;               // where you placed it in the Scene
        hiddenPos = shownPos + Vector3.down * hideOffset;
        transform.position = hiddenPos;
        StartCoroutine(Appear());
    }

    public bool Serve()
    {
        if (!canServe || served) return false;
        served = true;
        canServe = false;
        Debug.Log(name + " was served!");
        StartCoroutine(Leave());
        return true;
    }

    IEnumerator Appear()
    {
        yield return Move(hiddenPos, shownPos);
        canServe = true;
    }

    IEnumerator Leave()
    {
        yield return new WaitForSeconds(stayTime);
        yield return Move(shownPos, hiddenPos);   // sink back down
        Destroy(gameObject);
    }

    IEnumerator Move(Vector3 from, Vector3 to)
    {
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / moveTime;
            transform.position = Vector3.Lerp(from, to, Mathf.SmoothStep(0, 1, t));
            yield return null;
        }
        transform.position = to;
    }
}