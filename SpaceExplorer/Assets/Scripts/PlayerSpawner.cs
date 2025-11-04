using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject playerPrefab;

    [SerializeField]
    private Transform startSpawnPoint; // tùy chọn: kéo thả từ scene nếu có

    void Start()
    {
        // Nếu đã có Player trong scene thì thôi
        var existing = GameObject.FindGameObjectWithTag("Player");
        if (existing != null) return;

        Vector3 spawnPos;
        if (startSpawnPoint != null)
        {
            spawnPos = startSpawnPoint.position;
        }
        else
        {
            // Trung tâm màn hình, hơi thấp xuống
            float halfHeight = Camera.main.orthographicSize;
            spawnPos = new Vector3(0f, -halfHeight * 0.5f, 0f);
        }

        if (playerPrefab != null)
        {
            var player = Instantiate(playerPrefab, spawnPos, Quaternion.identity);
            player.tag = "Player";
        }
        else
        {
            Debug.LogError("PlayerSpawner: Chưa gán playerPrefab trong Inspector.");
        }
    }
}


