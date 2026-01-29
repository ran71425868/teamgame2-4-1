using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshProを使う場合

public class HUDManager : MonoBehaviour
{
    
    public Slider hpSlider;
    public TextMeshProUGUI weaponText; // 普通のTextを使う場合は "Text" に変更

    // HP更新処理（PlayerHealthから呼ぶ）
    public void UpdateHP(float current, float max)
    {
        if (hpSlider != null)
        {
            hpSlider.value = current / max; // 0.0〜1.0の割合にする
        }
    }

    // 武器名更新処理（Pickupから呼ぶ）
    public void UpdateWeapon(string name)
    {
        if (weaponText != null)
        {
            // "(Clone)" という文字が邪魔なら消す
            string cleanName = name.Replace("(Clone)", "");
            weaponText.text = cleanName;
        }
    }
}