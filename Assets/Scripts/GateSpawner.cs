using UnityEngine;

public class GateSpawner : MonoBehaviour
{
    public CloudGate[] gates;               // alle Tor-Prefabs hier reinziehen
    public float distanceBetweenGates = 8;
    public float minDistance = 5;
    public int gatesUntilHarder = 4;        // alle 4 Tore kommt eine schwierigere Stufe dazu

    float distanceTravelled;
    int gatesSpawned = 0;
    int lastIndex = -1;

    void Start()
    {
        distanceTravelled = distanceBetweenGates;   // erstes Tor sofort
    }

    void Update()
    {
        if (GameManager.instance.state != GameManager.GameState.Playing) return;

        distanceTravelled += GameManager.instance.speed * Time.deltaTime;

        // Tore kommen mit der Zeit näher zusammen
        float currentDistance = distanceBetweenGates - gatesSpawned * 0.08f;
        if (currentDistance < minDistance) currentDistance = minDistance;

        if (distanceTravelled >= currentDistance)
        {
            distanceTravelled = 0;
            SpawnGate();
        }
    }

    void SpawnGate()
    {
        if (gates.Length == 0) return;

        int maxStepAllowed = 1 + gatesSpawned / gatesUntilHarder;

        // Zufälliges Tor suchen, das nicht zu schwer ist und nicht gleich wie das letzte
        int index = Random.Range(0, gates.Length);
        for (int i = 0; i < 20; i++)
        {
            index = Random.Range(0, gates.Length);
            if (gates[index].requiredStep <= maxStepAllowed && index != lastIndex) break;
        }

        lastIndex = index;
        Instantiate(gates[index], transform.position, Quaternion.identity);
        gatesSpawned++;
    }
}
