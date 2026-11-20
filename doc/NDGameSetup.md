# NDGameSetup

対象スクリプト：`Assets/Editor/NDGameSetup.cs`

## 役割

Unity Editor専用のセットアップ処理です。ゲームシーンがまだない場合の自動作成と、メニューからシーンや弾プレハブを作り直す機能を提供します。`Assets/Editor/` 内にあるため、ゲーム本体の実行ファイルには通常含まれません。

## 処理の流れ

1. Editor起動後、`AutoCreateSceneIfMissing()` を遅延実行します。`Assets/Scenes/Main.unity` がなければシーン作成を始めます。
2. `EnsureFolders()` が `Assets/Scenes`、`Assets/Art`、`Assets/Prefabs` を用意します。
3. `CreateSprite()` が簡単な図形の画像を作成し、UnityのSpriteとしてインポートします。
4. `MakeBulletPrefab()` が敵弾と通常弾のプレハブを作ります。
5. 空のシーンを作り、カメラ、プレイヤー、ボス、HUD、壁を配置します。プレイヤーとボスには各コントローラーを追加し、弾プレハブを割り当てます。
6. シーンを保存し、Build Settings（ビルド対象シーン一覧）にも登録します。

## 実際のコード

### Unity起動時に不足シーンを確認する

```csharp
[InitializeOnLoad]
public static class NDGameSetup
{
    static NDGameSetup()
    {
        EditorApplication.delayCall += AutoCreateSceneIfMissing;
    }
}
```

`InitializeOnLoad` によりEditor読み込み時にクラスが初期化されます。`delayCall` に登録した関数は、Editorの初期処理後に呼ばれます。`AutoCreateSceneIfMissing()` はシーンファイルがない場合にだけ作成します。

### プレハブにコンポーネントを付けて保存する

```csharp
CircleCollider2D col = go.AddComponent<CircleCollider2D>();
col.isTrigger = true;

Rigidbody2D rb = go.AddComponent<Rigidbody2D>();
rb.bodyType = RigidbodyType2D.Kinematic;
rb.gravityScale = 0f;

if (enemy)
    go.AddComponent<EnemyBullet>();
else
    go.AddComponent<NormalBullet>();

GameObject prefab = PrefabUtility.SaveAsPrefabAsset(go, path);
```

弾のGameObjectに当たり判定とRigidbody2Dを追加し、種類に応じたスクリプトを付けてPrefabとして保存します。`isTrigger` が有効なので、衝突で押し返す代わりに `OnTriggerEnter2D` が呼ばれる構成です。

### シーンを保存してビルド対象にする

```csharp
EditorSceneManager.SaveScene(scene, ScenePath);
EditorBuildSettings.scenes = new[]
{
    new EditorBuildSettingsScene(ScenePath, true)
};
AssetDatabase.SaveAssets();
```

作成したシーンを `Main.unity` として保存し、ビルドで使うシーン一覧に登録します。最後にUnityのアセット変更を保存します。

## メニュー

`ND_Game01` メニューからシーン再作成を選ぶと、既存の `Main.unity` を含む現在のシーンを新しいゲームシーンに置き換えて保存します。手作業でシーンを編集している場合、再作成するとその編集内容は失われるため注意してください。
