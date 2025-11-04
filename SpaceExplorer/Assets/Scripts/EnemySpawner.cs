using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance { get; private set; }
    public Transform[] pathGroups;

    [SerializeField]
    private PlaneDatabase planeDatabase;

    [SerializeField]
    private Transform startSpawnPoint;

    [SerializeField]
    private TextMeshProUGUI waveName;

    [SerializeField]
    private TextMeshProUGUI waveCountText;

    [SerializeField]
    private GameObject[] bossPrefabs;

    [SerializeField]
    private float edgeMargin = 0.3f; // khoảng cách an toàn so với mép màn

    private GameManager gameManager;
    private int totalWaves = 10;
    private int currentWave = 9;
    public bool isSpawning = false;
    public bool isGridPattern = false;

    void Awake()
    {
        gameManager = GameManager.Instance;
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        waveName.gameObject.SetActive(true);
        // waveCountText.text = "Wave 1";
        waveCountText.text = $"Wave {currentWave + 1} / {totalWaves}";
        StartCoroutine(StartWaves());
    }

    IEnumerator StartWaves()
    {
        while (currentWave < totalWaves)
        {
            currentWave++;

            waveCountText.text = $"Wave {currentWave} / {totalWaves}";

            // Randomly choose between arc or grid
            bool spawnArc = Random.value > 0.5f;

            yield return new WaitForSeconds(2f);

            waveName.gameObject.SetActive(false);

            if (spawnArc)
            {
                int enemyCount = Mathf.Min(6 + currentWave, 16); // tăng mật độ và trần số lượng
                float xSpacing = ComputeArcSpacing(enemyCount);
                SpawnArcWave(enemyCount, xSpacing);
            }
            else
            {
                SpawnGridPattern();
            }

            yield return new WaitUntil(() => GameObject.FindGameObjectsWithTag("enemy").Length == 0
            );

            if (currentWave < totalWaves)
            {
                waveName.gameObject.SetActive(true);
                waveName.text = $"Wave {currentWave + 1}";
            }
            yield return new WaitForSeconds(2f); // small pause before next wave
        }

        waveName.gameObject.SetActive(true);
        waveName.text = "Boss Alert!!!";
        yield return new WaitForSeconds(2f);
        SpawnBoss();
        // Chờ tới khi cả hai boss bị tiêu diệt mới kết thúc
        yield return StartCoroutine(WaitForBossesDefeated());
        waveName.gameObject.SetActive(true);
        waveName.text = "Victory!";
    }

    public void SpawnArcWave(int enemyCount, float xSpacing)
    {
        isSpawning = true;
        gameManager.blockControl = true;

        isGridPattern = false; // Reset grid pattern flag

        Transform randomPathGroup = pathGroups[Random.Range(0, pathGroups.Length)];
        List<Transform> pathPoints = new();
        foreach (Transform point in randomPathGroup)
        {
            pathPoints.Add(point);
        }

        // Tính toán bề rộng an toàn theo Camera
        float halfHeight = Camera.main.orthographicSize;
        float halfWidth = halfHeight * Camera.main.aspect;
        float usableHalfWidth = Mathf.Max(0.1f, halfWidth - edgeMargin);

        float totalWidth = (enemyCount - 1) * xSpacing;
        totalWidth = Mathf.Min(totalWidth, usableHalfWidth * 2f);
        float xStart = -totalWidth / 2f; // center align

        for (int i = 0; i < enemyCount; i++)
        {
            float xPos = Mathf.Clamp(xStart + i * xSpacing, -usableHalfWidth, usableHalfWidth);
            Vector3 spawnPos = new(
                xPos,
                Mathf.Max(pathPoints[0].position.y, halfHeight + 1f),
                pathPoints[0].position.z
            );

            GameObject enemy = Instantiate(
                planeDatabase
                    .enemyPlanes[Random.Range(0, planeDatabase.enemyPlanes.Count)]
                    .enemyPrefab,
                spawnPos,
                Quaternion.Euler(0, 0, 180f)
            );
            // Đảm bảo tag chuẩn để đếm wave
            enemy.tag = "enemy";

            // Thay vì đi theo path, cho địch zig-zag rơi xuống
            EnemyZigZagMover zig = enemy.AddComponent<EnemyZigZagMover>();
            // Cấu hình biên độ theo bề rộng hiện có
            zig.GetType().GetField("horizontalAmplitude", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(zig, Mathf.Clamp(usableHalfWidth * 0.4f, 1.2f, usableHalfWidth));
            zig.GetType().GetField("downwardSpeed", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(zig, 1.8f + currentWave * 0.05f);
        }
        isSpawning = false;
        gameManager.blockControl = false;
    }

    public void SpawnGridPattern()
    {
        isGridPattern = true;
        isSpawning = true;
        gameManager.blockControl = true; // Block player control during spawn

        int[] enemiesPerRow = { 7, 6, 5 }; // tăng số lượng mỗi hàng

        float halfHeight = Camera.main.orthographicSize;
        float halfWidth = halfHeight * Camera.main.aspect;
        float usableHalfWidth = Mathf.Max(0.1f, halfWidth - edgeMargin);

        // Tỉ lệ khoảng cách theo kích thước màn
        float xSpacing = Mathf.Clamp(usableHalfWidth * 0.25f, 0.9f, 3.5f);
        float ySpacing = Mathf.Clamp(halfHeight * 0.16f, 0.7f, 2.5f);

        Vector2 targetStartPos = new(0, Mathf.Clamp(halfHeight * 0.7f, 2f, halfHeight - 0.5f));
        float spawnHeight = halfHeight + 2f; // Spawn off-screen phía trên

        for (int row = 0; row < enemiesPerRow.Length; row++)
        {
            int count = enemiesPerRow[row];
            float rowWidth = (count - 1) * xSpacing;
            rowWidth = Mathf.Min(rowWidth, usableHalfWidth * 2f);
            float xStart = -rowWidth / 2f; // Center the row

            for (int col = 0; col < count; col++)
            {
                float xPos = Mathf.Clamp(xStart + col * xSpacing, -usableHalfWidth, usableHalfWidth);
                Vector2 targetPos = new(xPos, targetStartPos.y - row * ySpacing);
                Vector2 spawnPos = new(targetPos.x, spawnHeight);

                GameObject enemy = Instantiate(
                    planeDatabase
                        .enemyPlanes[Random.Range(0, planeDatabase.enemyPlanes.Count)]
                        .enemyPrefab,
                    spawnPos,
                    Quaternion.Euler(0, 0, 180f)
                );

                // Dynamically add movement only for grid pattern
                EnemyGridMover mover = enemy.AddComponent<EnemyGridMover>();
                mover.SetTargetPosition(targetPos);
            }
        }
        isSpawning = false;
        gameManager.blockControl = false;
    }

    private void SpawnBoss()
    {
        gameManager.blockControl = true;
        waveName.gameObject.SetActive(false);
        isSpawning = true;
        float halfHeight = Camera.main.orthographicSize;
        float halfWidth = halfHeight * Camera.main.aspect;
        float xOffset = Mathf.Max(1.5f, (halfWidth - edgeMargin) * 0.5f);

        GameObject bossPrefabA = bossPrefabs[Random.Range(0, bossPrefabs.Length)];
        GameObject bossPrefabB = bossPrefabs[Random.Range(0, bossPrefabs.Length)];

        GameObject bossA = Instantiate(
            bossPrefabA,
            new Vector3(-xOffset, halfHeight + 2f, 0),
            Quaternion.Euler(0, 0, 180f)
        );
        GameObject bossB = Instantiate(
            bossPrefabB,
            new Vector3(xOffset, halfHeight + 2f, 0),
            Quaternion.Euler(0, 0, 180f)
        );
        bossA.tag = "Boss";
        bossB.tag = "Boss";
        // Đảm bảo có BossHealth để nhận sát thương
        if (bossA.GetComponent<BossHealth>() == null) bossA.AddComponent<BossHealth>();
        if (bossB.GetComponent<BossHealth>() == null) bossB.AddComponent<BossHealth>();

        // Optional: add boss movement into scene
        BossMover bossMoverA = bossA.AddComponent<BossMover>();
        bossMoverA.targetPosition = new Vector3(-xOffset * 0.8f, Mathf.Max(halfHeight * 0.33f, 2f), 0);
        BossMover bossMoverB = bossB.AddComponent<BossMover>();
        bossMoverB.targetPosition = new Vector3(xOffset * 0.8f, Mathf.Max(halfHeight * 0.33f, 2f), 0);
        isSpawning = false;
        gameManager.blockControl = false;
    }

    private IEnumerator WaitForBossesDefeated()
    {
        while (true)
        {
            var bosses = GameObject.FindGameObjectsWithTag("Boss");
            if (bosses == null || bosses.Length == 0)
                yield break;
            yield return null;
        }
    }

    private float ComputeArcSpacing(int enemyCount)
    {
        float halfHeight = Camera.main.orthographicSize;
        float halfWidth = halfHeight * Camera.main.aspect;
        float usableWidth = Mathf.Max(0.1f, (halfWidth - edgeMargin) * 2f);
        if (enemyCount <= 1) return 0f;
        // Phân bố đều trong bề rộng khả dụng, với kẹp để không quá thưa hay quá dày
        float spacing = usableWidth / (enemyCount - 1);
        return Mathf.Clamp(spacing, 0.8f, Mathf.Max(1.2f, usableWidth * 0.4f));
    }
}
