# PlayerController

対象スクリプト：`Assets/Scripts/PlayerController.cs`

## 役割

プレイヤーを操作し、通常弾を発射します。また、敵弾を捕獲して周囲に並べ、右クリックでまとめてボスへ投げ返します。

## 処理の流れ

1. `Awake()` で `MainCamera` を取得します。
2. 毎フレーム `Update()` から `MoveWithMouse()` を呼び、プレイヤーをマウス位置へ移動します。画面端から `screenPadding` 分内側に位置を制限します。
3. 捕獲弾があれば回転角を更新し、`UpdateOrbit()` でプレイヤーの周囲に等間隔で配置します。
4. 左クリック時、`shotTimer` が0以下なら `FireNormalBullet()` を実行します。弾はプレイヤーの右側に生成されます。発射後は `shotInterval` の間隔を空けます。
5. 右クリック時、`ThrowAllCaptured()` が捕獲中の弾をボスの方向へ発射します。ボスが見つからない場合の方向は右です。

## 実際のコード

### 左クリックを調べて発射する

```csharp
shotTimer -= Time.deltaTime;
if (Input.GetMouseButtonDown(0) && shotTimer <= 0f)
{
    FireNormalBullet();
    shotTimer = shotInterval;
}
```

`GetMouseButtonDown(0)` は左ボタンを押した瞬間だけ `true` になります。タイマーが0以下の場合に発射し、タイマーを `shotInterval` に戻して連射間隔を作ります。

### プレイヤーの右側に通常弾を作る

```csharp
void FireNormalBullet()
{
    if (!normalBulletPrefab) return;

    Vector3 spawn = transform.position + Vector3.right * 0.55f;
    Instantiate(normalBulletPrefab, spawn, Quaternion.identity);
}
```

`transform.position` はプレイヤーの位置です。`Vector3.right * 0.55f` を足して、右へ0.55 Unity単位ずらした場所を作っています。`Instantiate` はその位置にプレハブの実体を生成します。実際に飛ぶ向きは弾側のスクリプトが決めます。

### 捕獲弾を登録する

```csharp
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
```

弾が存在しない、所持上限に達した、同じ弾がすでに登録済み、のいずれかなら失敗を返します。それ以外はリストに追加し、弾へ「捕獲中」に切り替えるよう伝えます。

### 捕獲弾の向きを決めて投げ返す

```csharp
BossController boss = FindFirstObjectByType<BossController>();
Vector2 direction = Vector2.right;
if (boss)
{
    direction = ((Vector2)boss.transform.position - (Vector2)transform.position).normalized;
}
```

ボスがいる場合、ボス位置からプレイヤー位置を引いて方向を求めます。`.normalized` は距離の長さを1にそろえ、向きだけを表すベクトルにします。その後、各弾にこの方向と `returnSpeed` を渡します。

## 敵弾の捕獲

`TryCapture()` は、弾が有効で、上限未満で、まだ登録されていない場合にリストへ追加します。捕獲成功時に弾の `SetCaptured()` を呼び、弾自身の状態を捕獲中にします。

## 主な調整値

- `moveSpeed`：マウスへ近づく速さ
- `screenPadding`：画面端から空ける距離
- `maxCapturedBullets`：捕獲できる弾の上限
- `orbitRadius` / `orbitSpeed`：捕獲弾の周回半径と速さ
- `returnSpeed`：投げ返す弾の速さ
- `normalBulletPrefab`：通常弾として生成するプレハブ
- `shotInterval`：通常弾を発射できる間隔

## 関連クラス

捕獲対象と投げ返す弾は [EnemyBullet](EnemyBullet.md)、通常弾の移動は [NormalBullet](NormalBullet.md) が担当します。
