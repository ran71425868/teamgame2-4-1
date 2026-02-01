using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameClear : MonoBehaviour
{
    public static GameClear instance;

    [Header("クリア条件")]
    public int targetDefeatCount = 10; // 10体倒したらクリア
    private int currentDefeatCount = 0;
    private bool isCleared = false;    // すでにクリア演出中かどうかのフラグ

    [Header("演出設定")]
    public GameObject clearImageUI;
    public AudioClip victorySound;
    private AudioSource audioSource;

    void Awake()
    {
        if (instance == null) instance = this;
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.ignoreListenerPause = true;
    }

    // 以前の関数との互換性のために残す（中身は空でOK）
    public void SetEnemyCount(int count) { }

    // 敵が死んだときに呼ばれる
    public void EnemyDefeated()
    {
        // すでにクリアしていたら、それ以上の通知は無視する（二重実行防止）
        if (isCleared) return;

        currentDefeatCount++;
        Debug.Log($"倒した数: {currentDefeatCount} / {targetDefeatCount}");

        if (currentDefeatCount >= targetDefeatCount)
        {
            isCleared = true; // クリアフラグを立てる
            StartCoroutine(ApexVictorySequence());
        }
    }

    IEnumerator ApexVictorySequence()
    {
        // 1. プレイヤー固定
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            MonoBehaviour[] scripts = player.GetComponents<MonoBehaviour>();
            foreach (var s in scripts) s.enabled = false;
            Rigidbody rb = player.GetComponent<Rigidbody>();
            if (rb != null) { rb.velocity = Vector3.zero; rb.isKinematic = true; }
        }

        // 2. スローモーション
        Time.timeScale = 0.2f;

        // 3. 音再生
        if (victorySound != null) audioSource.PlayOneShot(victorySound);

        // 4. UIフェードイン
        if (clearImageUI != null)
        {
            clearImageUI.SetActive(true);
            CanvasGroup group = clearImageUI.GetComponent<CanvasGroup>();
            float elapsed = 0f;
            while (elapsed < 1.5f)
            {
                elapsed += Time.unscaledDeltaTime;
                if (group != null) group.alpha = elapsed / 1.5f;
                Camera.main.fieldOfView = Mathf.Lerp(60, 55, elapsed / 1.5f);
                yield return null;
            }
        }

        // 5. 1秒待ってタイトルへ
        yield return new WaitForSecondsRealtime(1.0f);
        Time.timeScale = 3.0f;
        SceneManager.LoadScene("TitleScene");
    }
}