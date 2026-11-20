# NormalBullet

対象スクリプト：`Assets/Scripts/NormalBullet.cs`

## 役割

プレイヤーが左クリックで発射する通常弾です。現在の実装では、生成位置から右方向へ直進し、設定時間が過ぎると消えます。

## 処理の流れ

1. `PlayerController` が通常弾プレハブをプレイヤーの右側に生成します。
2. `Start()` で `Rigidbody2D` を重力なしの Kinematic（物理演算で押されたり落下したりしない設定）にします。
3. 毎フレーム `Update()` で `Vector3.right` 方向へ `speed × Time.deltaTime` 移動します。`Time.deltaTime` を掛けることで、フレームレートが違っても時間あたりの移動距離をそろえます。
4. `lifetime` が0以下になると弾を削除します。
5. ボスとのトリガー接触で `BossController.TakeDamage()` を呼び、弾を削除します。

## 実際のコード

### 右方向へ移動する

```csharp
void Update()
{
    transform.position += Vector3.right * speed * Time.deltaTime;

    lifetime -= Time.deltaTime;
    if (lifetime <= 0f)
    {
        Destroy(gameObject);
    }
}
```

`Vector3.right` は右向きの単位ベクトルです。そこに速度と経過時間を掛けた分だけ、毎フレーム位置を更新します。`lifetime` は毎フレーム経過時間分だけ減り、0以下になれば `Destroy` で弾を消します。

### ボスに当たったとき

```csharp
void OnTriggerEnter2D(Collider2D other)
{
    BossController boss = other.GetComponent<BossController>();
    if (boss)
    {
        boss.TakeDamage(damage);
        Destroy(gameObject);
    }
}
```

`OnTriggerEnter2D` は2Dトリガーに触れたときUnityから呼ばれる関数です。接触相手に `BossController` が付いていれば、設定したダメージを渡し、弾を消します。

## 主な調整値

- `speed`：弾の移動速度
- `damage`：ボスに与えるダメージ値（初期値8）
- `lifetime`：弾が存在する秒数

## 注意点

現状は通常弾側の `OnTriggerEnter2D()` と、ボス側の同名処理の両方で通常弾のダメージを処理しています。Unityが両方の接触イベントを呼ぶ構成では、ダメージが二重になる可能性があります。実際のゲーム内ダメージを調整する際は、両方の処理を確認してください。
