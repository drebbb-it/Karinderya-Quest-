using UnityEngine;

public class PlasticContainer : MonoBehaviour
{
    
        public Sprite emptySprite; 
        public Sprite filledSprite;
        public bool isFilled;

        SpriteRenderer sr;
        Camera cam;
        Vector3 offset;
        Vector3 startPos;
        bool dragging;

        void Awake()
        {
            sr = GetComponent<SpriteRenderer>();
        }
        void Start()
        {
            cam = Camera.main;
            startPos = transform.position;
            sr.sprite = emptySprite;
        }
        

        public void Fill()
        {
            if (isFilled) return;
            isFilled = true;
            sr.sprite = filledSprite;
        }
        void OnMouseUp()
    {
        if (!dragging) return;
        dragging = false;

        var hit = Physics2D.OverlapPoint(transform.position, LayerMask.GetMask("NPC"));
        if (hit != null)
        {
            var npc = hit.GetComponent<NPC>();
            if (npc != null && npc.Serve())
            {
                isFilled = false;
                sr.sprite = emptySprite;
            }
        }
    }
        void OnMouseDown()
        {
            if (!isFilled) return;
            offset = transform.position - MouseWorld();
            dragging = true;
        }
        void OnMouseDrag()
        {
            if (dragging)
            {
                transform.position = MouseWorld() + offset;
            }
        }
        Vector3 MouseWorld()
        {
            var p = cam.ScreenToWorldPoint(Input.mousePosition);
            p.z = 0;
            return p;
        }
        


 }


