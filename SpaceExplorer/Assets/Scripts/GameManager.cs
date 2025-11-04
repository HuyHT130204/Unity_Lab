// GameManager.cs
using UnityEngine;
using UnityEngine.SceneManagement; // Cần thiết để restart game
using TMPro;

public class GameManager : MonoBehaviour
{
    // Singleton
    public static GameManager Instance;

    public enum GameState { Playing, Paused, GameOver }
    public GameState currentState;

    // Gameplay state dùng bởi nhiều script
    public bool blockControl = false;
    public float playerMoveSpeed = 6f;
    public float playerFireRate = 0.2f;
    public bool isSpecialActive = false;

    public int coinsCollected = 0;
    public TextMeshProUGUI coinsCountText;

    [Header("Health UI")]
    public UnityEngine.UI.Slider healthBar;
    public TextMeshProUGUI healthText;

    private int score = 0;
    private int maxHealth = 100;
    private int _playerHealth = 100;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        currentState = GameState.Playing;
        Time.timeScale = 1f;
        
        // Khởi tạo health ban đầu
        maxHealth = 100;
        _playerHealth = 100; // Set trực tiếp để tránh gọi UpdateHealthUI 2 lần
        
        // Đảm bảo health bar có min/max đúng
        if (healthBar != null)
        {
            healthBar.minValue = 0f;
            healthBar.maxValue = 1f;
        }
        
        UpdateHealthUI();

        // An toàn nếu chưa có UIManager
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateScore(score);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentState == GameState.Playing)
            {
                PauseGame();
            }
            else if (currentState == GameState.Paused)
            {
                ResumeGame();
            }
        }
    }

    public void AddScore(int points)
    {
        if (currentState != GameState.Playing) return;
        score += points;
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateScore(score);
        }
    }

    public void PauseGame()
    {
        currentState = GameState.Paused;
        Time.timeScale = 0f;
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowPauseMenu(true);
        }
    }

    public void ResumeGame()
    {
        currentState = GameState.Playing;
        Time.timeScale = 1f;
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowPauseMenu(false);
            UIManager.Instance.ForceHUDVisible();
        }
    }

    public void GameOver()
    {
        currentState = GameState.GameOver;
        Time.timeScale = 0f;
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowGameOverMenu(true, score);
        }
    }

    public void WinGame()
    {
        currentState = GameState.GameOver;
        Time.timeScale = 0f;
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowVictory(score);
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public System.Collections.IEnumerator StartBulletPowerUpTimer(float duration)
    {
        isSpecialActive = true;
        yield return new WaitForSeconds(duration);
        isSpecialActive = false;
    }

    public void HandlePlayerDeath()
    {
        PlayerHealth = 0;
        UpdateHealthUI();
        GameOver();
    }

    public void UpdateHealthUI()
    {
        float healthPercent = (float)PlayerHealth / maxHealth;
        
        // Cập nhật health bar (Slider)
        if (healthBar != null)
        {
            healthBar.value = healthPercent;
            Debug.Log($"GameManager: UpdateHealthUI - Health: {PlayerHealth}/{maxHealth}, Bar Value: {healthPercent}, Slider Min: {healthBar.minValue}, Max: {healthBar.maxValue}");
        }
        else
        {
            Debug.LogWarning("GameManager: HealthBar chưa được gán trong Inspector!");
        }

        // Cập nhật health text nếu có
        if (healthText != null)
        {
            healthText.text = $"{PlayerHealth}/{maxHealth}";
        }

        // Cũng gọi UIManager nếu có method UpdateHealth
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateHealth(PlayerHealth, maxHealth);
        }
    }

    // Property để tự động cập nhật UI khi health thay đổi
    public int PlayerHealth
    {
        get { return _playerHealth; }
        set
        {
            _playerHealth = Mathf.Clamp(value, 0, maxHealth);
            UpdateHealthUI();
        }
    }
}