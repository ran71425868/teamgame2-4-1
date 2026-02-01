using UnityEngine;

public class WeaponController : MonoBehaviour
{
    // ★追加: ダメージ数字のプレハブを入れる箱
    [Header("エフェクト設定")]
    public GameObject damagePopupPrefab;

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

                // ★追加: ダメージ数字を表示する処理
                if (damagePopupPrefab != null)
                {
                    // 敵の少し上あたりに出現させる
                    // Hitした場所(other.ClosestPoint)に出すとより正確ですが、簡単のため敵の位置+少し上にします
                    Vector3 spawnPosition = other.transform.position + Vector3.up * 1.5f;

                    // 少し位置をランダムにずらす（重ならないように）
                    spawnPosition += new Vector3(Random.Range(-0.2f, 0.2f), 0, Random.Range(-0.2f, 0.2f));

                    // 生成！
                    GameObject popup = Instantiate(damagePopupPrefab, spawnPosition, Quaternion.identity);

                    // 数字をセットする
                    popup.GetComponent<DamagePopup>().Setup(finalDamage);
                }
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