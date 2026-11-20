using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class NormalBullet : MonoBehaviour
{
    [SerializeField] float speed = 10f;
    [SerializeField] float damage = 8f;
    [SerializeField] float lifetime = 3f;

    public float Damage { get { return damage; } }

    void Start()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
    }

    void Update()
    {
        // 上方向に直進させる
        transform.position += Vector3.up * speed * Time.deltaTime;

        // 一定時間経過で自動削除
        lifetime -= Time.deltaTime;
        if (lifetime <= 0f)
        {
            Destroy(gameObject);
        }
    }

    // ボスに当たった時の処理
    void OnTriggerEnter2D(Collider2D other)
    {
        BossController boss = other.GetComponent<BossController>();
        if (boss)
        {
            boss.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}