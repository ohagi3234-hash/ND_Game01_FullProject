# EnemyBullet

対象スクリプト：`Assets/Scripts/EnemyBullet.cs`

## 役割

ボスが発射する弾です。プレイヤーへ向かって飛び、近づくと捕獲されます。捕獲後はプレイヤーの周囲に留まり、投げ返されるとボスにダメージを与えます。

## 3つの状態

- `FlyingToPlayer`：プレイヤーの位置を追いながら移動します。`captureDistance` 以内に入ると `PlayerController.TryCapture()` を呼びます。捕獲に失敗した弾は削除されます。
- `Captured`：通常の移動を止めます。`PlayerController.UpdateOrbit()` から `SetOrbitPosition()` を受けて位置を更新します。
- `Returned`：`ReturnToEnemy()` で受け取った方向と速さで直進します。寿命が尽きるか、ボスに当たると削除されます。

## 実際のコード

### 状態を表す

```csharp
public enum State { FlyingToPlayer, Captured, Returned }
State state = State.FlyingToPlayer;
```

`enum` は選択肢に名前を付けた型です。敵弾は最初にプレイヤーへ飛ぶ状態で始まり、捕獲や投げ返しのタイミングで状態を切り替えます。

### プレイヤーへ飛び、近づいたら捕獲を頼む

```csharp
Vector2 toPlayer = (Vector2)player.transform.position - (Vector2)transform.position;
direction = toPlayer.sqrMagnitude > 0.001f ? toPlayer.normalized : Vector2.left;
transform.position += (Vector3)(direction * enemySpeed * Time.deltaTime);

if (toPlayer.magnitude <= captureDistance && !player.TryCapture(this))
{
    Destroy(gameObject);
}
```

`toPlayer` は弾からプレイヤーへ向かうベクトルです。長さがほぼ0でないときは `.normalized` で向きだけにし、弾を移動します。捕獲距離に入ったとき捕獲できなければ、弾を削除します。

### 投げ返し状態に切り替える

```csharp
public void ReturnToEnemy(Vector2 launchDirection, float speed)
{
    state = State.Returned;
    direction = launchDirection.normalized;
    enemySpeed = speed;
    lifeTimer = returnedLifetime;
}
```

プレイヤーから受け取った方向と速度を保存し、寿命タイマーを初期化します。次の `Update()` から投げ返し中の移動処理が使われます。

### 投げ返した弾だけダメージを与える

```csharp
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
```

最初の条件で投げ返し中以外の接触を無視します。ボスに当たった場合はダメージを与え、弾を消します。

## 接触時のダメージ

投げ返し中（`Returned`）だけ、ボスとのトリガー接触で `returnedDamage` を与えます。捕獲中の弾がボスに触れてもダメージ処理は行いません。

## 主な調整値

- `enemySpeed`：敵弾の追尾速度。投げ返し時には `PlayerController` から渡された速度に置き換わります。
- `captureDistance`：捕獲を試す距離
- `returnedDamage`：投げ返したときのダメージ（初期値40）
- `returnedLifetime`：投げ返し後に存在する秒数
