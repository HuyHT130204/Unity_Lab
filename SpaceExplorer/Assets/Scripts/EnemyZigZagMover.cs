using UnityEngine;

public class EnemyZigZagMover : MonoBehaviour
{
    [SerializeField]
    private float downwardSpeed = 1.6f; // chậm hơn cho cảm giác tự nhiên
    [SerializeField]
    private float horizontalSpeed = 1.2f; // tốc độ ngang nhẹ

    private float minY;
    private float maxY;
    private float settleY;
    private float leftBound;
    private float rightBound;
    private int horizontalDirection = 1;

    void Start()
    {
        float halfHeight = Camera.main.orthographicSize;
        float halfWidth = halfHeight * Camera.main.aspect;
        // Chỉ cho phép di chuyển trong nửa màn hình từ tâm
        float allowedHalf = Mathf.Max(0.1f, (halfWidth * 0.5f) - 0.3f);

        leftBound = -allowedHalf;
        rightBound = allowedHalf;

        maxY = halfHeight + 2f;
        minY = -halfHeight - 2f;
        // Vị trí dừng theo trục Y (trong vùng nhìn thấy), lắc ngang tại đây
        settleY = Mathf.Clamp(halfHeight * 0.6f, 1.5f, halfHeight - 0.5f);
    }

    void Update()
    {
        // Di chuyển dọc xuống tới settleY, sau đó giữ Y cố định
        Vector3 pos = transform.position;
        if (pos.y > settleY)
        {
            pos.y -= downwardSpeed * Time.deltaTime;
            if (pos.y < settleY) pos.y = settleY;
        }

        // Di chuyển ngang qua lại trong [-allowedHalf, allowedHalf]
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

        // Không tự hủy theo Y để tránh qua màn sớm khi chưa tiêu diệt
    }
}

