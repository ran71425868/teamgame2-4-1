using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    [Header("Pickup Settings")]
    public Camera fpsCamera;          // FPS用カメラ
    public float pickupDistance = 3f; // 拾える距離
    public KeyCode pickupKey = KeyCode.E;

    [Header("Equip Settings")]
    public Transform handPoint;       // 武器を持たせる位置
    private GameObject currentItem;   // 現在装備中のアイテム

    void Update()
    {
        // 拾うキーが押されたら
        if (Input.GetKeyDown(pickupKey))
        {
            TryPickup();
        }
    }

    // アイテムを拾えるかチェック
    void TryPickup()
    {
        // カメラ位置から正面にRayを飛ばす
        Ray ray = new Ray(
            fpsCamera.transform.position,
            fpsCamera.transform.forward
        );

        RaycastHit hit;

        // 指定距離以内に何か当たったら
        if (Physics.Raycast(ray, out hit, pickupDistance))
        {
            // WeaponItem が付いてるか？
            WeaponItem item = hit.collider.GetComponent<WeaponItem>();

            if (item != null)
            {
                // アイテム側のPickup処理を呼ぶ
                item.Pickup(this);
            }
        }
    }

    // WeaponItem から呼ばれる装備処理
    public void EquipItem(GameObject equipPrefab)
    {
        if (equipPrefab == null) return;

        // すでに持ってたら破棄
        if (currentItem != null)
        {
            Destroy(currentItem);
        }

        // 手元に装備
        currentItem = Instantiate(equipPrefab, handPoint);
        currentItem.transform.localPosition = Vector3.zero;
        currentItem.transform.localRotation = Quaternion.identity;

    }
    void LateUpdate()
    {
        if (currentItem != null)
        {
            currentItem.transform.localPosition = Vector3.zero;
            currentItem.transform.localRotation = Quaternion.identity;
        }
    }

}
