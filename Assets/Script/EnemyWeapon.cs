using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
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
    }

    private void OnTriggerEnter(Collider other)
    {
        if (canDamage && other.CompareTag("Player"))
        {
            // 1. 相手（プレイヤー）のHPスクリプトを取得する
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

            // 2. スクリプトがちゃんと付いていたらダメージを与える
            if (playerHealth != null)
            {
                // ★修正箇所★
                // transform.root は使わず、親を遡って EnemyHealth を探す
                // これにより、フォルダ分けされていても確実に敵本体を取得できます
                Transform attacker = transform.root; // 見つからなかった時の保険

                EnemyHealth enemySelf = GetComponentInParent<EnemyHealth>();
                if (enemySelf != null)
                {
                    attacker = enemySelf.transform;
                }

                // 特定した攻撃者(attacker)を渡す
                playerHealth.TakeDamage(damage, attacker);

                Debug.Log("プレイヤーに " + damage + " ダメージ！ 攻撃者: " + attacker.name);
            }

            canDamage = false; // 1回の振りで2回当たらないようにする
        }
    }
}