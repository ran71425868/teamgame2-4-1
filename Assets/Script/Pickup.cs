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

    // HUDManagerへの参照
    private HUDManager hudManager;

    private WeaponItem currentTargetItem;
    private Armor currentTargetArmor;

    // --- 追加: 音声設定 ---
    [Header("Sound Settings")]
    public AudioClip weaponPickupSound; // 武器を拾った時の音
    public AudioClip armorPickupSound;  // アーマーを拾った時の音
    public AudioSource audioSource;     // 音を鳴らすスピーカー
    // ---------------------

    void Start()
    {
        Playerarmor = GetComponent<Armor>();
        hudManager = FindObjectOfType<HUDManager>();

        // --- 追加: AudioSourceが設定されていなければ自動取得 ---
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    void Update()
    {
        CheckObjectInSight();
    }

    void CheckObjectInSight()
    {
        Ray ray = new Ray(fpsCamera.transform.position, fpsCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, pickupDistance))
        {
            // 武器のチェック
            WeaponItem weapon = hit.collider.GetComponent<WeaponItem>();
            if (weapon != null)
            {
                if (currentTargetItem != weapon)
                {
                    ClearCurrentTarget();
                    currentTargetItem = weapon;
                    currentTargetItem.OnLookEnter();
                }
                HandlePickupInput(ray);
                return;
            }

            // アーマーのチェック
            Armor armor = hit.collider.GetComponentInParent<Armor>();
            if (armor != null && armor.isPickup)
            {
                if (currentTargetArmor != armor)
                {
                    ClearCurrentTarget();
                    currentTargetArmor = armor;
                    currentTargetArmor.OnLookEnter();
                }
                HandlePickupInput(ray);
                return;
            }
        }

        ClearCurrentTarget();
    }

    void ClearCurrentTarget()
    {
        if (currentTargetItem != null) { currentTargetItem.OnLookExit(); currentTargetItem = null; }
        if (currentTargetArmor != null) { currentTargetArmor.OnLookExit(); currentTargetArmor = null; }
    }

    void HandlePickupInput(Ray ray)
    {
        if (Input.GetKeyDown(pickupKey))
        {
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, pickupDistance))
            {
                Armor armor = hit.collider.GetComponentInParent<Armor>();
                if (armor != null && armor.isPickup)
                {
                    if (Playerarmor != null)
                    {
                        // --- 追加: アーマー取得音を再生 ---
                        if (audioSource != null && armorPickupSound != null)
                        {
                            audioSource.PlayOneShot(armorPickupSound);
                        }
                        // ------------------------------

                        Playerarmor.EquipArmor(armor.armorValue);
                        Destroy(armor.gameObject);
                        currentTargetArmor = null;
                    }
                    return;
                }

                WeaponItem weapon = hit.collider.GetComponent<WeaponItem>();
                if (weapon != null)
                {
                    // --- 追加: 武器取得音を再生 ---
                    if (audioSource != null && weaponPickupSound != null)
                    {
                        audioSource.PlayOneShot(weaponPickupSound);
                    }
                    // ---------------------------

                    weapon.Pickup(this);
                    currentTargetItem = null;
                    return;
                }
            }
        }
    }

    // ドロップ処理を追加した装備関数
    public void EquipItem(GameObject equipPrefab)
    {
        if (equipPrefab == null) return;

        // --- ドロップ処理 ---
        if (currentItem != null)
        {
            // 今持っている武器の WeaponData を取得
            WeaponData oldData = currentItem.GetComponent<WeaponData>();

            // ドロップ用プレハブが設定されていれば生成
            if (oldData != null && oldData.dropPrefab != null)
            {
                // プレイヤーの少し前・少し上に生成（足元に埋まらないように）
                Vector3 dropPos = transform.position + (transform.forward * 0.5f) + (Vector3.up * 0.5f);
                Instantiate(oldData.dropPrefab, dropPos, Quaternion.identity);
            }

            // 古い武器を削除
            Destroy(currentItem);
        }
        // ------------------

        // 新しい武器を生成
        currentItem = Instantiate(equipPrefab, handPoint);
        currentItem.transform.localPosition = Vector3.zero;
        currentItem.transform.localRotation = Quaternion.identity;

        // アイコン更新処理
        if (hudManager != null)
        {
            WeaponData data = currentItem.GetComponent<WeaponData>();
            if (data != null) hudManager.UpdateWeaponIcon(data.icon);
            else hudManager.UpdateWeaponIcon(null);
        }
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