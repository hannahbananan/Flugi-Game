using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public enum GameState { Start, Playing, GameOver }
    public GameState state = GameState.Start;

    public float lift = 100;
    public float maxLift = 100;
    public float[] liftPerSecond = { 9, -5, -13, -24 };   // bei 0°, 30°, 60°, 90°
    public float hitDamage = 30;

    public int passPoints = 10;
    public int perfectPoints = 50;
    public int comboBonus = 25;

    public float speed = 3;
    public float speedIncrease = 0.12f;
    public float maxSpeed = 10;

    public PlaneController plane;
    public RectTransform liftBar;       // Pivot X = 0, dann wird der Balken von links her kürzer
    public TMP_Text scoreText;
    public TMP_Text infoText;
    public TMP_Text gameOverText;
    public GameObject startScreen;
    public GameObject gameOverScreen;
    public GameObject hitEffect;

    int score = 0;
    int combo = 0;
    int gatesPassed = 0;
    float gameOverTime = 0;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if (startScreen != null) startScreen.SetActive(true);
        if (gameOverScreen != null) gameOverScreen.SetActive(false);
        if (infoText != null) infoText.text = "";
        UpdateUI();
    }

    void Update()
    {
        if (state == GameState.Playing)
        {
            // Gerade fliegen = Höhe zurück, schräg fliegen = sinken
            int index = Mathf.Abs(plane.step);
            lift = lift + liftPerSecond[index] * Time.deltaTime;
            if (lift > maxLift) lift = maxLift;

            if (lift <= 0)
            {
                lift = 0;
                GameOver();
            }

            UpdateUI();
        }

        if (state == GameState.GameOver)
        {
            gameOverTime += Time.deltaTime;
        }
    }

    // Wird bei jedem Tastendruck aufgerufen. true = Flugzeug darf kippen.
    public bool ButtonPressed()
    {
        if (state == GameState.Start)
        {
            StartGame();
            return false;
        }

        if (state == GameState.GameOver)
        {
            if (gameOverTime > 1) Restart();
            return false;
        }

        return true;
    }

    void StartGame()
    {
        state = GameState.Playing;
        if (startScreen != null) startScreen.SetActive(false);
    }

    void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void PlaneHitCloud(CloudGate gate)
    {
        if (state != GameState.Playing) return;
        if (gate.wasHit) return;    // pro Tor nur einmal Schaden

        gate.wasHit = true;
        lift -= hitDamage;
        combo = 0;

        if (hitEffect != null)
        {
            Instantiate(hitEffect, plane.transform.position, Quaternion.identity);
        }

        ShowInfo("GETROFFEN  -" + hitDamage);
        Debug.Log("Wolke getroffen! Höhe: " + lift);
    }

    public void GatePassed(CloudGate gate)
    {
        if (state != GameState.Playing) return;

        gatesPassed++;

        if (gate.wasHit == false)
        {
            // Perfekt = genau so schräg wie nötig, nicht mehr
            if (gate.requiredStep > 0 && gate.maxStepUsed == gate.requiredStep)
            {
                combo++;
                int points = perfectPoints + comboBonus * (combo - 1);
                score += points;
                ShowInfo("PERFEKT  +" + points);
            }
            else
            {
                score += passPoints;
                if (gate.maxStepUsed > gate.requiredStep) combo = 0;
            }
        }

        speed = speed + speedIncrease;
        if (speed > maxSpeed) speed = maxSpeed;

        Debug.Log("Tor geschafft: " + gatesPassed + "  Punkte: " + score);
    }

    void GameOver()
    {
        state = GameState.GameOver;
        gameOverTime = 0;

        int best = PlayerPrefs.GetInt("best", 0);
        if (score > best)
        {
            best = score;
            PlayerPrefs.SetInt("best", best);
        }

        if (gameOverScreen != null) gameOverScreen.SetActive(true);
        if (gameOverText != null)
        {
            gameOverText.text = "ABGESTÜRZT\n\n" + score + "\n\nBESTWERT  " + best;
        }

        Debug.Log("Game Over! Punkte: " + score);
    }

    void UpdateUI()
    {
        if (liftBar != null) liftBar.localScale = new Vector3(lift / maxLift, 1, 1);

        if (scoreText != null)
        {
            if (combo > 1) scoreText.text = score + "\nCOMBO x" + combo;
            else scoreText.text = score.ToString();
        }
    }

    void ShowInfo(string message)
    {
        if (infoText == null) return;
        infoText.text = message;

        CancelInvoke("ClearInfo");
        Invoke("ClearInfo", 1f);
    }

    void ClearInfo()
    {
        infoText.text = "";
    }
}
