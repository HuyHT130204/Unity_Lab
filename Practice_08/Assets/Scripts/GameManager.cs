using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// GameManager chịu trách nhiệm hiển thị popup, quản lý nút Continue/Restart
/// Gắn script này lên Main Camera (hoặc 1 empty GameObject persistent)
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("UI Popup")]
    public GameObject popupCanvas;      // kéo PopupCanvas vào đây
    public TMP_Text titleText;          // kéo Title_TMP
    public TMP_Text messageText;        // kéo Message_TMP

    [Header("Buttons (optional if you want refs)")]
    public Button continueButton;       // kéo Button_Continue (không bắt buộc)
    public Button restartButton;        // kéo Button_Restart (không bắt buộc)

    void Awake()
    {
        // đảm bảo popup ẩn khi khởi đầu
        if (popupCanvas != null) popupCanvas.SetActive(false);
    }

    /// <summary>
    /// Hiện popup và thay nội dung title/message
    /// </summary>
    public void ShowPopup(string title, string message)
    {
        if (popupCanvas != null) popupCanvas.SetActive(true);
        if (titleText != null) titleText.text = title;
        if (messageText != null) messageText.text = message;
    }

    /// <summary>
    /// Chuyển sang scene tiếp theo (theo build index)
    /// Gắn cho nút Continue -> OnClick -> GameManager.NextScene
    /// </summary>
    public void NextScene()
    {
        int nextIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextIndex);
        }
        else
        {
            Debug.Log("No next scene in Build Settings");
        }
    }

    /// <summary>
    /// Restart scene hiện tại
    /// Gắn cho nút Restart -> OnClick -> GameManager.RestartScene
    /// </summary>
    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// (Optional) Hide the popup (ví dụ trước khi chuyển scene)
    /// </summary>
    public void HidePopup()
    {
        if (popupCanvas != null) popupCanvas.SetActive(false);
    }
}
