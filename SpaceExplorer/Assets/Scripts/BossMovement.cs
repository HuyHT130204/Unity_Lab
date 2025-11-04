using UnityEngine;

public class BossMovement : MonoBehaviour
{
    private float fireRate = 0.7f; // bắn nhanh hơn
    private float nextFireTime = 0f;
    private EnemySpawner enemySpawner;
    private GameManager gameManager;

    [SerializeField]
    private GameObject[] spawnPoints;

    [SerializeField]
    private GameObject bullet;

    void Awake()
    {
        gameManager = GameManager.Instance;
        enemySpawner = EnemySpawner.Instance;
    }

    void Start()
    {
        nextFireTime = fireRate;
        Debug.Log("Boss Spawned at: " + transform.position);
    }

    void OnDestroy()
    {
        Debug.Log("Boss Destroyed");
    }

    void Update()
    {
        if (gameManager == null || enemySpawner == null)
            return;
        if (gameManager.blockControl)
            return;

        if (!enemySpawner.isSpawning)
        {
            nextFireTime -= Time.deltaTime;
            if (nextFireTime <= 0f)
            {
                foreach (var spawnPoint in spawnPoints)
                {
                    FireBullet(spawnPoint, bullet);
                }
                nextFireTime = fireRate;
            }
        }
    }

    private void FireBullet(GameObject spawnPoint, GameObject bulletPrefab)
    {
        Instantiate(bulletPrefab, spawnPoint.transform.position, Quaternion.Euler(0f, 0f, -90f));
    }
}
