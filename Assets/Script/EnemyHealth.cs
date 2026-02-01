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
        // 1. ナビメッシュを完全に無効化する
        UnityEngine.AI.NavMeshAgent agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent != null)
        {
            agent.isStopped = true;       // 移動停止命令
            agent.velocity = Vector3.zero; // 現在持っている慣性をゼロにする
            agent.enabled = false;        // コンポーネント自体をOFFにする（これ以降の更新を止める）
        }

        // 2. 死亡アニメーションの再生
        Animator anim = GetComponent<Animator>();
        if (anim != null)
        {
            anim.SetTrigger("Die");
        }

        // 3. 他のスクリプト（AIの思考など）も止める
        if (GetComponent<EnemyPatrol>() != null)
        {
            GetComponent<EnemyPatrol>().enabled = false;
        }

        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false; // 死体すり抜けを可能にする
        }

        // 4. キャラクター削除の予約
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