using UnityEngine;

public class BulletScript : MonoBehaviour
{
    [SerializeField]
    private int damage;

    [SerializeField]
    private Rigidbody2D rb;

    [SerializeField]
    private GameObject coinPrefab;

    [SerializeField]
    private GameObject[] collectables;

    private GameManager gameManager;
    private EnemySpawner enemySpawner;

    void Awake()
    {
        gameManager = GameManager.Instance;
        enemySpawner = EnemySpawner.Instance;
        
        // Đảm bảo có Rigidbody2D
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                Debug.LogWarning("BulletScript: Không có Rigidbody2D. Thêm Rigidbody2D.");
                rb = gameObject.AddComponent<Rigidbody2D>();
                rb.gravityScale = 0f;
                rb.bodyType = RigidbodyType2D.Dynamic;
                rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            }
        }
    }

    void Start()
    {
        // Đảm bảo rb không null trước khi dùng
        if (rb != null)
        {
            rb.linearVelocity = Vector2.up * 8f;
        }
        else
        {
            Debug.LogError("BulletScript: Rigidbody2D không được gán!");
        }
    }

    void Update()
    {
        // Destroy bullet if out of bounds
        if (transform.position.y > 5f || transform.position.y < -5f)
        {
            Destroy(gameObject);
        }

        // Kiểm tra null trước khi truy cập
        if (gameManager != null && gameManager.PlayerHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("enemy"))
        {
            Destroy(collision.gameObject);

            if (Random.value <= 0.88f) // 88% chance to spawn coin
            {
                Instantiate(
                    coinPrefab,
                    collision.transform.position + Vector3.down * 0.5f,
                    Quaternion.identity
                );
            }

            if (collectables != null && collectables.Length > 0)
            {
                int randomChance = Random.Range(1, 101);

                if (randomChance <= 30)
                {
                    Instantiate(
                        collectables[Random.Range(0, collectables.Length)],
                        collision.transform.position + Vector3.up * 0.5f,
                        Quaternion.identity
                    );
                }
            }

            Destroy(gameObject);
        }

        if (collision.CompareTag("Boss"))
        {
            if (enemySpawner != null && enemySpawner.isSpawning)
                return;

            // Gây sát thương theo từng boss thay vì dùng biến toàn cục
            var bossHealth = collision.GetComponent<BossHealth>();
            if (bossHealth != null)
            {
                bossHealth.TakeDamage(damage);
            }
            Destroy(gameObject);
        }

        if (GameObject.FindGameObjectsWithTag("Boss") != null)
            return;

        if (GameObject.FindGameObjectsWithTag("enemy").Length == 0)
        {
            Destroy(gameObject);
        }
    }
}
