using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    [Header("HP UI")]
    public Slider hpSlider;

    [Header("Armor UI")]
    public Slider armorSlider;
    public Image armorIcon;

    [Header("Weapon UI")]
    public Image weaponIcon;

    // ★追加: ゲーム開始時にアイコンを隠す処理
    void Awake()
    {
        if (weaponIcon != null) weaponIcon.gameObject.SetActive(false);
        if (armorIcon != null) armorIcon.gameObject.SetActive(false);
    }

    public void UpdateHP(float current, float max)
    {
        if (hpSlider != null)
        {
            hpSlider.value = current / max;
        }
    }

    public void UpdateArmor(int current, int max)
    {
        if (armorSlider != null)
        {
            if (current <= 0)
            {
                armorSlider.gameObject.SetActive(false);
            }
            else
            {
                armorSlider.gameObject.SetActive(true);
                armorSlider.value = (float)current / max;
            }
        }

        // ★修正: アーマーがあっても、画像(sprite)がセットされていなければ表示しないようにする
        if (armorIcon != null)
        {
            // 「アーマー値が0より大きい」かつ「画像が空っぽ(null)ではない」ときだけ表示
            bool shouldShow = (current > 0) && (armorIcon.sprite != null);
            armorIcon.gameObject.SetActive(shouldShow);
        }
    }

    public void SetArmorIcon(Sprite icon)
    {
        if (armorIcon != null && icon != null)
        {
            armorIcon.sprite = icon;
        }
    }

    public void UpdateWeapon(string name)
    {
        // 処理なし
    }

    public void UpdateWeaponIcon(Sprite icon)
    {
        if (weaponIcon == null) return;

        if (icon != null)
        {
            weaponIcon.sprite = icon;
            weaponIcon.gameObject.SetActive(true);
        }
        else
        {
            weaponIcon.gameObject.SetActive(false);
        }
    }
}