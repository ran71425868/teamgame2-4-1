using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement; // シーン移動に必要
using UnityEngine.UI; // UI操作に必要

public class GameManager : MonoBehaviour
{
    public static GameManager instance; // どこからでも呼べるようにする
    public TextMeshProUGUI enemyText;
    private int enemyCount;
    public GameObject pausePanel; // インスペクターでPausePanelをドラッグ
    public GameObject settingsPanel; // 設定画面のパネル
    public static bool isPaused = false;
    public FPSCameraController cameraScript; // インスペクターでPlayerをドラッグ
    public Slider sensitivitySlider;         // インスペクターでSliderをドラッグ
    public Text sensitivityValueText;   // 感度表示用テキスト

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        // シーン開始時に少しだけ遅らせて数える（敵の生成を待つため）
        Invoke("UpdateEnemyCount", 0.1f);

        // ゲーム開始時にスライダーの初期値を設定
        if (sensitivitySlider != null && cameraScript != null)
        {
            sensitivitySlider.value = cameraScript.sensitivity;

            // 初期表示を更新
            UpdateSensitivityText(sensitivitySlider.value);
            // スライダーが動いた時に実行するメソッドを登録
            sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);
        }
    }

    // シーン内の"Enemy"タグがついた敵を数え直す
    public void UpdateEnemyCount()
    {
        // "Enemy"タグを持つオブジェクトをすべて検索
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        enemyCount = enemies.Length;

        // UI表示を更新
        if (enemyText != null)
        {
            enemyText.text = "残り敵数: " + enemyCount;
        }

        // 0人になった時の処理（例：クリア！）
        if (enemyCount <= 0)
        {
            enemyText.text = "全滅！クリア！";
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) ResumeGame();
            else PauseGame();
        }
    }

    // スライダーが動くたびに実行される
    public void OnSensitivityChanged(float value)
    {
        if (cameraScript != null)
        {
            cameraScript.SetSensitivity(value);

            // 数値表示を更新
            UpdateSensitivityText(value);
        }
    }

    // 感度表示テキストを更新する
    void UpdateSensitivityText(float value)
    {
        if (sensitivityValueText != null)
        {
            float truncatedValue = Mathf.Floor(value * 100f) / 100f;
            sensitivityValueText.text = truncatedValue.ToString("0.00");
        }
    }

    // --- 設定画面を開く ---
    public void OpenSettings()
    {
        pausePanel.SetActive(false);    // メインメニューを隠す
        settingsPanel.SetActive(true); // 設定画面を表示
    }

    // --- 設定画面から戻る ---
    public void CloseSettings()
    {
        settingsPanel.SetActive(false); // 設定画面を隠す
        pausePanel.SetActive(true);    // メインメニューを表示
    }


    // ゲーム再開
    public void ResumeGame()
    {
        isPaused = false;
        pausePanel.SetActive(false); // パネルを隠す
        settingsPanel.SetActive(false); // 設定画面も閉じる
        Time.timeScale = 1f;         // 時間を動かす
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // ゲーム停止
    void PauseGame()
    {
        isPaused = true;
        pausePanel.SetActive(true);  // パネルを表示
        Time.timeScale = 0f;         // 時間を止める
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // タイトルへ戻るボタン
    public void GoToTitle()
    {
        isPaused = false;
        Time.timeScale = 1f; // 重要：時間を戻してからシーン移動
        SceneManager.LoadScene("TitleScene"); // タイトルシーンの名前に合わせて変更
    }
}
