using UnityEngine;

public class EnemyBulletScript : MonoBehaviour
{
    [SerializeField]
    private int damage = 10; // Sát thương mặc định

    [SerializeField]
    private Rigidbody2D rb;

    private GameManager gameManager;
    private float speed;
    private float minY, maxY;

    void Start()
    {
        gameManager = GameManager.Instance;

        speed = 2.4f;

        // Tính biên dưới/trên theo Camera thay vì số cố định
        float halfHeight = Camera.main.orthographicSize;
        maxY = halfHeight + 1.0f;
        minY = -halfHeight - 1.0f;

        // Đảm bảo có Collider2D và Rigidbody2D
        EnsureColliderWorking();
    }

    private void EnsureColliderWorking()
    {
        var collider2D = GetComponent<Collider2D>();
        if (collider2D == null)
        {
            Debug.LogWarning("EnemyBulletScript: Không có Collider2D. Thêm CircleCollider2D.");
            var circleCollider = gameObject.AddComponent<CircleCollider2D>();
            circleCollider.isTrigger = true;
            circleCollider.radius = 0.1f;
        }
        else
        {
            collider2D.isTrigger = true;
        }

        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                Debug.LogWarning("EnemyBulletScript: Không có Rigidbody2D. Thêm Rigidbody2D.");
                rb = gameObject.AddComponent<Rigidbody2D>();
                rb.gravityScale = 0f;
                rb.bodyType = RigidbodyType2D.Dynamic; // Dynamic để dùng linearVelocity
                rb.constraints = RigidbodyConstraints2D.FreezeRotation; // Giữ rotation cố định
            }
        }
        else
        {
            // Đảm bảo có thể dùng linearVelocity
            if (rb.bodyType == RigidbodyType2D.Kinematic)
            {
                rb.bodyType = RigidbodyType2D.Dynamic;
                rb.gravityScale = 0f;
                rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            }
        }
    }

    void Update()
    {
        // Đạn đi thẳng xuống
        rb.linearVelocity = Vector2.down * speed;

        // Hủy khi ra khỏi màn hình theo y
        if (transform.position.y > maxY || transform.position.y < minY)
        {
            Destroy(gameObject);
        }

        if (GameObject.FindGameObjectsWithTag("Boss") != null)
            return;

        if (GameObject.FindGameObjectsWithTag("enemy").Length == 0)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Bỏ qua đạn của player (không nên va chạm với đạn của player)
        if (collision.CompareTag("playerBullet"))
        {
            return;
        }
        
        // Chỉ log khi va chạm với Player (để debug)
        if (collision.CompareTag("Player"))
        {
            Debug.Log($"EnemyBulletScript: Đánh trúng Player! Health trước: {gameManager?.PlayerHealth}, Damage: {damage}");
        }
        
        if (collision.CompareTag("Player"))
        {
            Destroy(gameObject);
            if (gameManager != null && gameManager.currentState == GameManager.GameState.Playing)
            {
                // Kiểm tra health trước khi trừ damage
                if (gameManager.PlayerHealth <= 0)
                {
                    // Đã chết rồi, không xử lý thêm
                    return;
                }
                
                gameManager.PlayerHealth -= damage;
                Debug.Log($"EnemyBulletScript: Health sau: {gameManager.PlayerHealth}");
                
                // Kiểm tra nếu health <= 0 thì gọi GameOver
                if (gameManager.PlayerHealth <= 0)
                {
                    gameManager.PlayerHealth = 0;
                    gameManager.HandlePlayerDeath();
                }
            }
        }
    }
}
