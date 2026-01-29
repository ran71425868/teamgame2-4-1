using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeapon : MonoBehaviour
{
    public int damage = 10;
    private bool canDamage = false;

    // アニメーションから呼び出して判定をON/OFFする
    public void SetAttackActive(bool active)
    {
        canDamage = active;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (canDamage && other.CompareTag("Player"))
        {
            // プレイヤーにダメージを与える処理（PlayerHealthスクリプトなどが必要）
            Debug.Log("プレイヤーに " + damage + " ダメージ！");
            canDamage = false; // 1回の振りで2回当たらないようにする
        }
    }
}
