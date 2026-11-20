# ND_Game01

Unity 6.3 LTS（6000.3.11f1）で作成した、弾を撃ち返す2Dゲームです。

## ゲームの遊び方

- マウスを動かしてプレイヤーを移動します。
- 左クリックで通常弾を発射します。通常弾はプレイヤーの右側から出て、右へ進みます。
- 敵の弾はプレイヤーに近づくと自動で捕まえます。捕まえられる弾は最大5発です。
- 右クリックで捕まえた弾をまとめてボスへ撃ち返します。
- 通常弾は8ダメージ、撃ち返した弾は40ダメージを与えます。

## コードの仕組み

クラスごとの処理や設定値は [`doc/`](doc/README.md) の説明書を参照してください。

## 主なスクリプト

| ファイル | 役割 |
| --- | --- |
| `Assets/Scripts/PlayerController.cs` | プレイヤー移動、通常弾発射、敵弾の捕獲と撃ち返し |
| `Assets/Scripts/NormalBullet.cs` | 通常弾の移動、ダメージ、寿命 |
| `Assets/Scripts/EnemyBullet.cs` | 敵弾の追尾、捕獲、撃ち返し後の移動 |
| `Assets/Scripts/BossController.cs` | ボスの移動、攻撃、HP管理 |
| `Assets/Scripts/GameHUD.cs` | ゲーム情報の画面表示 |
| `Assets/Editor/NDGameSetup.cs` | Unity Editor上でゲーム用シーンとプレハブを作成 |

各クラスの詳しい説明：[doc/README.md](doc/README.md)

## Unityで開く

Unity Hubからこのフォルダーを追加し、Unity 6.3 LTS（6000.3.11f1）で開いてください。ゲームシーンは `Assets/Scenes/Main.unity` です。
