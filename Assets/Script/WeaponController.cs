using UnityEngine;

public class WeaponController : MonoBehaviour
{
    private Collider weaponCollider;
    private bool isPlayerWeapon = false;

    // ★修正ポイント1: Awakeでは「取得」だけ行う（無効化はしない！）
    void Awake()
    {
        weaponCollider = GetComponent<Collider>();

        // もしコライダーが見つからなければエラーを出す（念のため）
        if (weaponCollider == null)
        {
            // 子オブジェクトにあるかもしれないので探す
            weaponCollider = GetComponentInChildren<Collider>();
        }

        // ▼▼▼ 削除またはコメントアウトしました ▼▼▼
        /* * ここで enabled = false にしてしまうと、地面にある時に
         * レイキャスト（視線）が当たらなくなり、拾えなくなります。
         */
        // if (weaponCollider != null)
        // {
        //    weaponCollider.enabled = false;
        //    weaponCollider.isTrigger = true;
        // }
        // ▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲
    }

    public void Setup(bool isPlayer)
    {
        isPlayerWeapon = isPlayer;

        // 物理挙動を止める
        if (TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        // ★修正ポイント2: 拾われたこのタイミングで初めてコライダーを攻撃用に切り替える
        if (weaponCollider != null)
        {
            weaponCollider.enabled = false; // 攻撃する時までOFF
            weaponCollider.isTrigger = true; // 物理衝突しないようにTriggerにする
        }
    }

    // ★安全対策: nullチェックを追加
    public void EnableHitBox()
    {
        if (weaponCollider != null) weaponCollider.enabled = true;
    }

    public void DisableHitBox()
    {
        if (weaponCollider != null) weaponCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isPlayerWeapon)
        {
            if (other.CompareTag("Enemy"))
            {
                other.SendMessage("TakeDamage", 10, SendMessageOptions.DontRequireReceiver);
            }
        }
        else
        {
            if (other.CompareTag("Player"))
            {
                other.SendMessage("TakeDamage", 10, SendMessageOptions.DontRequireReceiver);
            }
        }
    }
}