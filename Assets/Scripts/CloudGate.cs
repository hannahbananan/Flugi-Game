using UnityEngine;

public class CloudGate : MonoBehaviour
{
    public int requiredStep = 1;    // 0 = gerade, 1 = 30°, 2 = 60°, 3 = 90°
    public float gateHeight = 3;
    public bool randomMirror = true;
    public GameObject lightning;    // optional

    public bool wasHit = false;
    public int maxStepUsed = 0;

    // Nur für die gelben Hilfslinien im Editor: so breit sollte die Lücke sein
    float[] recommendedGaps = { 5f, 4.1f, 2.6f, 0.9f };

    PlaneController plane;
    bool passed = false;
    float lightningTimer = 2;

    void Start()
    {
        plane = FindFirstObjectByType<PlaneController>();

        if (randomMirror && Random.value < 0.5f)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }

        if (lightning != null) lightning.SetActive(false);
    }

    void Update()
    {
        if (GameManager.instance.state != GameManager.GameState.Playing) return;

        transform.position += Vector3.down * GameManager.instance.speed * Time.deltaTime;

        float distance = transform.position.y - plane.transform.position.y;

        // Merken, wie schräg der Spieler im Tor war
        if (passed == false && Mathf.Abs(distance) < gateHeight / 2)
        {
            int currentStep = Mathf.Abs(plane.step);
            if (currentStep > maxStepUsed) maxStepUsed = currentStep;
        }

        if (passed == false && distance < -gateHeight / 2)
        {
            passed = true;
            GameManager.instance.GatePassed(this);
        }

        UpdateLightning();

        if (transform.position.y < -15)
        {
            Destroy(gameObject);
        }
    }

    void UpdateLightning()
    {
        if (lightning == null) return;

        lightningTimer -= Time.deltaTime;

        if (lightningTimer <= 0)
        {
            if (lightning.activeSelf)
            {
                lightning.SetActive(false);
                lightningTimer = Random.Range(1.5f, 4f);
            }
            else
            {
                lightning.SetActive(true);
                lightningTimer = 0.4f;
            }
        }
    }

    void OnDrawGizmos()
    {
        if (requiredStep < 0 || requiredStep >= recommendedGaps.Length) return;

        float half = recommendedGaps[requiredStep] / 2;
        Vector3 p = transform.position;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(p + new Vector3(-half, -gateHeight / 2, 0), p + new Vector3(-half, gateHeight / 2, 0));
        Gizmos.DrawLine(p + new Vector3(half, -gateHeight / 2, 0), p + new Vector3(half, gateHeight / 2, 0));
    }
}
