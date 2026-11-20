# クラス別の仕組み説明書

このフォルダーには、ゲームを構成するクラスごとの説明をまとめています。各ページは対応するスクリプトの役割、処理の流れ、Inspectorで調整できる主な値を説明します。

## 説明書一覧

- [PlayerController](PlayerController.md)：プレイヤー移動、通常弾発射、敵弾の捕獲と投げ返し
- [NormalBullet](NormalBullet.md)：プレイヤーの通常弾の移動と寿命
- [EnemyBullet](EnemyBullet.md)：敵弾の追尾、捕獲中、投げ返し中の動作
- [BossController](BossController.md)：ボスの移動、攻撃、HPと被ダメージ
- [GameHUD](GameHUD.md)：画面上のゲーム情報表示
- [NDGameSetup](NDGameSetup.md)：Unity Editorでシーンとプレハブを用意する処理

スクリプト本体は `Assets/Scripts/` と `Assets/Editor/` にあります。説明書は初心者向けの概要です。数値や動作を変更する場合は、スクリプトとUnity Inspectorの設定も確認してください。
