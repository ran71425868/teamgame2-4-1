using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponItem : MonoBehaviour
{
    public GameObject equipPrefab;

  
    public void Pickup(Pickup player)
    {
        // 物理停止
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        // 移動系スクリプトを無効化
        MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();
        foreach (var s in scripts)
        {
            if (s != this)
                s.enabled = false;
        }

        player.EquipItem(equipPrefab);
        Destroy(gameObject);
    }

}

