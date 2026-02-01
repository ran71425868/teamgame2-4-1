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
    public Transform handSocket;   // ★追加: 右手ボーンの下に作った武器ソケット

    // ★変更: 2つの武器を管理するように変更
    private GameObject currentCameraItem; // カメラ用
    private GameObject currentHandItem;   // 手用

    Armor Playerarmor;

    // HUDManagerへの参照
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
                        if (audioSource != null && armorPickupSound != null)
                        {
                            audioSource.PlayOneShot(armorPickupSound);
                        }

                        Playerarmor.EquipArmor(armor.armorValue);
                        Destroy(armor.gameObject);
                        currentTargetArmor = null;
                    }
                    return;
                }

                WeaponItem weapon = hit.collider.GetComponent<WeaponItem>();
                if (weapon != null)
                {
                    if (audioSource != null && weaponPickupSound != null)
                    {
                        audioSource.PlayOneShot(weaponPickupSound);
                    }

                    weapon.Pickup(this);
                    currentTargetItem = null;
                    return;
                }
            }
        }
    }

    // ★修正: 2つの武器を生成してPlayerに登録する処理
    public void EquipItem(GameObject equipPrefab)
    {
        if (equipPrefab == null)
        {
            Debug.LogError("エラー: Equip Prefab が設定されていません！");
            return;
        }

        // --- 古い武器のドロップと削除 ---
        if (currentCameraItem != null)
        {
            // ドロップ品生成は片方（カメラ用）から情報を取ればOK
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
        // -----------------------------

        // 1. カメラ用の武器を生成（普段見える用）
        currentCameraItem = Instantiate(equipPrefab, handPoint);
        currentCameraItem.transform.localPosition = Vector3.zero;
        currentCameraItem.transform.localRotation = Quaternion.identity;

        // カメラ用の武器は、スクリプトや当たり判定を無効化しておく（手元のが判定を持つため）
        WeaponController camCtrl = currentCameraItem.GetComponent<WeaponController>();
        if (camCtrl != null) camCtrl.enabled = false;
        Collider col = currentCameraItem.GetComponent<Collider>();
        if (col != null) col.enabled = false;

        // 2. 手用の武器を生成（攻撃用・普段は見えない）
        if (handSocket != null)
        {
            currentHandItem = Instantiate(equipPrefab, handSocket);
            currentHandItem.transform.localPosition = Vector3.zero;
            currentHandItem.transform.localRotation = Quaternion.identity;

            // 最初は非表示
            currentHandItem.SetActive(false);
        }
        else
        {
            Debug.LogError("Pickupスクリプトの 'Hand Socket' が設定されていません！");
        }

        // 3. Playerスクリプトに登録
        // 攻撃判定を行うのは「手の武器」の方のコントローラー
        if (currentHandItem != null)
        {
            WeaponController handCtrl = currentHandItem.GetComponent<WeaponController>();
            if (handCtrl != null)
            {
                handCtrl.Setup(true);

                Player player = GetComponentInParent<Player>();
                if (player != null)
                {
                    // 攻撃用スクリプトとして「手の武器」を登録
                    player.weaponScript = handCtrl;

                    // 表示切り替え用に2つのオブジェクトを登録
                    player.cameraWeaponObj = currentCameraItem;
                    player.handWeaponObj = currentHandItem;

                    Debug.Log("成功: Playerに武器（カメラ用・手用）を登録しました！");
                }
            }
        }

        // アイコン更新
        if (hudManager != null)
        {
            WeaponData data = currentCameraItem.GetComponent<WeaponData>();
            if (data != null) hudManager.UpdateWeaponIcon(data.icon);
            else hudManager.UpdateWeaponIcon(null);
        }
    }

    void LateUpdate()
    {
        // カメラ用の武器だけ位置を強制リセット（手の武器はボーンについていくので不要）
        if (currentCameraItem != null)
        {
            currentCameraItem.transform.localPosition = Vector3.zero;
            currentCameraItem.transform.localRotation = Quaternion.identity;
        }
    }
}