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

        // 1. 死亡アニメーションを再生
        if (GetComponent<Animator>() != null)
        {
            GetComponent<Animator>().SetTrigger("Die"); // Animatorで"Die"トリガーを設定してください
        }

        // 2. AIやナビゲーションを止める（倒れた後も追いかけてくるのを防ぐ）
        if (GetComponent<UnityEngine.AI.NavMeshAgent>() != null)
        {
            GetComponent<UnityEngine.AI.NavMeshAgent>().isStopped = true;
        }

        // このスクリプト自体を無効にして、死体にさらにダメージが入るのを防ぐ
        this.enabled = false;

        // 3. 2秒＋アニメーション時間を考慮して削除
        // ここでは「命令を出してから4秒後」に削除するように設定（アニメ2秒＋余韻2秒）
        Destroy(gameObject, 4.0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        // 当たった相手のタグが "PlayerWeapon" の場合だけ実行
        if (other.CompareTag("Enemy"))
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