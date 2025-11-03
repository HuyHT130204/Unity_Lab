using UnityEngine;

/// <summary>
/// Gắn script này lên Wall (đối tượng có isTrigger = true)
/// Khi Player (tag = "Player") chạm vào, mở popup bằng GameManager
/// </summary>
public class EnterGame : MonoBehaviour
{
    public GameManager gameManager; // kéo từ Inspector -> Main Camera

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (gameManager != null)
            {
                gameManager.ShowPopup("Game Over", "You collided with the wall. Press Continue or Restart.");
            }
            else
            {
                Debug.LogWarning("GameManager ref missing in EnterGame");
            }
        }
    }
}
