using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // ★追加: シーン移動に必要

public class HUDManager : MonoBehaviour
{
    [Header("HP UI")]
    public Slider hpSlider;

    [Header("Armor UI")]
    public Slider armorSlider;
    public Image armorIcon;

    [Header("Weapon UI")]
    public Image weaponIcon;

    [Header("Game Over UI")] // ★追加
    public GameObject gameOverUI; // 負けた時に表示するパネル全体（赤背景・文字・ボタンを含む親オブジェクト）

    void Awake()
    {
        if (weaponIcon != null) weaponIcon.gameObject.SetActive(false);
        if (armorIcon != null) armorIcon.gameObject.SetActive(false);

        // ★追加: ゲーム開始時は負け画面を隠しておく
        if (gameOverUI != null) gameOverUI.SetActive(false);
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

        if (armorIcon != null)
        {
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

    // ★追加: 負けた時に呼び出す
    public void ShowGameOver()
    {
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);

            // カーソルを表示してクリックできるようにする
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    // ★追加: タイトルへ戻るボタンから呼び出す
    public void OnTitleButton()
    {
        // "Title" という名前のシーンへ移動します（実際のシーン名に合わせて変更してください）
        SceneManager.LoadScene("TitleScene");
    }
}