using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class EnemyBullet : MonoBehaviour
{
    // 弾の状態（プレイヤーへ飛ぶ / キャッチ中 / 投げ返し中）
    public enum State { FlyingToPlayer, Captured, Returned }

    [SerializeField] float enemySpeed = 3.1f;
    [SerializeField] float captureDistance = 0.78f;
    [SerializeField] float returnedDamage = 40f;
    [SerializeField] float returnedLifetime = 5f;

    State state = State.FlyingToPlayer;
    PlayerController player;
    Vector2 direction;
    float lifeTimer;

    public bool IsReturned { get { return state == State.Returned; } }
    public float ReturnedDamage { get { return returnedDamage; } }

    void Start()
    {
        player = FindFirstObjectByType<PlayerController>();

        // 物理挙動で勝手に落ちないように設定
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
    }

    void Update()
    {
        if (state == State.FlyingToPlayer)
        {
            if (!player) player = FindFirstObjectByType<PlayerController>();
            if (!player) return;

            // プレイヤーに向かって移動
            Vector2 toPlayer = (Vector2)player.transform.position - (Vector2)transform.position;
            if (toPlayer.sqrMagnitude > 0.001f)
            {
                direction = toPlayer.normalized;
            }
            else
            {
                direction = Vector2.left;
            }
            transform.position += (Vector3)(direction * enemySpeed * Time.deltaTime);

            // 一定距離まで近づいたらキャッチ判定を行う
            if (toPlayer.magnitude <= captureDistance)
            {
                // 所持上限などでキャッチできなければ消す
                if (!player.TryCapture(this))
                {
                    Destroy(gameObject);
                }
            }
        }
        else if (state == State.Returned)
        {
            // 投げ返された後は指定方向へ直進し、時間経過で消える
            transform.position += (Vector3)(direction * enemySpeed * Time.deltaTime);
            lifeTimer -= Time.deltaTime;
            if (lifeTimer <= 0f)
            {
                Destroy(gameObject);
            }
        }
    }

    // プレイヤーにキャッチされた状態にする
    public void SetCaptured(PlayerController owner)
    {
        player = owner;
        state = State.Captured;
    }

    // キャッチ中、プレイヤーの周囲を回る位置を更新する
    public void SetOrbitPosition(Vector3 position)
    {
        if (state == State.Captured)
        {
            transform.position = position;
        }
    }

    // ボスに向かって投げ返す処理
    public void ReturnToEnemy(Vector2 launchDirection, float speed)
    {
        state = State.Returned;
        direction = launchDirection.normalized;
        enemySpeed = speed;
        lifeTimer = returnedLifetime;
    }

    // 投げ返した弾がボスに当たった時のダメージ処理
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsReturned) return;

        BossController boss = other.GetComponent<BossController>();
        if (boss)
        {
            boss.TakeDamage(returnedDamage);
            Destroy(gameObject);
        }
    }
}