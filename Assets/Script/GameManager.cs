using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // シーン移動に必要

public class GameManager : MonoBehaviour
{
    public GameObject pausePanel; // インスペクターでPausePanelをドラッグ
    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) ResumeGame();
            else PauseGame();
        }
    }

    // ゲーム再開
    public void ResumeGame()
    {
        isPaused = false;
        pausePanel.SetActive(false); // パネルを隠す
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

    // 設定ボタン（今はログを出すだけ）
    public void OpenSettings()
    {
        Debug.Log("設定画面を開く処理（UIの切り替えなど）");
    }

    // タイトルへ戻るボタン
    public void GoToTitle()
    {
        Time.timeScale = 1f; // 重要：時間を戻してからシーン移動
        SceneManager.LoadScene("TitleScene"); // タイトルシーンの名前に合わせて変更
    }
}
