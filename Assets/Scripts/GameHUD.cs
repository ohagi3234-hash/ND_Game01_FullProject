using UnityEngine;

public class GameHUD : MonoBehaviour
{
    static string message = "";
    static float messageUntil;

    // 画面中央にメッセージを一定時間表示する
    public static void ShowMessage(string value)
    {
        message = value;
        messageUntil = Time.time + 4f;
    }

    void OnGUI()
    {
        // 文字スタイルの設定
        GUIStyle title = new GUIStyle(GUI.skin.label);
        title.fontSize = 22;
        title.fontStyle = FontStyle.Bold;
        title.normal.textColor = Color.white;

        GUIStyle info = new GUIStyle(GUI.skin.label);
        info.fontSize = 16;
        info.normal.textColor = Color.white;

        PlayerController player = FindFirstObjectByType<PlayerController>();
        BossController boss = FindFirstObjectByType<BossController>();

        // 左上にタイトルと操作説明を表示
        GUI.Label(new Rect(18, 14, 600, 32), "CATCH & RETURN  /  ND_Game01", title);
        GUI.Label(new Rect(20, 50, 520, 28), "Left Click: Shoot    Right Click: Return captured bullets", info);

        if (player)
        {
            GUI.Label(new Rect(20, 78, 350, 28), $"Captured Bullets: {player.CapturedCount} / {player.MaxCapturedBullets}", info);
        }

        // 右上にボスのHP数値とHPバーを表示
        if (boss)
        {
            GUI.Label(new Rect(Screen.width - 300, 18, 280, 28), $"BOSS HP: {boss.Health:0} / {boss.MaxHealth:0}", info);

            Rect bar = new Rect(Screen.width - 300, 50, 260, 16);
            GUI.color = new Color(0.15f, 0.15f, 0.15f);
            GUI.DrawTexture(bar, Texture2D.whiteTexture);

            GUI.color = Color.red;
            GUI.DrawTexture(new Rect(bar.x, bar.y, bar.width * boss.Health / boss.MaxHealth, bar.height), Texture2D.whiteTexture);
            GUI.color = Color.white;
        }

        // 撃破メッセージなどの表示
        if (Time.time < messageUntil)
        {
            GUIStyle center = new GUIStyle(title);
            center.alignment = TextAnchor.MiddleCenter;
            center.fontSize = 34;
            GUI.Label(new Rect(0, Screen.height * 0.38f, Screen.width, 60), message, center);
        }
    }
}