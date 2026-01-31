using UnityEngine;
using UnityEngine.UI;
// using TMPro; // テキストを使わないならこの行は不要になります

public class HUDManager : MonoBehaviour
{
    [Header("HP UI")]
    public Slider hpSlider;

    [Header("Armor UI")]
    public Slider armorSlider;

    // public TextMeshProUGUI weaponText; // ← この変数を削除しました

    [Header("Weapon UI")]
    public Image weaponIcon;           // アイコン表示用のImage

    public void UpdateHP(float current, float max)
    {
        if (hpSlider != null)
        {
            hpSlider.value = current / max;
        }
    }

    public void UpdateArmor(int current, int max)
    {
        if (armorSlider == null) return;

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

    // ★重要: 関数自体を消すと、これを呼んでいる場所でエラーになるので、
    // 関数は残しておいて「中身だけ」空にします。
    public void UpdateWeapon(string name)
    {
        // テキスト表示機能は削除したので何もしない
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