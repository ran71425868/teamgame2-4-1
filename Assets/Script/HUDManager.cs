using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDManager : MonoBehaviour
{
    [Header("HP UI")]
    public Slider hpSlider;

    [Header("Armor UI")]
    public Slider armorSlider; // 追加: アーマーバー

    [Header("Weapon UI")]
    public TextMeshProUGUI weaponText;

    public void UpdateHP(float current, float max)
    {
        if (hpSlider != null)
        {
            hpSlider.value = current / max;
        }
    }

    // --- 追加: アーマー更新処理 ---
    public void UpdateArmor(int current, int max)
    {
        if (armorSlider == null) return;

        // アーマーが0以下ならバーを隠す
        if (current <= 0)
        {
            armorSlider.gameObject.SetActive(false);
        }
        else
        {
            // アーマーがあるならバーを表示して値を更新
            armorSlider.gameObject.SetActive(true);
            armorSlider.value = (float)current / max;
        }
    }
    // ---------------------------

    public void UpdateWeapon(string name)
    {
        if (weaponText != null)
        {
            string cleanName = name.Replace("(Clone)", "");
            weaponText.text = cleanName;
        }
    }
}