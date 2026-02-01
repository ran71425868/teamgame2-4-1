using UnityEngine;

public class WeaponController : MonoBehaviour
{
    // ★SimpleWeaponの数値を参照するための変数
    private SimpleWeapon simpleWeapon;

    // もしSimpleWeaponがない場合の予備ダメージ
    public int defaultDamage = 10;

    private Collider weaponCollider;
    private bool isPlayerWeapon = false;

    void Awake()
    {
        weaponCollider = GetComponent<Collider>();
        if (weaponCollider == null)
        {
            weaponCollider = GetComponentInChildren<Collider>();
        }

        // ★同じオブジェクトにある SimpleWeapon スクリプトを取得する
        simpleWeapon = GetComponent<SimpleWeapon>();
        if (simpleWeapon == null)
        {
            // なければ子オブジェクトも探してみる
            simpleWeapon = GetComponentInChildren<SimpleWeapon>();
        }
    }

    public void Setup(bool isPlayer)
    {
        isPlayerWeapon = isPlayer;

        if (TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        if (weaponCollider != null)
        {
            weaponCollider.enabled = false;
            weaponCollider.isTrigger = true;
        }
    }

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
            if (other.CompareTag("Enemy") || other.CompareTag("enemy_mob"))
            {
                // ★攻撃力を決定するロジック
                int finalDamage = defaultDamage;

                // SimpleWeaponがついているなら、そこのdamage数値を使う
                if (simpleWeapon != null)
                {
                    finalDamage = simpleWeapon.damage;
                }

                // 決定したダメージを送る
                other.SendMessage("TakeDamage", finalDamage, SendMessageOptions.DontRequireReceiver);

                // 確認用ログ
                Debug.Log(other.name + " に " + finalDamage + " ダメージ (SimpleWeapon参照)");
            }
        }
        else
        {
            if (other.CompareTag("Player"))
            {
                int finalDamage = defaultDamage;
                if (simpleWeapon != null) finalDamage = simpleWeapon.damage;

                other.SendMessage("TakeDamage", finalDamage, SendMessageOptions.DontRequireReceiver);
            }
        }
    }
}