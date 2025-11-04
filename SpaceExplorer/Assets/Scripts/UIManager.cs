using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Victory UI")]
    [SerializeField]
    private GameObject victoryPanel;
    [SerializeField]
    private Button restartButton;
    [SerializeField]
    private Button homeButton;
    [SerializeField]
    private TextMeshProUGUI victoryScoreText;

    [Header("Health UI")]
    [SerializeField]
    private UnityEngine.UI.Slider healthBar;
    [SerializeField]
    private TextMeshProUGUI healthText;

    void Awake()
    {
        if (Instance == null) Instance = this; else Destroy(gameObject);
        if (victoryPanel != null) victoryPanel.SetActive(false);
    }

    public void UpdateScore(int score)
    {
        // Nếu bạn có HUD điểm, gán vào đây (tuỳ dự án)
    }

    public void ShowPauseMenu(bool show)
    {
        // Nếu có Panel Pause, bật/tắt ở đây
    }

    public void ForceHUDVisible()
    {
        // Nếu cần ép HUD hiện lại sau Resume
    }

    public void ShowGameOverMenu(bool show, int finalScore)
    {
        // Bạn có thể tái sử dụng Victory Panel hoặc tạo panel riêng cho Game Over
    }

    public void ShowVictory(int finalScore)
    {
        if (victoryPanel == null)
        {
            Debug.LogWarning("UIManager: Chưa gán victoryPanel - không thể hiện Victory UI.");
            return;
        }

        if (victoryScoreText != null)
        {
            victoryScoreText.text = $"Score: {finalScore}";
        }

        victoryPanel.SetActive(true);

        if (restartButton != null)
        {
            restartButton.onClick.RemoveAllListeners();
            restartButton.onClick.AddListener(() =>
            {
                Time.timeScale = 1f;
                GameManager.Instance.RestartGame();
            });
        }

        if (homeButton != null)
        {
            homeButton.onClick.RemoveAllListeners();
            homeButton.onClick.AddListener(() =>
            {
                Time.timeScale = 1f;
                SceneManager.LoadScene("MainMenu");
            });
        }
    }

    public void UpdateHealth(int currentHealth, int maxHealth)
    {
        // Cập nhật health bar (Slider)
        if (healthBar != null)
        {
            healthBar.value = (float)currentHealth / maxHealth;
        }

        // Cập nhật health text nếu có
        if (healthText != null)
        {
            healthText.text = $"{currentHealth}/{maxHealth}";
        }
    }
}


