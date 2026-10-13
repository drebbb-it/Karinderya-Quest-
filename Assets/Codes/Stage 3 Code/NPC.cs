using UnityEngine;
using System.Collections;
using TMPro;

public class NPC : MonoBehaviour
{
    public enum ServeResult { NotReady, Wrong, Correct }

    public bool served;
    public float hideOffset = 4f;
    public float moveTime = 0.5f;
    public float stayTime = 0.8f;

    [Header("Order")]
    public string[] possibleOrders = { "Adobo", "Afritada", "Giniling", "Menudo" };
    public string wantedFood;
    public GameObject orderBubble;    // child object na may text
    public TextMeshPro orderText;

    bool canServe;
    Vector3 shownPos, hiddenPos;

    void Awake()
    {
        shownPos = transform.position;
        hiddenPos = shownPos + Vector3.down * hideOffset;
        transform.position = hiddenPos;

        wantedFood = possibleOrders[Random.Range(0, possibleOrders.Length)];
        if (orderText != null) orderText.text = wantedFood;

        SetBubble(false);
    }

    void SetBubble(bool on)
    {
        if (orderBubble != null) orderBubble.SetActive(on);
        else if (orderText != null) orderText.gameObject.SetActive(on);
    }

    public void Show()
    {
        StartCoroutine(Appear());
    }

    public ServeResult Serve(string food)
    {
        if (!canServe || served) return ServeResult.NotReady;

        if (food != wantedFood)
        {
            Debug.Log(name + " rejected " + food + " (wants " + wantedFood + ")");
            StartCoroutine(Shake());
            return ServeResult.Wrong;
        }

        served = true;
        canServe = false;
        SetBubble(false);
        Debug.Log(name + " was served!");
        StartCoroutine(Leave());
        return ServeResult.Correct;
    }

    IEnumerator Appear()
    {
        yield return Move(hiddenPos, shownPos);

        // Maghihintay muna ng 0.3 seconds pagkatapos umakyat bago lumabas ang order text
        yield return new WaitForSeconds(0.3f);

        canServe = true;
        SetBubble(true);
    }

    IEnumerator Leave()
    {
        yield return new WaitForSeconds(stayTime);
        yield return Move(shownPos, hiddenPos);
        Destroy(gameObject);
    }

    IEnumerator Shake()
    {
        canServe = false;                     // hindi muna tatanggap habang umiiling
        float t = 0;
        while (t < 0.4f)
        {
            t += Time.deltaTime;
            float x = Mathf.Sin(t * 50f) * 0.08f;
            transform.position = shownPos + new Vector3(x, 0, 0);
            yield return null;
        }
        transform.position = shownPos;
        canServe = true;
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