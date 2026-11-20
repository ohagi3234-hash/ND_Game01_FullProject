# GameHUD

対象スクリプト：`Assets/Scripts/GameHUD.cs`

## 役割

画面に操作説明、捕獲弾数、ボスHP、ゲーム中のメッセージを表示します。Unityの `OnGUI()` を使って描画します。

## 表示内容

- 左上：ゲーム名と操作説明
- 左側：捕獲弾数（プレイヤーが存在する場合）
- 右上：ボスHPとHPバー（ボスが存在する場合）
- 画面中央付近：`ShowMessage()` で設定されたメッセージ

`ShowMessage(string value)` は表示する文字列を保存し、4秒後まで表示するよう期限を設定します。ボス撃破時に `BossController` から呼び出されます。

## 実際のコード

### 一定時間表示するメッセージを登録する

```csharp
public static void ShowMessage(string value)
{
    message = value;
    messageUntil = Time.time + 4f;
}
```

`static` メソッドなので、HUDオブジェクトを探さず `GameHUD.ShowMessage(...)` と呼べます。表示文字列と、表示終了時刻を保存します。

### 終了時刻まで画面に表示する

```csharp
if (Time.time < messageUntil)
{
    GUIStyle center = new GUIStyle(title);
    center.alignment = TextAnchor.MiddleCenter;
    center.fontSize = 34;
    GUI.Label(new Rect(0, Screen.height * 0.38f, Screen.width, 60), message, center);
}
```

現在時刻 `Time.time` が保存した終了時刻より前なら、画面中央寄りにメッセージを描画します。

### ボスHPのバーを描く

```csharp
GUI.color = Color.red;
GUI.DrawTexture(
    new Rect(bar.x, bar.y, bar.width * boss.Health / boss.MaxHealth, bar.height),
    Texture2D.whiteTexture);
GUI.color = Color.white;
```

赤い矩形の幅を `現在HP / 最大HP` の割合にしています。最後にGUI色を白へ戻し、後続の描画に赤色が残らないようにします。

## 動作上のポイント

`OnGUI()` は1フレーム内に複数回呼ばれることがあります。ここでは表示のたびにプレイヤーとボスを検索し、ラベルやHPバーを描画しています。
