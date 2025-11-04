using UnityEngine;

public class BossMover : MonoBehaviour
{
    public Vector3 targetPosition;
    public float speed = 2f;
    public float horizontalSpeed = 1.0f; // di chuyển ngang chậm

    private bool reachedTarget;
    private float leftBound;
    private float rightBound;
    private int horizontalDirection = 1;

    private EnemySpawner enemySpawner;

    void Awake()
    {
        enemySpawner = EnemySpawner.Instance;
    }

    void Update()
    {
        if (!reachedTarget)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                speed * Time.deltaTime
            );

            if (Vector3.Distance(transform.position, targetPosition) < 0.05f)
            {
                reachedTarget = true;
                float halfHeight = Camera.main.orthographicSize;
                float halfWidth = halfHeight * Camera.main.aspect;
                float allowedHalf = Mathf.Max(0.1f, (halfWidth * 0.5f) - 0.3f);
                leftBound = -allowedHalf;
                rightBound = allowedHalf;
            }
        }
        else
        {
            Vector3 pos = transform.position;
            pos.x += horizontalDirection * horizontalSpeed * Time.deltaTime;
            if (pos.x >= rightBound)
            {
                pos.x = rightBound;
                horizontalDirection = -1;
            }
            else if (pos.x <= leftBound)
            {
                pos.x = leftBound;
                horizontalDirection = 1;
            }
            transform.position = pos;
        }
    }
}
