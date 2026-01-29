using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("体力設定")]
    public float maxHealth = 100f;
    private float currentHealth;

    public bool isDead = false; // 死亡フラグ
    private Armor armor;
    public HUDManager hudManager;

    void Start()
    {
        // ゲーム開始時に体力を全回復
        currentHealth = maxHealth;
        armor = GetComponent<Armor>();

        if (hudManager != null) hudManager.UpdateHP(currentHealth, maxHealth);

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            TakeDamage(10f);
        }
    }

    // ダメージを受ける関数（外部から呼び出す）
    public void TakeDamage(float amount)
    {
        if (isDead) return;

        int damage = Mathf.RoundToInt(amount);

        // アーマーで先に吸収
        if (armor != null)
        {
            damage = armor.AbsorbDamage(damage);
        }

        // 残りをHPへ
        if (damage > 0)
        {
            currentHealth -= damage;
            Debug.Log("HP : " + currentHealth);
        }
        if (hudManager != null) hudManager.UpdateHP(currentHealth, maxHealth);
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // 死亡処理
    void Die()
    {
        isDead = true;
        Debug.Log("プレイヤーが死亡しました");

        // ここで死亡時のアクションを実行
        // 例: 画面を赤くする、リロードする、入力を無効化するなど
        DisableControls();
    }

    // 死亡した時に入力を止める
    void DisableControls()
    {
        // 移動スクリプトを止める
        if (TryGetComponent<Player>(out Player movement))
        {
            movement.enabled = false;
        }

        // カメラ（回転）スクリプトを止める
        if (TryGetComponent<FPSCameraController>(out FPSCameraController cameraControl))
        {
            cameraControl.enabled = false;
        }

        // マウスカーソルを表示する
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
