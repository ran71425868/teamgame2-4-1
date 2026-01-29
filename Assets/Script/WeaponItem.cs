using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponItem : MonoBehaviour
{
    public GameObject equipPrefab;
    public GameObject pickupUI;

    void Start()
    {
        // 最初は必ず非表示にしておく
        if (pickupUI != null) pickupUI.SetActive(false);
    }

    // プレイヤーに見られた時
    public void OnLookEnter()
    {
        if (pickupUI != null) pickupUI.SetActive(true);
    }

    // 目を逸らされた時
    public void OnLookExit()
    {
        if (pickupUI != null) pickupUI.SetActive(false);
    }

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

