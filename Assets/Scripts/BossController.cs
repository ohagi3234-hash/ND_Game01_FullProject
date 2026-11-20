using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class BossController : MonoBehaviour
{
    [SerializeField] GameObject enemyBulletPrefab;
    [SerializeField] float maxHealth = 300f;
    [SerializeField] float fireInterval = 1.35f;
    [SerializeField] float spawnOffset = 0.7f;
    [SerializeField] float normalBulletDamage = 8f;

    float health;
    float fireTimer;
    Vector3 startPosition;

    // UI表示用に現在のHPと最大HPを公開
    public float Health { get { return health; } }
    public float MaxHealth { get { return maxHealth; } }

    void Start()
    {
        health = maxHealth;
        startPosition = transform.position;
        fireTimer = 0.7f;
    }

    void Update()
    {
        // 上下にゆらゆら移動させる
        float offsetY = Mathf.Sin(Time.time * 1.15f) * 1.05f;
        transform.position = startPosition + Vector3.up * offsetY;

        // 一定時間ごとに弾を発射
        fireTimer -= Time.deltaTime;
        if (fireTimer <= 0f)
        {
            FireAtPlayer();
            fireTimer = fireInterval;
        }
    }

    // プレイヤーの方向を狙って弾を撃つ
    void FireAtPlayer()
    {
        if (!enemyBulletPrefab) return;

        PlayerController player = FindFirstObjectByType<PlayerController>();
        if (!player) return;

        Vector2 dir = ((Vector2)player.transform.position - (Vector2)transform.position).normalized;
        Vector3 spawn = transform.position + (Vector3)(dir * spawnOffset);
        Instantiate(enemyBulletPrefab, spawn, Quaternion.identity);
    }

    // ダメージ処理
    public void TakeDamage(float damage)
    {
        health = Mathf.Max(0f, health - damage);
        if (health <= 0f)
        {
            GameHUD.ShowMessage("BOSS DEFEATED!");
            Destroy(gameObject);
        }
    }

    // プレイヤーの通常弾が当たった時の判定
    void OnTriggerEnter2D(Collider2D other)
    {
        NormalBullet normal = other.GetComponent<NormalBullet>();
        if (normal)
        {
            TakeDamage(normal.Damage);
            Destroy(normal.gameObject);
        }
    }
}