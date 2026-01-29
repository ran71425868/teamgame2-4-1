using UnityEngine;

// FPS用：武器・アーマーを拾って装備／管理するクラス
public class Pickup : MonoBehaviour
{
    // =========================
    // 拾う設定
    // =========================

    [Header("Pickup Settings")]
    public Camera fpsCamera;
    public float pickupDistance = 3f;
    public KeyCode pickupKey = KeyCode.E;

    // =========================
    // 武器装備
    // =========================

    [Header("Weapon Equip")]
    public Transform handPoint;
    private GameObject currentWeapon;

   

    void Update()
    {
        if (Input.GetKeyDown(pickupKey))
        {
            TryPickup();
        }
    }

    // =========================
    // 拾う判定
    // =========================
    void TryPickup()
    {
        Ray ray = new Ray(
            fpsCamera.transform.position,
            fpsCamera.transform.forward
        );

        RaycastHit hit;

        if (!Physics.Raycast(ray, out hit, pickupDistance))
            return;

        // ----------------------
        // 武器
        // ----------------------
        WeaponItem weapon = hit.collider.GetComponent<WeaponItem>();
        if (weapon != null)
        {
            weapon.Pickup(this);
            return;
        }

      
    }

    // =========================
    // 武器装備
    // =========================
    public void EquipItem(GameObject prefab)
    {
        if (prefab == null) return;

        if (currentWeapon != null)
            Destroy(currentWeapon);

        currentWeapon = Instantiate(prefab, handPoint);
        currentWeapon.transform.localPosition = Vector3.zero;
        currentWeapon.transform.localRotation = Quaternion.identity;
    }


    // =========================
    // 武器ズレ防止
    // =========================
    void LateUpdate()
    {
        if (currentWeapon != null)
        {
            currentWeapon.transform.localPosition = Vector3.zero;
            currentWeapon.transform.localRotation = Quaternion.identity;
        }
    }
}
