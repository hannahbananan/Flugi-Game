using UnityEngine;

// Spielt ein paar Bilder nacheinander ab (z.B. Propeller, Blitz, Rauch)
public class FrameAnimation : MonoBehaviour
{
    public Sprite[] frames;             // max. 3 Bilder
    public float framesPerSecond = 10;
    public bool loop = true;
    public bool destroyWhenDone = false;

    SpriteRenderer spriteRenderer;
    float timer = 0;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (frames.Length == 0) return;

        timer += Time.deltaTime;
        int frame = (int)(timer * framesPerSecond);

        if (frame >= frames.Length)
        {
            if (loop)
            {
                frame = frame % frames.Length;
            }
            else if (destroyWhenDone)
            {
                Destroy(gameObject);
                return;
            }
            else
            {
                frame = frames.Length - 1;
            }
        }

        spriteRenderer.sprite = frames[frame];
    }
}
