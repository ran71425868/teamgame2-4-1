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
    public Transform handPoint;    // カメラの下にある武器ホルダー
    public Transform handSocket;   // 右手ボーンの下に作った武器ソケット

    // 2つの武器を管理
    private GameObject currentCameraItem; // カメラ用
    private GameObject currentHandItem;   // 手用

    Armor Playerarmor;
    private HUDManager hudManager;
    private WeaponItem currentTargetItem;
    private Armor currentTargetArmor;

    [Header("Sound Settings")]
    public AudioClip weaponPickupSound;
    public AudioClip armorPickupSound;
    public AudioSource audioSource;

    void Start()
    {
        Playerarmor = GetComponent<Armor>();
        hudManager = FindObjectOfType<HUDManager>();
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        CheckObjectInSight();
    }

    // (視線判定のコードは変更なしのため省略します...元のままにしてください)
    void CheckObjectInSight()
    {
        Ray ray = new Ray(fpsCamera.transform.position, fpsCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, pickupDistance))
        {
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
            // Armor処理...
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
                // Armor処理...
                Armor armor = hit.collider.GetComponentInParent<Armor>();
                if (armor != null && armor.isPickup)
                {
                    if (Playerarmor != null)
                    {
                        if (audioSource != null && armorPickupSound != null) audioSource.PlayOneShot(armorPickupSound);
                        Playerarmor.EquipArmor(armor.armorValue);
                        Destroy(armor.gameObject);
                        currentTargetArmor = null;
                    }
                    return;
                }

                // 武器処理
                WeaponItem weapon = hit.collider.GetComponent<WeaponItem>();
                if (weapon != null)
                {
                    if (audioSource != null && weaponPickupSound != null) audioSource.PlayOneShot(weaponPickupSound);
                    weapon.Pickup(this);
                    currentTargetItem = null;
                    return;
                }
            }
        }
    }

    // ★★★ ここを修正！確実にPlayerへ情報を渡す ★★★
    public void EquipItem(GameObject equipPrefab)
    {
        if (equipPrefab == null) return;

        // Playerを探しておく
        Player player = GetComponentInParent<Player>();
        if (player == null)
        {
            Debug.LogError("エラー: Playerスクリプトが見つかりません！");
            return;
        }

        // --- 1. 古い武器の削除とリセット ---
        // 先にPlayer側の参照を空にしておく（Missing回避）
        player.weaponScript = null;
        player.currentCameraWeapon = null;
        player.currentHandWeapon = null;

        if (currentCameraItem != null)
        {
            // ドロップ品生成
            WeaponData oldData = currentCameraItem.GetComponent<WeaponData>();
            if (oldData != null && oldData.dropPrefab != null)
            {
                Vector3 dropPos = transform.position + (transform.forward * 0.5f) + (Vector3.up * 0.5f);
                Instantiate(oldData.dropPrefab, dropPos, Quaternion.identity);
            }
            Destroy(currentCameraItem);
        }
        if (currentHandItem != null)
        {
            Destroy(currentHandItem);
        }

        // --- 2. カメラ用武器の生成 ---
        currentCameraItem = Instantiate(equipPrefab, handPoint);
        currentCameraItem.transform.localPosition = Vector3.zero;
        currentCameraItem.transform.localRotation = Quaternion.identity;

        // カメラ用はスクリプト無効化
        var camCtrl = currentCameraItem.GetComponent<WeaponController>();
        if (camCtrl) camCtrl.enabled = false;
        var camCol = currentCameraItem.GetComponent<Collider>();
        if (camCol) camCol.enabled = false;

        // --- 3. 手用武器の生成 ---
        if (handSocket != null)
        {
            currentHandItem = Instantiate(equipPrefab, handSocket);
            currentHandItem.transform.localPosition = Vector3.zero;
            currentHandItem.transform.localRotation = Quaternion.identity;
            currentHandItem.SetActive(false); // 即座に隠す
        }
        else
        {
            Debug.LogError("エラー: Hand Socket が設定されていません！");
            return;
        }

        // --- 4. 新しい武器情報をPlayerに登録 ---
        // WeaponControllerを探す（子オブジェクトも含めて探すように強化）
        WeaponController handCtrl = currentHandItem.GetComponent<WeaponController>();
        if (handCtrl == null)
        {
            handCtrl = currentHandItem.GetComponentInChildren<WeaponController>();
        }

        if (handCtrl != null)
        {
            // セットアップ
            handCtrl.Setup(true);

            // ★ここで確実にPlayerに代入！
            player.weaponScript = handCtrl;
            player.currentCameraWeapon = currentCameraItem;
            player.currentHandWeapon = currentHandItem;

            Debug.Log($"武器持ち替え完了: {handCtrl.name} を登録しました");
        }
        else
        {
            Debug.LogError($"エラー: 武器プレハブ {equipPrefab.name} に WeaponController がついていません！");
        }

        // アイコン更新
        if (hudManager != null)
        {
            WeaponData data = currentCameraItem.GetComponent<WeaponData>();
            if (data != null) hudManager.UpdateWeaponIcon(data.icon);
        }
    }

    void LateUpdate()
    {
        if (currentCameraItem != null)
        {
            currentCameraItem.transform.localPosition = Vector3.zero;
            currentCameraItem.transform.localRotation = Quaternion.identity;
        }
    }
}