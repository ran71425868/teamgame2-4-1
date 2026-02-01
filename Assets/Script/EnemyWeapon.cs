using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeapon : MonoBehaviour
{
    public int damage = 10;
    private bool canDamage = false;
    public int active;

    // アニメーションから呼び出して判定をON/OFFする
    public void SetAttackActive(bool active)
    {
        canDamage = active;
        if (canDamage)
        {
            StopAllCoroutines(); // 前のタイマーがあればリセット
            StartCoroutine(AutoDisableDamage());
        }
    }

    // ★追加: 自動で判定を消すコルーチン
    IEnumerator AutoDisableDamage()
    {
        // 攻撃アニメーションの長さに合わせて時間は調整してください（0.5秒〜1.0秒が目安）
        yield return new WaitForSeconds(0.6f);
        canDamage = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        // 攻撃判定がONで、かつプレイヤーに当たった場合
        if (canDamage && other.CompareTag("Player"))
        {
            // ★修正: 武器を奪う処理（StealWeapon）のブロックを完全に削除しました。
            // これにより、敵が武器を持っていてもいなくても、プレイヤーの武器を奪うことはありません。

            // --- ダメージ処理 ---
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                // 攻撃者の特定（自分自身の親など）
                Transform attacker = transform.root;
                EnemyHealth enemySelf = GetComponentInParent<EnemyHealth>();

                if (enemySelf != null)
                {
                    attacker = enemySelf.transform;
                }

                // TakeDamageの引数定義に合わせて呼び出し
                playerHealth.TakeDamage(damage, attacker);
                Debug.Log("プレイヤーに " + damage + " ダメージ！");
            }

            canDamage = false; // 一回の攻撃で多段ヒットしないように判定をオフにする
        }
    }
}