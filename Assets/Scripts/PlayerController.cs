using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("移動")]
    [SerializeField] float moveSpeed = 12f;
    [SerializeField] float screenPadding = 0.55f;

    [Header("キャッチ＆投げ返し")]
    [SerializeField] int maxCapturedBullets = 5;
    [SerializeField] float orbitRadius = 1.05f;
    [SerializeField] float orbitSpeed = 170f;
    [SerializeField] float returnSpeed = 14f;

    [Header("通常弾")]
    [SerializeField] GameObject normalBulletPrefab;
    [SerializeField] float shotInterval = 0.25f;

    readonly List<EnemyBullet> captured = new List<EnemyBullet>();
    Camera cam;
    float orbitAngle;
    float shotTimer;

    public int CapturedCount { get { return captured.Count; } }
    public int MaxCapturedBullets { get { return maxCapturedBullets; } }

    void Awake()
    {
        cam = Camera.main;
    }

    void Update()
    {
        MoveWithMouse();

        // キャッチした弾を自分の周りで回転させる
        orbitAngle += orbitSpeed * Time.deltaTime;
        UpdateOrbit();

        // 左クリックで通常弾を発射
        shotTimer -= Time.deltaTime;
        if (Input.GetMouseButtonDown(0) && shotTimer <= 0f)
        {
            FireNormalBullet();
            shotTimer = shotInterval;
        }

        // 右クリックでキャッチ中の弾をまとめて投げ返す
        if (Input.GetMouseButtonDown(1))
        {
            ThrowAllCaptured();
        }
    }

    // マウスカーソルの位置に向かって移動（画面外に出ないよう制限付き）
    void MoveWithMouse()
    {
        if (!cam) return;

        Vector3 target = cam.ScreenToWorldPoint(Input.mousePosition);
        target.z = 0f;
        transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);

        Vector3 min = cam.ViewportToWorldPoint(new Vector3(0f, 0f, 0f));
        Vector3 max = cam.ViewportToWorldPoint(new Vector3(1f, 1f, 0f));

        Vector3 p = transform.position;
        p.x = Mathf.Clamp(p.x, min.x + screenPadding, max.x - screenPadding);
        p.y = Mathf.Clamp(p.y, min.y + screenPadding, max.y - screenPadding);
        transform.position = p;
    }

    // 敵弾をキャッチしてリストに追加する
    public bool TryCapture(EnemyBullet bullet)
    {
        if (!bullet || captured.Count >= maxCapturedBullets || captured.Contains(bullet))
        {
            return false;
        }

        captured.Add(bullet);
        bullet.SetCaptured(this);
        return true;
    }

    // キャッチした弾を等間隔の円周上に配置する
    void UpdateOrbit()
    {
        for (int i = captured.Count - 1; i >= 0; i--)
        {
            if (!captured[i])
            {
                captured.RemoveAt(i);
                continue;
            }

            float angle = (orbitAngle + i * 360f / Mathf.Max(1, captured.Count)) * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * orbitRadius;
            captured[i].SetOrbitPosition(transform.position + offset);
        }
    }

    // キャッチしている弾をすべてボスに向けて発射する
    void ThrowAllCaptured()
    {
        if (captured.Count == 0) return;

        BossController boss = FindFirstObjectByType<BossController>();
        Vector2 direction = Vector2.right;
        if (boss)
        {
            direction = ((Vector2)boss.transform.position - (Vector2)transform.position).normalized;
        }

        EnemyBullet[] bullets = captured.ToArray();
        captured.Clear();

        foreach (EnemyBullet bullet in bullets)
        {
            if (bullet)
            {
                bullet.ReturnToEnemy(direction, returnSpeed);
            }
        }
    }

    // 通常弾の生成
    void FireNormalBullet()
    {
        if (!normalBulletPrefab) return;

        Vector3 spawn = transform.position + Vector3.right * 0.55f;
        Instantiate(normalBulletPrefab, spawn, Quaternion.identity);
    }
}
