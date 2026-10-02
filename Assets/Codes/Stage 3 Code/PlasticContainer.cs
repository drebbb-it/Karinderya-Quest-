using UnityEngine;

public class PlasticContainer : MonoBehaviour
{
    
        public Sprite emptySprite; 
        public Sprite filledSprite;
        public bool isFilled;

        SpriteRenderer sr;

        void Awake() => sr = GetComponent<SpriteRenderer>();

        public void Fill()
        {
            if (isFilled) return;
            isFilled = true;
            sr.sprite = filledSprite;
        }
 }


