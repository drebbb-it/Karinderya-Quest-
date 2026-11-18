using UnityEngine;

public class PlasticContainer : MonoBehaviour
{
        private const float PlasticSfxDuration = 1f;

        public string currentFood = "";
        public Sprite emptySprite; 
        public Sprite filledSprite;
        public Sprite adoboBagSprite;
        public Sprite afritadaBagSprite;
        public Sprite ginilingBagSprite;
        public Sprite menudoBagSprite;
        public bool isFilled;
        [SerializeField] private AudioClip rejectionSound;
        [SerializeField] private AudioClip plasticSound;

        SpriteRenderer sr;
        Camera cam;
        Vector3 offset;
        Vector3 startPos;
        bool dragging;

        void Awake()
        {
            sr = GetComponent<SpriteRenderer>();
            if (GetComponent<SpriteHighlight>() == null)
            {
                gameObject.AddComponent<SpriteHighlight>();
            }
        }
        void Start()
        {
            cam = Camera.main;
            startPos = transform.position;
            sr.sprite = emptySprite;
        }
        

        public void Fill(string food)
        {
            if (isFilled) return;
            isFilled = true;
            currentFood = food;
            sr.sprite = GetBagSprite(food);
            PlayPlasticSound();
        }

        private Sprite GetBagSprite(string food)
        {
            Sprite bagSprite;
            switch (food)
            {
                case "Adobo":
                    bagSprite = adoboBagSprite;
                    break;
                case "Afritada":
                    bagSprite = afritadaBagSprite;
                    break;
                case "Giniling":
                    bagSprite = ginilingBagSprite;
                    break;
                case "Menudo":
                    bagSprite = menudoBagSprite;
                    break;
                default:
                    bagSprite = null;
                    break;
            }

            if (bagSprite == null)
            {
                Debug.LogError($"PlasticContainer has no bag sprite configured for '{food}'.", this);
                return filledSprite;
            }

            return bagSprite;
        }
        void OnMouseUp()
        {
            if (PauseManager.IsPaused || PauseManager.IsGameOver) return;
            if (!dragging) return;
            dragging = false;
            
            var hit = Physics2D.OverlapPoint(transform.position, LayerMask.GetMask("NPC"));
            if (hit != null)
            {
                var npc = hit.GetComponent<NPC>();
                if (npc != null)
                {
                    var result = npc.Serve(currentFood);
                    if (result == NPC.ServeResult.Wrong)
                    {
                        ButtonSFXUtility.PlayOneShot(rejectionSound);
                        if (NPCManager.Instance != null)
                        {
                            NPCManager.Instance.RegisterWrongOrder();
                        }
                        else
                        {
                            Debug.LogError("PlasticContainer could not find the Stage 3 NPCManager to register the wrong order.", this);
                        }
                    }

                    if (result != NPC.ServeResult.NotReady)
                    {
                        isFilled = false;
                        currentFood = "";
                        sr.sprite = emptySprite;
                    }
                } 
            }
            transform.position = startPos;
        }
        void OnMouseDown()
        {
            if (PauseManager.IsPaused || PauseManager.IsGameOver) return;
            if (!isFilled) return;
            offset = transform.position - MouseWorld();
            dragging = true;
        }

        private void PlayPlasticSound()
        {
            ButtonSFXUtility.PlayBriefly(plasticSound, PlasticSfxDuration);
        }
        void OnMouseDrag()
        {
            if (PauseManager.IsPaused || PauseManager.IsGameOver) return;
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


