using UnityEngine;

// プレイヤー用アーマー & アーマーPickup 兼用
public class Armor : MonoBehaviour
{
    [Header("Role")]
    public bool isPlayer = false;   // プレイヤー用か？
    public bool isPickup = false;   // 拾われるアーマーか？

    [Header("Pickup Settings")]
    public float pickupRange = 3.0f; // 拾える距離
    public Sprite armorSprite;
    [Header("Armor Value")]
    public int maxArmor = 0;        // 最大アーマー
    public int currentArmor = 0;    // 現在のアーマー
    public int armorValue = 50;     // Pickup時に渡す値
    public HUDManager hudManager;

    private GameObject playerObj;
    public GameObject pickupUI;

    void Awake()
    {
        // 最初はUIを隠しておく
        if (isPickup && pickupUI != null) pickupUI.SetActive(false);
    }

    // 視線が入ったとき
    public void OnLookEnter()
    {
        if (isPickup && pickupUI != null) pickupUI.SetActive(true);
    }

    // 視線が外れたとき
    public void OnLookExit()
    {
        if (isPickup && pickupUI != null) pickupUI.SetActive(false);
    }

    void Start()
    {
        // プレイヤーを探しておく（Pickup用の場合に必要）
        if (isPickup)
        {
            playerObj = GameObject.FindGameObjectWithTag("Player");
        }

        // ゲーム開始時にUIをリセット（プレイヤーの場合のみ）
        if (isPlayer && hudManager != null)
        {
            if (armorSprite != null) hudManager.SetArmorIcon(armorSprite);
            hudManager.UpdateArmor(currentArmor, maxArmor);
        }
    }

    private void TryPickup()
    {
        // プレイヤー側のArmorコンポーネントを取得
        Armor playerArmor = playerObj.GetComponent<Armor>();

        if (playerArmor != null && playerArmor.isPlayer)
        {
            // プレイヤーに装備させる
            playerArmor.EquipArmor(armorValue, armorSprite);

            // フィールド上のアーマーを消す
            Destroy(gameObject);
        }
    }

    // =========================
    // Player側処理
    // =========================

    // アーマーを装備（着替え）
    public void EquipArmor(int value, Sprite icon = null)
    {
        maxArmor = value;
        currentArmor = value;

        Debug.Log("Armor Equipped : " + value);
        if (hudManager != null)
        {
            // ★追加: アイコンを更新してからスライダー等を更新
            if (icon != null) hudManager.SetArmorIcon(icon);
            hudManager.UpdateArmor(currentArmor, maxArmor);
        }
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
        if (hudManager != null) hudManager.UpdateArmor(currentArmor, maxArmor);
        return damage - absorbed;
    }
}