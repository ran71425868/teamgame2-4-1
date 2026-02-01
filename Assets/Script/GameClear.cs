using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameClear : MonoBehaviour
{
    public static GameClear instance;

    public int remainingEnemies;
    public GameObject clearImageUI; // ここにさっき作ったImageを入れる

    void Awake()
    {
        if (instance == null) instance = this;
    }

    void Start()
    {
        // 最初は画像を隠しておく
        if (clearImageUI != null) clearImageUI.SetActive(false);
    }

    public void EnemyDefeated()
    {
        remainingEnemies--;
        if (remainingEnemies <= 0)
        {
            Gameclear();
        }
    }

    void Gameclear()
    {
        // 画像を表示！
        if (clearImageUI != null) clearImageUI.SetActive(true);

        // ゲームを止める（スローにしたり止めたくない場合は消してOK）
        Time.timeScale = 0;
    }
}
