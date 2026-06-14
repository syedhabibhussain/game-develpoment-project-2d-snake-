using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class SnakeMovement : MonoBehaviour 
{
    private Vector2 _direction = Vector2.right;
    public GameObject bodyPrefab; 
    public GameObject gameOverPanel; 
    
    // 🟢 LIVE HUD
    public TextMeshProUGUI liveScoreText;
    public TextMeshProUGUI liveHighScoreText;

    // 🔵 GAME OVER UI
    public TextMeshProUGUI gameOverScoreText;
    public TextMeshProUGUI gameOverHighScoreText;
    public TextMeshProUGUI goldenText; // "NEW HIGH SCORE!"

    // 🎬 CANVAS GROUPS FOR ANIMATION
    public CanvasGroup gameOverGroup;
    public CanvasGroup scoreGroup;
    public CanvasGroup congratsGroup;
    public CanvasGroup buttonsGroup;

    // ✨ OPTIONAL: Golden particle prefab
    public ParticleSystem goldenParticlePrefab;

    // 🆕 PAUSE
    public GameObject pausePanel;
    private bool isPaused = false;

    // 🆕 LEVEL COMPLETE PANEL
    public CanvasGroup levelCompletePanel;

    // 🔗 MANAGER REFERENCES
    public FoodManager foodManager;
    public PowerUpManager powerUpManager;

    // 🆕 UI MANAGER REFERENCE (For pop-up messages)
    public PowerUpUIManager powerUpUIManager;

    // 🆕 LEVEL SYSTEM VARIABLES
    private int currentLevel = 1;
    public int targetScore = 10;      // Set in Inspector or via code
    public string nextLevelName = "Level2";

    private List<Transform> _segments = new List<Transform>();
    [Range(0.05f, 1f)] public float speed = 0.1f;

    private int score = 0;
    private int highScore = 0;
    private bool isGameOver = false;

    // 🆕 Power-Up variables
    private bool isDoubleScore = false;
    private bool isSlowDown = false;
    private float slowDownTimer = 0f;
    private float originalSpeed;

    private bool invincible = false;
    private float invincibleTimer = 0f;
    private bool isSpeedUp = false;
    private float speedUpTimer = 0f;
    private int scoreMultiplier = 1;
    private float multiplierTimer = 0f;
    private bool isGhostMode = false;
    private float ghostTimer = 0f;
    private bool isFrozen = false;

    private void Start() {
        _segments.Add(this.transform);
        Time.timeScale = 1; 
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        UpdateLiveUI();
        gameOverPanel.SetActive(false);
        pausePanel.SetActive(false);
        
        // Hide Level Complete Panel at start
        if (levelCompletePanel != null) {
            levelCompletePanel.alpha = 0;
            levelCompletePanel.blocksRaycasts = false;
        }

        isGameOver = false;
        originalSpeed = speed;
        
        // 🔥 DETECT WHICH LEVEL WE ARE IN
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "Level1") {
            currentLevel = 1;
            targetScore = 10;
            nextLevelName = "Level2";
            // Disable power-ups for Level 1
            if (powerUpManager != null) {
                powerUpManager.enabled = false;
            }
        } else if (sceneName == "Level2") {
            currentLevel = 2;
            targetScore = 20;
            nextLevelName = "Level3";
            // Enable power-ups for Level 2
            if (powerUpManager != null) {
                powerUpManager.enabled = true;
            }
        } else if (sceneName == "Level3") {
            currentLevel = 3;
            targetScore = 30;
            nextLevelName = ""; // No next level (end game)
            // Enable power-ups for Level 3
            if (powerUpManager != null) {
                powerUpManager.enabled = true;
            }
        }

        // 🎵 Switch to gameplay music (With Null Check)
        if (AudioManager.Instance != null) {
            AudioManager.Instance.PlayGameplayBGM();
        } else {
            Debug.LogWarning("AudioManager is missing in this scene.");
        }

        // 🍎 Ask FoodManager to spawn the first food
        if (foodManager != null) foodManager.SpawnFood();
    }

    private void Update() {
        if (isGameOver || isFrozen) return;

        if (Input.GetKeyDown(KeyCode.Escape)) {
            TogglePause();
        }

        // Movement input
        if (Input.GetKeyDown(KeyCode.W) && _direction != Vector2.down) _direction = Vector2.up;
        else if (Input.GetKeyDown(KeyCode.S) && _direction != Vector2.up) _direction = Vector2.down;
        else if (Input.GetKeyDown(KeyCode.A) && _direction != Vector2.right) _direction = Vector2.left;
        else if (Input.GetKeyDown(KeyCode.D) && _direction != Vector2.left) _direction = Vector2.right;

        // 🆕 Power-Up timers
        if (isSlowDown) {
            slowDownTimer -= Time.deltaTime;
            if (slowDownTimer <= 0f) {
                isSlowDown = false;
                speed = originalSpeed;
            }
        }

        if (invincible) {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer <= 0f) invincible = false;
        }

        if (isSpeedUp) {
            speedUpTimer -= Time.deltaTime;
            if (speedUpTimer <= 0f) {
                isSpeedUp = false;
                speed = originalSpeed;
            }
        }

        if (scoreMultiplier > 1) {
            multiplierTimer -= Time.deltaTime;
            if (multiplierTimer <= 0f) scoreMultiplier = 1;
        }

        if (isGhostMode) {
            ghostTimer -= Time.deltaTime;
            if (ghostTimer <= 0f) isGhostMode = false;
        }

        // 🆕 LEVEL COMPLETION CHECK
        if (!isGameOver && score >= targetScore && currentLevel < 3) {
            // Save current score
            PlayerPrefs.SetInt("CurrentScore", score);
            PlayerPrefs.Save();
            
            // 🆕 Show Level Complete Panel instead of instantly loading
            if (levelCompletePanel != null) {
                levelCompletePanel.alpha = 1;
                levelCompletePanel.blocksRaycasts = true;
            }
            
            // Pause the game
            Time.timeScale = 0;
            isGameOver = true; // Temporarily stop movement
        }
    }

    private void FixedUpdate() {
        if (isGameOver || isPaused || isFrozen) return;

        for (int i = _segments.Count - 1; i > 0; i--) {
            _segments[i].position = _segments[i - 1].position;
        }

        float nextX = Mathf.Round(this.transform.position.x) + _direction.x;
        float nextY = Mathf.Round(this.transform.position.y) + _direction.y;

        // Wall teleportation
        if (nextX > 12) nextX = -12; else if (nextX < -12) nextX = 12;
        if (nextY > 5) nextY = -3; else if (nextY < -3) nextY = 5;

        this.transform.position = new Vector3(nextX, nextY, 0.0f);
    }

    private void Grow() {
        // ⚠️ Safety check: Is bodyPrefab assigned?
        if (bodyPrefab == null) {
            Debug.LogError("SnakeMovement: bodyPrefab is null! Assign a body segment prefab in the Inspector.");
            return;
        }

        // ⚠️ Safety check: Are there existing segments to spawn from?
        if (_segments.Count == 0) {
            Debug.LogError("SnakeMovement: No segments found to grow from!");
            return;
        }

        GameObject segment = Instantiate(this.bodyPrefab);
        segment.transform.position = _segments[_segments.Count - 1].position;
        _segments.Add(segment.transform);

        // 🆕 Double Score logic
        int points = isDoubleScore ? 2 : 1;
        score += points;
        UpdateLiveUI();
        UpdateSpeed();

        // 🎵 Eat sound (safety check)
        if (AudioManager.Instance != null) {
            AudioManager.Instance.PlayEat();
        } else {
            Debug.LogWarning("AudioManager missing, so Eat sound was skipped.");
        }
    }

    private void UpdateLiveUI() {
        if (liveScoreText != null) 
            liveScoreText.text = "Score: " + score;
        if (liveHighScoreText != null)
            liveHighScoreText.text = "High Score: " + highScore;
    }

    private void UpdateSpeed() {
        int prevLevel = (score - 1) / 5;
        int newLevel = score / 5;
        if (newLevel > prevLevel) {
            if (AudioManager.Instance != null) {
                AudioManager.Instance.PlaySpeedUp();
            }
        }

        int level = score / 5;
        float newSpeed = 0.1f - (level * 0.01f);
        speed = Mathf.Clamp(newSpeed, 0.02f, 0.1f);

        // Color tint based on speed
        float t = Mathf.InverseLerp(0.1f, 0.02f, speed);
        Color bodyColor = Color.Lerp(Color.green, Color.red, t);
        
        foreach (Transform seg in _segments) {
            SpriteRenderer sr = seg.GetComponent<SpriteRenderer>();
            if (sr != null) sr.color = bodyColor;
        }
    }

    // 🆕 Activate Power-Up (called from PowerUp script)
    public void ActivatePowerUp(int type) {
        string message = "";
        Color color = Color.white;

        switch (type) {
            case 0: // Double Score
                isDoubleScore = true;
                message = "Double Score!";
                color = Color.gold;
                break;
            case 1: // Slow Down
                isSlowDown = true;
                slowDownTimer = 5f;
                originalSpeed = speed;
                speed *= 0.5f;
                message = "Slow Down!";
                color = Color.cyan;
                break;
            case 2: // Magnet
                StartCoroutine(MagnetRoutine());
                message = "Magnet!";
                color = Color.magenta;
                break;
            case 3: // Extra Length
                for (int i = 0; i < 3; i++) Grow();
                message = "Extra Length!";
                color = Color.green;
                break;
            case 4: // Length Decrease
                if (_segments.Count > 3) {
                    for (int i = 0; i < 3; i++) {
                        Destroy(_segments[_segments.Count - 1].gameObject);
                        _segments.RemoveAt(_segments.Count - 1);
                    }
                }
                message = "Length Decrease!";
                color = Color.red;
                break;
            case 5: // Invincibility
                invincible = true;
                invincibleTimer = 4f;
                message = "Invincible!";
                color = Color.white;
                break;
            case 6: // Speed Up
                isSpeedUp = true;
                speedUpTimer = 4f;
                originalSpeed = speed;
                speed *= 1.5f;
                message = "Speed Up!";
                color = Color.magenta;
                break;
            case 7: // Score Multiplier (x3)
                scoreMultiplier = 3;
                multiplierTimer = 8f;
                message = "x3 Score!";
                color = new Color(1f, 0.5f, 0f);
                break;
            case 8: // Freeze
                isFrozen = true;
                Time.timeScale = 0;
                StartCoroutine(UnfreezeAfterDelay(3f));
                message = "Freeze!";
                color = Color.blue;
                break;
            case 9: // Ghost Mode
                isGhostMode = true;
                ghostTimer = 5f;
                message = "Ghost Mode!";
                color = Color.black;
                break;
        }

        // ✅ Show the pop-up message
        if (powerUpUIManager != null) {
            powerUpUIManager.ShowPowerUpMessage(message, color);
        }
    }

    // 🧲 Magnet coroutine
    private IEnumerator MagnetRoutine() {
        float magnetDuration = 6f;
        float elapsed = 0f;
        while (elapsed < magnetDuration) {
            GameObject[] foods = GameObject.FindGameObjectsWithTag("Food");
            foreach (GameObject food in foods) {
                Vector2 direction = (transform.position - food.transform.position).normalized;
                food.transform.position += (Vector3)(direction * 2f * Time.deltaTime);
            }
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    // 🧊 Unfreeze after delay
    private IEnumerator UnfreezeAfterDelay(float delay) {
        yield return new WaitForSecondsRealtime(delay);
        isFrozen = false;
        Time.timeScale = 1;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Food")) {
            Destroy(other.gameObject);
            Grow();
            if (foodManager != null) foodManager.SpawnFood();
        } 
        else if (other.CompareTag("Body")) {
            if (!invincible) {
                ShowGameOver();
            }
            // If invincible, ignore body collision
        }
        else if (other.CompareTag("Wall")) {
            if (!isGhostMode) {
                ShowGameOver();
            }
            // If ghost mode, ignore wall collision
        }
        // 🆕 OBSTACLE COLLISION
        else if (other.CompareTag("Obstacle")) {
            ShowGameOver();
        }
    }

    // 🔗 Helper method for FoodManager to check snake position
    public List<Transform> GetSegments() {
        return _segments;
    }

    // 🔗 Helper method for PowerUpManager to stop spawning when game over
    public bool IsGameOver() {
        return isGameOver;
    }

    // 🔗 Helper method to get current score
    public int GetScore() {
        return score;
    }

    private void ShowGameOver() {
        isGameOver = true;
        gameOverPanel.SetActive(true);
        
        // 🎵 Play game over sound (with null check)
        if (AudioManager.Instance != null) {
            AudioManager.Instance.PlayGameOver();
        }
        
        StartCoroutine(GameOverSequence());
    }

    private IEnumerator GameOverSequence() {
        gameOverGroup.alpha = 1;
        Transform goText = gameOverGroup.transform.GetChild(0);
        goText.localScale = Vector3.zero;
        float popTime = 0.4f;
        float elapsed = 0f;
        while (elapsed < popTime) {
            elapsed += Time.unscaledDeltaTime;
            goText.localScale = Vector3.one * Mathf.Lerp(0, 1, elapsed / popTime);
            yield return null;
        }
        goText.localScale = Vector3.one;
        yield return new WaitForSecondsRealtime(1f);
        gameOverGroup.alpha = 0;

        float fadeTime = 0.5f;
        elapsed = 0f;
        while (elapsed < fadeTime) {
            elapsed += Time.unscaledDeltaTime;
            scoreGroup.alpha = Mathf.Lerp(0, 1, elapsed / fadeTime);
            yield return null;
        }
        scoreGroup.alpha = 1;
        gameOverScoreText.text = "Score: " + score;
        gameOverHighScoreText.text = "High Score: " + highScore;
        yield return new WaitForSecondsRealtime(1f);

        if (score > highScore) {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();

            elapsed = 0f;
            while (elapsed < fadeTime) {
                elapsed += Time.unscaledDeltaTime;
                congratsGroup.alpha = Mathf.Lerp(0, 1, elapsed / fadeTime);
                yield return null;
            }
            congratsGroup.alpha = 1;
            goldenText.text = "NEW HIGH SCORE!";
            goldenText.color = new Color(1f, 0.84f, 0f);

            if (goldenParticlePrefab != null) {
                ParticleSystem particles = Instantiate(goldenParticlePrefab, congratsGroup.transform.position, Quaternion.identity);
                particles.Play();
                Destroy(particles.gameObject, 2f);
            }

            if (AudioManager.Instance != null) {
                AudioManager.Instance.PlayHighScore();
            }
            
            yield return new WaitForSecondsRealtime(1.5f);
        }

        elapsed = 0f;
        while (elapsed < fadeTime) {
            elapsed += Time.unscaledDeltaTime;
            buttonsGroup.alpha = Mathf.Lerp(0, 1, elapsed / fadeTime);
            yield return null;
        }
        buttonsGroup.alpha = 1;
    }

    private void TogglePause() {
        isPaused = !isPaused;
        pausePanel.SetActive(isPaused);
        Time.timeScale = isPaused ? 0 : 1;

        if (isPaused) {
            if (AudioManager.Instance != null) {
                AudioManager.Instance.PlayMenuBGM();
            }
        } else {
            if (AudioManager.Instance != null) {
                AudioManager.Instance.PlayGameplayBGM();
            }
        }
    }

    public void ResumeGame() {
        TogglePause();
    }

    // 🆕 BUTTON METHODS FOR LEVEL COMPLETE PANEL
    public void PlayAgain() {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void NextLevel() {
        Time.timeScale = 1;
        if (!string.IsNullOrEmpty(nextLevelName)) {
            SceneManager.LoadScene(nextLevelName);
        }
    }

    public void MainMenu() {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }

    public void RestartGame() {
        // 🆕 PROPER RESTART: Reload the current level scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame() {
        Debug.Log("Game is quitting...");
        Application.Quit();
    }
}