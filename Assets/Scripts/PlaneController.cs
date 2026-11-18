using UnityEngine;
using UnityEngine.InputSystem;

public class PlaneController : MonoBehaviour
{
    public int step = 0;        // -3 bis 3  (minus = links gekippt, plus = rechts gekippt)
    public int maxStep = 3;

    // Breite vom Flugzeug bei 0°, 30°, 60°, 90°
    public float[] widths = { 4.4f, 3.8f, 2.2f, 0.5f };

    // Später eure Bilder für 0°, 30°, 60°, 90°. Solange leer, wird der Balken skaliert.
    public Sprite[] planeImages;

    public SpriteRenderer planeRenderer;
    public BoxCollider2D planeCollider;

    void Start()
    {
        // Sonst "schläft" das Flugzeug ein und merkt die Wolken nicht mehr
        GetComponent<Rigidbody2D>().sleepMode = RigidbodySleepMode2D.NeverSleep;

        UpdatePlane();
    }

    void Update()
    {
        bool leftPressed = Keyboard.current.aKey.wasPressedThisFrame;
        bool rightPressed = Keyboard.current.dKey.wasPressedThisFrame;

        if (leftPressed || rightPressed)
        {
            bool canTilt = GameManager.instance.ButtonPressed();

            if (canTilt)
            {
                if (rightPressed) step++;
                else step--;

                step = Mathf.Clamp(step, -maxStep, maxStep);
                UpdatePlane();
            }
        }
    }

    void UpdatePlane()
    {
        int index = Mathf.Abs(step);

        planeCollider.size = new Vector2(widths[index], planeCollider.size.y);

        if (planeImages.Length > index && planeImages[index] != null)
        {
            // Echte Bilder: nach links = gespiegelt
            planeRenderer.sprite = planeImages[index];
            planeRenderer.flipX = step < 0;
        }
        else
        {
            // Platzhalter: Balken einfach breiter oder schmaler machen
            Vector3 size = planeRenderer.transform.localScale;
            size.x = widths[index];
            planeRenderer.transform.localScale = size;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        CloudGate gate = other.GetComponentInParent<CloudGate>();

        if (gate != null)
        {
            GameManager.instance.PlaneHitCloud(gate);
        }
    }
}
