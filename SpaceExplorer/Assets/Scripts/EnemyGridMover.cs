using UnityEngine;

public class EnemyGridMover : MonoBehaviour
{
    private Vector2 targetPosition;
    private float speed = 3f; // You can expose in Inspector if you want
    private bool movingIn = true;
    private GameManager gameManager;
    private float oscillationTime;
    [SerializeField]
    private float oscillationAmplitude = 0.8f; // biên độ nhỏ hơn, tự nhiên hơn
    [SerializeField]
    private float oscillationFrequency = 0.8f; // chậm hơn
    private float centerX;

    void Awake()
    {
        gameManager = GameManager.Instance;
    }

    public void SetTargetPosition(Vector2 target)
    {
        targetPosition = target;
    }

    void Update()
    {
        if (movingIn)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                targetPosition,
                speed * Time.deltaTime
            );

            if (Vector2.Distance(transform.position, targetPosition) < 0.05f)
            {
                transform.position = targetPosition;
                movingIn = false;
                centerX = targetPosition.x;
                oscillationTime = 0f;
            }
        }
        else
        {
            oscillationTime += Time.deltaTime;
            float halfHeight = Camera.main.orthographicSize;
            float halfWidth = halfHeight * Camera.main.aspect;
            // Chỉ cho phép di chuyển trong nửa màn hình tính từ tâm
            float allowedHalf = Mathf.Max(0.1f, (halfWidth * 0.5f) - 0.3f);

            // Dao động ngang quanh centerX
            float x = centerX + oscillationAmplitude * Mathf.Sin(2f * Mathf.PI * oscillationFrequency * oscillationTime);
            x = Mathf.Clamp(x, -allowedHalf, allowedHalf);
            transform.position = new Vector2(x, transform.position.y);
        }
    }
}
