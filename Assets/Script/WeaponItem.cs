using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponItem : MonoBehaviour
{
    public GameObject equipPrefab;
    public GameObject pickupUI;

    // --- 追加1: UIの高さ設定とカメラ用変数 ---
    [Header("UI Settings")]
    public float uiHeightOffset = 1.0f; // インスペクターで高さを調整できます
    private Camera mainCamera;
    // -------------------------------------

    void Start()
    {
        // --- 追加2: カメラを取得 ---
        mainCamera = Camera.main;
        // ------------------------

        // 最初は必ず非表示にしておく
        if (pickupUI != null) pickupUI.SetActive(false);
    }

    // --- 追加3: UIの位置と向きを毎フレーム更新 ---
    void Update()
    {
        // UIが表示されている時だけ位置を調整する
        if (pickupUI != null && pickupUI.activeSelf)
        {
            // 1. 位置を「武器の座標 + 高さ」に固定
            pickupUI.transform.position = transform.position + (Vector3.up * uiHeightOffset);

            // 2. UIが常にカメラの方を向くように回転（ビルボード処理）
            if (mainCamera != null)
            {
                pickupUI.transform.LookAt(
                    pickupUI.transform.position + mainCamera.transform.rotation * Vector3.forward,
                    mainCamera.transform.rotation * Vector3.up
                );
            }
        }
    }
    // ----------------------------------------

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
        // --- 追加4: 拾われたら念のためUIを消す ---
        if (pickupUI != null) pickupUI.SetActive(false);
        // --------------------------------------

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