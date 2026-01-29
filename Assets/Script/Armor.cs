using UnityEngine;

// プレイヤー用アーマー & アーマーPickup 兼用
public class Armor : MonoBehaviour
{
    [Header("Role")]
    public bool isPlayer = false;   // プレイヤー用か？
    public bool isPickup = false;   // 拾われるアーマーか？

    [Header("Armor Value")]
    public int maxArmor = 0;        // 最大アーマー
    public int currentArmor = 0;    // 現在のアーマー
    public int armorValue = 50;     // Pickup時に渡す値

    // =========================
    // Player側処理
    // =========================

    // アーマーを装備（着替え）
    public void EquipArmor(int value)
    {
        maxArmor = value;
        currentArmor = value;

        Debug.Log("Armor Equipped : " + value);
    }

    // ダメージを吸収して、残りダメージを返す
    public int AbsorbDamage(int damage)
    {
        if (currentArmor <= 0)
            return damage;

        int absorbed = Mathf.Min(currentArmor, damage);
        currentArmor -= absorbed;

        Debug.Log("Armor Absorbed : " + absorbed +
                  " / Remaining Armor : " + currentArmor);

        return damage - absorbed;
    }

    // =========================
    // Pickup側処理
    // =========================

    void OnTriggerEnter(Collider other)
    {
        if (!isPickup) return;

        if (other.CompareTag("Player"))
        {
            Armor playerArmor =
                other.GetComponent<Armor>();

            if (playerArmor != null && playerArmor.isPlayer)
            {
                // 今のアーマーを上書き装備
                playerArmor.EquipArmor(armorValue);
                Destroy(gameObject);
            }
        }
    }
}
