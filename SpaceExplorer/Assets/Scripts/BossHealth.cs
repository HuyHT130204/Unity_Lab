using UnityEngine;

public class BossHealth : MonoBehaviour
{
    [SerializeField]
    private int maxHealth = 200;
    public int CurrentHealth { get; private set; }

    [SerializeField]
    private GameObject coinPrefab;
    [SerializeField]
    private GameObject[] collectables;

    void Awake()
    {
        CurrentHealth = maxHealth;
        gameObject.tag = "Boss"; // đảm bảo tag đúng
    }

    public void TakeDamage(int amount)
    {
        if (amount <= 0) return;
        CurrentHealth -= amount;
        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Rơi coin/collectable tương tự enemy
        if (coinPrefab != null && Random.value <= 0.88f)
        {
            Instantiate(coinPrefab, transform.position + Vector3.down * 0.5f, Quaternion.identity);
        }
        if (collectables != null && collectables.Length > 0)
        {
            int randomChance = Random.Range(1, 101);
            if (randomChance <= 30)
            {
                Instantiate(collectables[Random.Range(0, collectables.Length)], transform.position + Vector3.up * 0.5f, Quaternion.identity);
            }
        }
        Destroy(gameObject);
    }
}

