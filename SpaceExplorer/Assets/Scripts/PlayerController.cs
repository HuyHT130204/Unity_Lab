using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private UserData userData;
    private GameManager gameManager;
    private float xBoundary = 1.8f;
    private float yBoundary = 4.1f;
    [SerializeField]
    private float edgeMargin = 0.2f; // khoảng cách an toàn với mép màn
    private float moveSpeed;
    private float fireRate;
    private float nextFireTime = 0f;

    [SerializeField]
    private GameObject[] spawnPoints;

    [SerializeField]
    private GameObject specialSpawnPoint;

    [SerializeField]
    private GameObject bullet;

    [SerializeField]
    private GameObject specialBullet;

    void Awake()
    {
        gameManager = GameManager.Instance;
    }

    void Start()
    {
        moveSpeed = gameManager.playerMoveSpeed;
        fireRate = gameManager.playerFireRate;
        if (moveSpeed <= 0f) moveSpeed = 6f; // fallback nếu chưa cấu hình Inspector
        if (fireRate <= 0f) fireRate = 0.2f; // fallback nếu chưa cấu hình Inspector
        UpdateDynamicBounds();
        EnsureWithinView();
        EnsureSpriteVisible();
        EnsureCenteredIfOffscreen();
        EnsureColliderWorking();
    }

    void Update()
    {
        UpdateDynamicBounds();
#if UNITY_STANDALONE || UNITY_EDITOR
        // Di chuyển bằng chuột khi giữ chuột trái
        if (Input.GetMouseButton(0))
        {
            MoveTowardsScreenPosition(Input.mousePosition);
        }

        // Di chuyển bằng bàn phím (WASD / mũi tên)
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        if (Mathf.Abs(horizontal) > 0.001f || Mathf.Abs(vertical) > 0.001f)
        {
            Vector3 delta = new Vector3(horizontal, vertical, 0f) * (moveSpeed * Time.deltaTime);
            Vector3 targetPosition = transform.position + delta;
            targetPosition.x = Mathf.Clamp(targetPosition.x, -xBoundary, xBoundary);
            targetPosition.y = Mathf.Clamp(targetPosition.y, -yBoundary, yBoundary);
            transform.position = targetPosition;
        }
#else
        // Mobile: chạm màn hình để di chuyển
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            MoveTowardsScreenPosition(touch.position);
        }
#endif

        HandleShooting();
    }

    private void UpdateDynamicBounds()
    {
        if (Camera.main == null) return;
        float halfHeight = Camera.main.orthographicSize;
        float halfWidth = halfHeight * Camera.main.aspect;
        xBoundary = Mathf.Max(0.1f, halfWidth - edgeMargin);
        yBoundary = Mathf.Max(0.1f, halfHeight - edgeMargin);
    }

    private void EnsureWithinView()
    {
        if (Camera.main == null) return;
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, -xBoundary, xBoundary);
        pos.y = Mathf.Clamp(pos.y, -yBoundary, yBoundary);
        pos.z = 0f;
        transform.position = pos;
    }

    private void EnsureSpriteVisible()
    {
        var sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.enabled = true;
            if (sr.sortingOrder < 10) sr.sortingOrder = 10; // đảm bảo trên nền
            if (sr.color.a < 0.99f)
            {
                Color c = sr.color; c.a = 1f; sr.color = c;
            }
        }
    }

    private void EnsureCenteredIfOffscreen()
    {
        if (Camera.main == null) return;
        Vector3 vp = Camera.main.WorldToViewportPoint(transform.position);
        bool offscreen = vp.x < 0f || vp.x > 1f || vp.y < 0f || vp.y > 1f;
        if (offscreen)
        {
            transform.position = new Vector3(0f, -yBoundary * 0.5f, 0f);
        }
    }

    private void MoveTowardsScreenPosition(Vector3 position)
    {
        Vector3 screenWorld = Camera.main.ScreenToWorldPoint(position);
        screenWorld.z = 0f;
        Vector3 targetPosition = Vector3.Lerp(
            transform.position,
            screenWorld,
            moveSpeed * Time.deltaTime
        );

        // Giới hạn toạ độ
        targetPosition.x = Mathf.Clamp(targetPosition.x, -xBoundary, xBoundary);
        targetPosition.y = Mathf.Clamp(targetPosition.y, -yBoundary, yBoundary);

        transform.position = targetPosition;
    }

    private void HandleShooting()
    {
        nextFireTime -= Time.deltaTime;
        if (nextFireTime > 0f || gameManager.blockControl) return;

        foreach (var spawnPoint in spawnPoints)
        {
            FireBullet(spawnPoint, bullet);
        }
        if (gameManager.isSpecialActive)
        {
            FireBullet(specialSpawnPoint, specialBullet);
        }
        nextFireTime = fireRate;
    }

    void FireBullet(GameObject spawnPoint, GameObject bulletPrefab)
    {
        Instantiate(bulletPrefab, spawnPoint.transform.position, Quaternion.Euler(0f, 0f, 90f));
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Bỏ qua đạn của player (không nên va chạm với player)
        if (collision.gameObject.CompareTag("playerBullet"))
        {
            return;
        }
        
        // Debug log để kiểm tra va chạm (chỉ log các vật thể quan trọng)
        if (!collision.gameObject.CompareTag("coin") && !collision.gameObject.CompareTag("health") && !collision.gameObject.CompareTag("specialBulletPowerUp"))
        {
            Debug.Log($"PlayerController: OnTriggerEnter2D với {collision.gameObject.name}, Tag: {collision.gameObject.tag}");
        }
        
        if (collision.gameObject.CompareTag("coin"))
        {
            gameManager.coinsCollected++;
            if (gameManager.coinsCountText != null)
            {
                gameManager.coinsCountText.text = $"{gameManager.coinsCollected}";
            }
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("health"))
        {
            if (gameManager.PlayerHealth < 100)
            {
                gameManager.PlayerHealth += 10;
                if (gameManager.PlayerHealth > 100) gameManager.PlayerHealth = 100;
            }
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("specialBulletPowerUp"))
        {
            StartCoroutine(gameManager.StartBulletPowerUpTimer(7f));
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("enemy"))
        {
            Destroy(collision.gameObject);
            if (gameManager != null)
            {
                gameManager.PlayerHealth -= 10;
                // Đảm bảo health không âm
                if (gameManager.PlayerHealth <= 0)
                {
                    gameManager.PlayerHealth = 0;
                    gameManager.HandlePlayerDeath();
                }
            }
        }
        else if (collision.GetComponent<EnemyBulletScript>() != null)
        {
            // Xử lý va chạm với đạn địch (nếu đạn không tự xử lý)
            var bulletScript = collision.GetComponent<EnemyBulletScript>();
            if (gameManager != null)
            {
                // EnemyBulletScript đã xử lý damage, nhưng đảm bảo health được cập nhật
                if (gameManager.PlayerHealth <= 0)
                {
                    gameManager.PlayerHealth = 0;
                    gameManager.HandlePlayerDeath();
                }
            }
        }
    }

    private void EnsureColliderWorking()
    {
        // Kiểm tra và fix collider nếu có vấn đề
        var polygonCollider = GetComponent<PolygonCollider2D>();
        var boxCollider = GetComponent<BoxCollider2D>();
        var circleCollider = GetComponent<CircleCollider2D>();
        var collider2D = GetComponent<Collider2D>();

        // Kiểm tra nếu có PolygonCollider2D với pathCount > 0 (đã có shape), chỉ cần đảm bảo isTrigger = true
        if (polygonCollider != null)
        {
            // Kiểm tra pathCount thực tế (không phải Shape Count trong Inspector)
            // Nếu pathCount > 0, nghĩa là collider đã có shape, chỉ cần đảm bảo isTrigger
            if (polygonCollider.pathCount > 0)
            {
                polygonCollider.isTrigger = true;
                Debug.Log($"PlayerController: PolygonCollider2D có {polygonCollider.pathCount} path(s), đảm bảo isTrigger = true.");
            }
            else
            {
                // Nếu pathCount = 0 (không có shape), thêm BoxCollider2D
                Debug.LogWarning("PlayerController: PolygonCollider2D có pathCount = 0. Thêm BoxCollider2D thay thế.");
                if (boxCollider == null)
                {
                    boxCollider = gameObject.AddComponent<BoxCollider2D>();
                }
                boxCollider.isTrigger = true;
                
                var sr = GetComponent<SpriteRenderer>();
                if (sr != null && sr.sprite != null)
                {
                    boxCollider.size = sr.sprite.bounds.size;
                }
                else
                {
                    boxCollider.size = new Vector2(1f, 1f);
                }
            }
        }
        else if (polygonCollider == null && boxCollider == null && circleCollider == null)
        {
            // Nếu không có collider nào, thêm BoxCollider2D
            Debug.LogWarning("PlayerController: Không có Collider2D. Thêm BoxCollider2D.");
            boxCollider = gameObject.AddComponent<BoxCollider2D>();
            boxCollider.isTrigger = true;
            
            var sr = GetComponent<SpriteRenderer>();
            if (sr != null && sr.sprite != null)
            {
                boxCollider.size = sr.sprite.bounds.size;
            }
            else
            {
                boxCollider.size = new Vector2(1f, 1f);
            }
        }
        else
        {
            // Đảm bảo collider hiện có đang là trigger
            if (boxCollider != null && !boxCollider.isTrigger)
            {
                boxCollider.isTrigger = true;
            }
            if (circleCollider != null && !circleCollider.isTrigger)
            {
                circleCollider.isTrigger = true;
            }
        }

        // Đảm bảo có Rigidbody2D (cần thiết cho trigger)
        var rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogWarning("PlayerController: Không có Rigidbody2D. Thêm Rigidbody2D.");
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.gravityScale = 0f;
        }
        else
        {
            // Đảm bảo Rigidbody2D đúng cấu hình (không thay đổi nếu đã là Dynamic)
            if (rb.bodyType == RigidbodyType2D.Dynamic)
            {
                rb.gravityScale = 0f; // Chỉ đảm bảo không có gravity
            }
            else
            {
                rb.bodyType = RigidbodyType2D.Kinematic;
                rb.gravityScale = 0f;
            }
        }

        // Đảm bảo tag = "Player"
        if (!gameObject.CompareTag("Player"))
        {
            gameObject.tag = "Player";
            Debug.LogWarning("PlayerController: Tag đã được đặt thành 'Player'.");
        }

        var finalCollider = GetComponent<Collider2D>();
        Debug.Log($"PlayerController: Collider đã được kiểm tra. Tag: {gameObject.tag}, Collider: {finalCollider?.GetType().Name}, IsTrigger: {finalCollider?.isTrigger}, Rigidbody2D: {rb != null}");
    }
}
