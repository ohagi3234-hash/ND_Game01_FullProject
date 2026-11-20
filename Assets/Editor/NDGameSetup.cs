#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

// エディタ起動時やメニューからゲーム用シーンを自動生成するスクリプト
[InitializeOnLoad]
public static class NDGameSetup
{
    const string ScenePath = "Assets/Scenes/Main.unity";

    static NDGameSetup()
    {
        EditorApplication.delayCall += AutoCreateSceneIfMissing;
    }

    // Mainシーンがまだ無ければ自動で作る
    static void AutoCreateSceneIfMissing()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode) return;

        if (!System.IO.Directory.Exists("Assets/Scenes"))
        {
            System.IO.Directory.CreateDirectory("Assets/Scenes");
        }
        if (!System.IO.File.Exists(ScenePath))
        {
            CreateGameScene(false);
        }
    }

    // 上部メニューから手動でシーンを作り直す用
    [MenuItem("ND_Game01/セットアップ/ゲームシーンを再作成")]
    public static void RecreateFromMenu()
    {
        CreateGameScene(true);
    }

    static void CreateGameScene(bool showDialog)
    {
        EnsureFolders();

        // 仮の図形スプライト画像を生成
        CreateSprite("Assets/Art/Player.png", new Color(0.15f, 0.88f, 1f), true);
        CreateSprite("Assets/Art/Boss.png", new Color(1f, 0.25f, 0.2f), false);
        CreateSprite("Assets/Art/EnemyBullet.png", new Color(1f, 0.82f, 0.18f), true);
        CreateSprite("Assets/Art/NormalBullet.png", new Color(0.35f, 1f, 0.48f), true);
        AssetDatabase.Refresh();

        Sprite playerSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Player.png");
        Sprite bossSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Boss.png");
        Sprite enemySprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/EnemyBullet.png");
        Sprite normalSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/NormalBullet.png");

        // 弾のプレハブを作成
        GameObject enemyPrefab = MakeBulletPrefab("EnemyBullet", enemySprite, true);
        GameObject normalPrefab = MakeBulletPrefab("NormalBullet", normalSprite, false);

        // 新規シーン作成
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // カメラ設定
        GameObject cameraGO = new GameObject("Main Camera");
        cameraGO.tag = "MainCamera";
        cameraGO.transform.position = new Vector3(0, 0, -10);
        Camera camera = cameraGO.AddComponent<Camera>();
        camera.orthographic = true;
        camera.orthographicSize = 5f;
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color(0.035f, 0.055f, 0.10f);
        cameraGO.AddComponent<AudioListener>();

        // プレイヤー配置とプレハブの紐付け
        GameObject player = MakeSpriteObject("Player", playerSprite, new Vector3(-3.2f, 0f, 0f), 0.75f);
        CircleCollider2D playerCollider = player.AddComponent<CircleCollider2D>();
        playerCollider.isTrigger = true;
        player.AddComponent<PlayerController>();

        SerializedObject playerSO = new SerializedObject(player.GetComponent<PlayerController>());
        playerSO.FindProperty("normalBulletPrefab").objectReferenceValue = normalPrefab;
        playerSO.ApplyModifiedProperties();

        // ボス配置とプレハブの紐付け
        GameObject boss = MakeSpriteObject("Boss", bossSprite, new Vector3(4f, 0f, 0f), 1.45f);
        BoxCollider2D bossCollider = boss.AddComponent<BoxCollider2D>();
        bossCollider.isTrigger = true;
        Rigidbody2D bossRB = boss.AddComponent<Rigidbody2D>();
        bossRB.bodyType = RigidbodyType2D.Kinematic;
        bossRB.gravityScale = 0f;
        BossController bossController = boss.AddComponent<BossController>();

        SerializedObject bossSO = new SerializedObject(bossController);
        bossSO.FindProperty("enemyBulletPrefab").objectReferenceValue = enemyPrefab;
        bossSO.ApplyModifiedProperties();

        // UI表示用オブジェクト
        GameObject hud = new GameObject("Game HUD");
        hud.AddComponent<GameHUD>();

        // 画面の枠（壁）を配置
        MakeWall("Top Wall", new Vector2(0, 5.35f), new Vector2(18f, 0.25f));
        MakeWall("Bottom Wall", new Vector2(0, -5.35f), new Vector2(18f, 0.25f));
        MakeWall("Left Wall", new Vector2(-9.1f, 0), new Vector2(0.25f, 10.7f));
        MakeWall("Right Wall", new Vector2(9.1f, 0), new Vector2(0.25f, 10.7f));

        // シーンを保存してビルド設定に登録
        EditorSceneManager.SaveScene(scene, ScenePath);
        EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
        AssetDatabase.SaveAssets();

        if (showDialog)
        {
            EditorUtility.DisplayDialog("ND_Game01", "Main.unity を作成しました。Assets/Scenes/Main.unity を開いて Play してください。", "OK");
        }
    }

    // 必要なフォルダが無ければ作成する
    static void EnsureFolders()
    {
        string[] folders = { "Assets/Scenes", "Assets/Art", "Assets/Prefabs" };
        foreach (string folder in folders)
        {
            if (!AssetDatabase.IsValidFolder(folder))
            {
                string parent = System.IO.Path.GetDirectoryName(folder).Replace("\\", "/");
                string name = System.IO.Path.GetFileName(folder);
                if (!AssetDatabase.IsValidFolder(parent))
                {
                    AssetDatabase.CreateFolder("Assets", name);
                }
                else
                {
                    AssetDatabase.CreateFolder(parent, name);
                }
            }
        }
    }

    static GameObject MakeSpriteObject(string name, Sprite sprite, Vector3 pos, float scale)
    {
        GameObject go = new GameObject(name);
        go.transform.position = pos;
        go.transform.localScale = Vector3.one * scale;
        go.AddComponent<SpriteRenderer>().sprite = sprite;
        return go;
    }

    // 敵弾・通常弾のプレハブを作成する
    static GameObject MakeBulletPrefab(string name, Sprite sprite, bool enemy)
    {
        float scale = enemy ? 0.28f : 0.22f;
        GameObject go = MakeSpriteObject(name, sprite, Vector3.zero, scale);

        CircleCollider2D col = go.AddComponent<CircleCollider2D>();
        col.isTrigger = true;

        Rigidbody2D rb = go.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;

        if (enemy)
        {
            go.AddComponent<EnemyBullet>();
        }
        else
        {
            go.AddComponent<NormalBullet>();
        }

        string path = "Assets/Prefabs/" + name + ".prefab";
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(go, path);
        Object.DestroyImmediate(go);
        return prefab;
    }

    static void MakeWall(string name, Vector2 pos, Vector2 size)
    {
        GameObject wall = new GameObject(name);
        wall.transform.position = pos;
        BoxCollider2D col = wall.AddComponent<BoxCollider2D>();
        col.size = size;
    }

    // 単色の丸または四角のテクスチャを書き出してSpriteにする
    static void CreateSprite(string path, Color color, bool circle)
    {
        const int size = 64;
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dx = (x + 0.5f - size / 2f) / (size / 2f);
                float dy = (y + 0.5f - size / 2f) / (size / 2f);
                bool inside = !circle || (dx * dx + dy * dy <= 0.9f);
                tex.SetPixel(x, y, inside ? color : Color.clear);
            }
        }
        tex.Apply();

        System.IO.File.WriteAllBytes(path, tex.EncodeToPNG());
        Object.DestroyImmediate(tex);

        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(path);
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.alphaIsTransparency = true;
        importer.filterMode = FilterMode.Bilinear;
        importer.SaveAndReimport();
    }
}
#endif