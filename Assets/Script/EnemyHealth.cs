using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    // ★追加: Rキーを押すとダメージを受ける（テスト用）
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            // 自分自身に20ダメージを与える
            TakeDamage(20);
        }
    }

    // ダメージを受ける関数
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"敵の残り体力: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("敵を倒しました！");
        // ここで倒れた時のアニメーション再生や、オブジェクト削除を行う
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        // 当たった相手のタグが "PlayerWeapon" の場合だけ実行
        if (other.CompareTag("PlayerWeapon"))
        {
            // ここでは一律10ダメージとしていますが、武器側にダメージ量を持たせることも可能です
            TakeDamage(10);

            // ヒットエフェクトなどをここに入れるとより良くなります
        }
    }

    public void AddHealth(int amount)
    {
        currentHealth += amount;

        // もし最大体力を超えて回復させたくない場合は、以下のコードを使います
        // currentHealth = Mathf.Min(currentHealth + amount, maxHealth);

        Debug.Log($"アーマー装着！体力が {amount} 増えた。現在の体力: {currentHealth}");
    }
}