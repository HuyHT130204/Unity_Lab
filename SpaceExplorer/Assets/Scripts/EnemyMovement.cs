using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    private float fireRate = 1f;
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
        if (enemySpawner.isGridPattern)
        {
            fireRate = 2f;
        }
        else
        {
            fireRate = 1.5f;
        }
    }

    void Update()
    {
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
        Instantiate(bulletPrefab, spawnPoint.transform.position, Quaternion.identity);
    }
}
