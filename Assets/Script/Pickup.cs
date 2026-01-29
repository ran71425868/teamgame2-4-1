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

    Armor playerArmor;

    void Awake()
    {
        playerArmor = GetComponent<Armor>();
    }

    void Update()
    {
        if (Input.GetKeyDown(pickupKey))
        {
            TryPickup();
        }
    }

    void TryPickup()
    {
        Ray ray = new Ray(
            fpsCamera.transform.position,
            fpsCamera.transform.forward
        );

        if (Physics.Raycast(ray, out RaycastHit hit, pickupDistance))
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
                if (playerArmor != null && playerArmor.isPlayer)
                {
                    playerArmor.EquipArmor(armorPickup.armorValue);
                    Destroy(armorPickup.gameObject);
                }
            }
        }
    }

    // WeaponItem から呼ばれる
    public void EquipItem(GameObject equipPrefab)
    {
        if (equipPrefab == null) return;

        if (currentItem != null)
            Destroy(currentItem);

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
