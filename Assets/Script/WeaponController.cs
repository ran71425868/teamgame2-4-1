using UnityEngine;

public class WeaponController : MonoBehaviour
{
    private Collider weaponCollider;

    // この武器の持ち主はプレイヤーか？ (true=プレイヤー, false=敵)
    private bool isPlayerWeapon = false;

    void Start()
    {
        weaponCollider = GetComponent<Collider>();
        weaponCollider.enabled = false;
        weaponCollider.isTrigger = true;
    }

    // ★重要: 拾われた瞬間に、持ち主の情報をセットするメソッド
    public void Setup(bool isPlayer)
    {
        isPlayerWeapon = isPlayer;
    }

    // アニメーションイベント用
    public void EnableHitBox() => weaponCollider.enabled = true;
    public void DisableHitBox() => weaponCollider.enabled = false;

    private void OnTriggerEnter(Collider other)
    {
        // 持ち主が「プレイヤー」の場合
        if (isPlayerWeapon)
        {
            // 敵(Enemy)にだけ当たる
            if (other.CompareTag("Enemy"))
            {
                // ダメージ処理（敵側のTakeDamageを呼ぶ）
                other.SendMessage("TakeDamage", 10, SendMessageOptions.DontRequireReceiver);
                Debug.Log("プレイヤーの攻撃が敵にヒット！");
            }
        }
        // 持ち主が「敵」の場合
        else
        {
            // プレイヤー(Player)にだけ当たる
            if (other.CompareTag("Player"))
            {
                // ダメージ処理（Player側のTakeDamageを呼ぶ）
                other.SendMessage("TakeDamage", 10, SendMessageOptions.DontRequireReceiver);
                Debug.Log("敵の攻撃がプレイヤーにヒット！");
            }
        }
    }
}