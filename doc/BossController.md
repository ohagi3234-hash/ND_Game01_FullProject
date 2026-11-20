# BossController

対象スクリプト：`Assets/Scripts/BossController.cs`

## 役割

ボスの上下移動、プレイヤーへの敵弾発射、HP管理を行います。

## 処理の流れ

1. `Start()` でHPを `maxHealth` に設定し、開始位置を記録します。
2. `Update()` で開始位置を基準に上下へ往復し、`fireTimer` が0になるたびに `FireAtPlayer()` を呼びます。
3. `FireAtPlayer()` はプレイヤーを探し、その方向へ敵弾プレハブを生成します。プレイヤーまたはプレハブがない場合は発射しません。
4. `TakeDamage()` がHPを減らします。HPが0になると撃破メッセージを表示し、ボスを削除します。
5. ボスのトリガー接触処理は通常弾を見つけると、その弾の `Damage` を使ってHPを減らします。

## 実際のコード

### 上下に動く

```csharp
float offsetY = Mathf.Sin(Time.time * 1.15f) * 1.05f;
transform.position = startPosition + Vector3.up * offsetY;
```

`Mathf.Sin` は時間に応じて-1から1まで変化する値を返します。これに振幅1.05を掛けて、開始位置を中心に上下へ揺らします。

### プレイヤーへ弾を発射する

```csharp
Vector2 dir = ((Vector2)player.transform.position - (Vector2)transform.position).normalized;
Vector3 spawn = transform.position + (Vector3)(dir * spawnOffset);
Instantiate(enemyBulletPrefab, spawn, Quaternion.identity);
```

プレイヤーの位置からボスの位置を引くと、ボスからプレイヤーへ向かうベクトルになります。正規化した方向へ少し離した位置に敵弾プレハブを生成します。

### HPを減らして撃破を判定する

```csharp
public void TakeDamage(float damage)
{
    health = Mathf.Max(0f, health - damage);
    if (health <= 0f)
    {
        GameHUD.ShowMessage("BOSS DEFEATED!");
        Destroy(gameObject);
    }
}
```

現在HPからダメージを引きます。`Mathf.Max` によってHPがマイナスにならないようにし、0以下ならメッセージを表示してボスを削除します。

## 主な調整値

- `maxHealth`：開始時のHP
- `fireInterval`：敵弾を発射する間隔
- `spawnOffset`：ボスから敵弾を出す距離
- `normalBulletDamage`：現行コードでは参照されていない値です。通常弾のダメージは弾側の `Damage` を使います。

## 関連クラス

敵弾の移動と撃ち返し後の処理は [EnemyBullet](EnemyBullet.md)、画面のHP表示は [GameHUD](GameHUD.md) が担当します。
