using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement; // シーン切り替えに必要
public class GameClear : MonoBehaviour
{
    public static GameClear instance;
    public int remainingEnemies;
    public GameObject clearImageUI; // Canvas Groupを付けた画像

    void Awake()
    {
        if (instance == null) instance = this;
    }

    public void SetEnemyCount(int count) => remainingEnemies = count;

    public void EnemyDefeated()
    {
        remainingEnemies--;
        // 敵が0になった瞬間にコルーチン開始
        if (remainingEnemies <= 0) StartCoroutine(ApexVictorySequence());
    }

    IEnumerator ApexVictorySequence()
    {
        // 1. プレイヤーをその場に完全固定（地面抜け防止）
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            // PlayerHealth.csのDisableControlsと同じ役割
            MonoBehaviour[] scripts = player.GetComponents<MonoBehaviour>();
            foreach (var s in scripts) s.enabled = false;

            Rigidbody rb = player.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.isKinematic = true;
            }
        }

        // 2. 倒した瞬間のスローモーション（APEXのフィニッシュ風）
        Time.timeScale = 0.2f;

        // 3. 画像のフェードイン処理
        if (clearImageUI != null)
        {
            clearImageUI.SetActive(true);
            CanvasGroup group = clearImageUI.GetComponent<CanvasGroup>();

            float elapsed = 0f;
            float duration = 1.5f; // 1.5秒かけて表示

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime; // スロー中も一定速度で計算
                if (group != null) group.alpha = elapsed / duration;

                // カメラをじわじわ引く演出
                Camera.main.fieldOfView = Mathf.Lerp(60, 55, elapsed / duration);
                yield return null;
            }
        }

        // 4. スローを戻す
        Time.timeScale = 1.0f;

        // 5. 操作不能のままカーソルだけ出す
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void BackToTitle()
    {
        // 念のためTimeScaleを1に戻してからシーン移動
        Time.timeScale = 1.0f;

        // "Title" は自分のタイトルシーンの名前に書き換えてください
        SceneManager.LoadScene("TitleScene");
    }
}