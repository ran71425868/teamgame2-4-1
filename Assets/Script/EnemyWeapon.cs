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
                // ★ここが修正点★
                // 第2引数に「transform.root」を渡します。
                // これにより、剣（武器）ではなく、その親の親...である「敵本体」の情報をプレイヤーに伝えます。
                playerHealth.TakeDamage(damage, transform.root);

                Debug.Log("プレイヤーに " + damage + " ダメージ！");
            }

            canDamage = false; // 1回の振りで2回当たらないようにする
        }
    }
}