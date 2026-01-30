using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    [Header("Pickup Settings")]
    public Camera fpsCamera;
    public float pickupDistance = 3f;
    public KeyCode pickupKey = KeyCode.E;

    [Header("Equip Settings")]
    public Transform handPoint;
    private GameObject currentItem;
    Armor Playerarmor;

    private WeaponItem currentTargetItem;
    // --- 追加: 今見ているアーマーを覚えておく変数 ---
    private Armor currentTargetArmor;

    void Start()
    {
        Playerarmor = GetComponent<Armor>();
    }

    void Update()
    {
        // 常に視線の先をチェック（UIの表示・非表示もここ）
        CheckObjectInSight();
    }

    void CheckObjectInSight()
    {
        Ray ray = new Ray(fpsCamera.transform.position, fpsCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, pickupDistance))
        {
            // --- 武器のチェック ---
            WeaponItem weapon = hit.collider.GetComponent<WeaponItem>();
            if (weapon != null)
            {
                if (currentTargetItem != weapon)
                {
                    ClearCurrentTarget(); // 他のターゲットを一旦クリア
                    currentTargetItem = weapon;
                    currentTargetItem.OnLookEnter();
                }
                HandlePickupInput(ray); // 視線が合っている間、入力を受け付ける
                return;
            }

            // --- アーマーのチェック ---
            Armor armor = hit.collider.GetComponentInParent<Armor>();
            if (armor != null && armor.isPickup)
            {
                if (currentTargetArmor != armor)
                {
                    ClearCurrentTarget(); // 他のターゲットを一旦クリア
                    currentTargetArmor = armor;
                    currentTargetArmor.OnLookEnter();
                }
                HandlePickupInput(ray); // 視線が合っている間、入力を受け付ける
                return;
            }
        }

        // 何も見ていない時はUIを消す
        ClearCurrentTarget();
    }

    // 全てのターゲットのUIを消して変数をリセットする関数
    void ClearCurrentTarget()
    {
        if (currentTargetItem != null) { currentTargetItem.OnLookExit(); currentTargetItem = null; }
        if (currentTargetArmor != null) { currentTargetArmor.OnLookExit(); currentTargetArmor = null; }
    }

    // --- 追加: 実際の「拾う」入力判定 ---
    // Pickup.cs の HandlePickupInput 内
    void HandlePickupInput(Ray ray)
    {
        if (Input.GetKeyDown(pickupKey))
        {
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, pickupDistance))
            {
                // 1. まずアーマーをチェック
                Armor armor = hit.collider.GetComponentInParent<Armor>();
                if (armor != null && armor.isPickup)
                {
                    if (Playerarmor != null)
                    {
                        Playerarmor.EquipArmor(armor.armorValue);
                        Destroy(armor.gameObject);
                        currentTargetArmor = null;
                    }
                    return; // アーマーを拾ったらここで終了
                }

                // 2. アーマーでなければ武器をチェック
                WeaponItem weapon = hit.collider.GetComponent<WeaponItem>();
                if (weapon != null)
                {
                    weapon.Pickup(this);
                    currentTargetItem = null;
                    return; // 武器を拾ったらここで終了
                }
            }
        }
    }

    public void EquipItem(GameObject equipPrefab)
    {
        if (equipPrefab == null) return;
        if (currentItem != null) Destroy(currentItem);
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