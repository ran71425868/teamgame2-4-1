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
    Armor  Playerarmor;
    // --- 追加: 今見ているアイテムを覚えておく変数 ---
    private WeaponItem currentTargetItem;
     void Start()
    {
        Playerarmor = GetComponent<Armor>();
    }
    void Update()
    {
        // 常に視線の先をチェックする
        CheckObjectInSight();

        // 拾うキーが押されたら、かつ「アイテムを見ているなら」
        if (Input.GetKeyDown(pickupKey))
        {
            if (currentTargetItem != null)
            {
                currentTargetItem.Pickup(this);
                currentTargetItem = null; // 拾ったらターゲットを空にする
            }
        }
    }
   

    // --- 変更: Rayを常に飛ばしてUIを制御する ---
    void CheckObjectInSight()
    {
        Ray ray = new Ray(fpsCamera.transform.position, fpsCamera.transform.forward);
        RaycastHit hit;

        // 何かに当たった？
        if (Physics.Raycast(ray, out hit, pickupDistance))
        {
            // それは WeaponItem か？
            WeaponItem item = hit.collider.GetComponent<WeaponItem>();

            if (item != null)
            {
                // 新しいアイテムを見た！
                if (currentTargetItem != item)
                {
                    if (currentTargetItem != null) currentTargetItem.OnLookExit(); // 前のを消す
                    currentTargetItem = item;
                    currentTargetItem.OnLookEnter(); // 今のを出す
                }
                return; // ここで終わる
            }
        }

        // 何も見ていない、またはアイテム以外を見ている場合
        if (currentTargetItem != null)
        {
            currentTargetItem.OnLookExit(); // UIを消す
            currentTargetItem = null;

        }

        if (Physics.Raycast(ray, out  hit, pickupDistance))
        {
            // ======================
            // 武器チェック
            // ======================
            WeaponItem weapon = hit.collider.GetComponent<WeaponItem>();
            if (weapon != null)
            {
                weapon.Pickup(this);
                return;
            }

            // ======================
            // アーマーチェック
            // ======================
            Armor armorPickup =
                hit.collider.GetComponentInParent<Armor>();

            if (armorPickup != null && armorPickup.isPickup)
            {
                if (Playerarmor != null && Playerarmor.isPlayer)
                {
                    Playerarmor.EquipArmor(armorPickup.armorValue);
                    Destroy(armorPickup.gameObject);
                }
            }
        }
    }

    // WeaponItem から呼ばれる装備処理（ここはそのまま）
    public void EquipItem(GameObject equipPrefab)
    {
        if (equipPrefab == null) return;

        if (currentItem != null)
        {
            Destroy(currentItem);
        }

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