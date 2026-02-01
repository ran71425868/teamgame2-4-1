using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HUDManager : MonoBehaviour
{
    [Header("HP UI")]
    public Slider hpSlider;

    [Header("Armor UI")]
    public Slider armorSlider;
    public Image armorIcon;

    [Header("Weapon UI")]
    public Image weaponIcon;

    [Header("Game Over UI")]
    public GameObject gameOverUI;

    [Header("Audio")]
    public AudioClip gameOverBGM;
    public AudioClip buttonSound; // ★ボタン音
    private AudioSource audioSource;

    // ボタン連打防止用のフラグ
    private bool isExiting = false;

    void Awake()
    {
        if (weaponIcon != null) weaponIcon.gameObject.SetActive(false);
        if (armorIcon != null) armorIcon.gameObject.SetActive(false);
        if (gameOverUI != null) gameOverUI.SetActive(false);

        // AudioSourceの準備
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void UpdateHP(float current, float max)
    {
        if (hpSlider != null) hpSlider.value = current / max;
    }

    public void UpdateArmor(int current, int max)
    {
        if (armorSlider != null)
        {
            if (current <= 0) armorSlider.gameObject.SetActive(false);
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
        if (armorIcon != null && icon != null) armorIcon.sprite = icon;
    }

    public void UpdateWeapon(string name) { }

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

    public void ShowGameOver()
    {
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            // 負けBGM再生
            if (audioSource != null && gameOverBGM != null)
            {
                audioSource.Stop();
                audioSource.clip = gameOverBGM;
                audioSource.loop = true;
                audioSource.Play();
            }
        }
    }

    // ★変更: コルーチンを使わない形に修正
    public void OnTitleButton()
    {
        // すでに処理中なら何もしない（連打防止）
        if (isExiting) return;
        isExiting = true;

        if (audioSource != null && buttonSound != null)
        {
            // 1. 音を鳴らす
            audioSource.PlayOneShot(buttonSound);

            // 2. Invokeを使って、音の長さ(秒)だけ待ってから "LoadTitleScene" 関数を実行予約する
            Invoke("LoadTitleScene", buttonSound.length);
        }
        else
        {
            // 音がない場合はすぐに移動
            LoadTitleScene();
        }
    }

    // ★追加: 実際にシーンを移動する関数
    // (Invokeから呼び出されるため、publicかprivateのメソッドとして定義が必要)
    void LoadTitleScene()
    {
        Time.timeScale = 1f; // 時間を通常に戻す
        SceneManager.LoadScene("TitleScene");
    }
}