using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("体力設定")]
    public float maxHealth = 100f;
    private float currentHealth;

    public bool isDead = false;
    private Armor armor;
    public HUDManager hudManager;

    [Header("音声設定")]
    public AudioClip damageSound;
    public AudioSource audioSource;

    void Start()
    {
        currentHealth = maxHealth;
        armor = GetComponent<Armor>();

        if (hudManager != null) hudManager.UpdateHP(currentHealth, maxHealth);

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            TakeDamage(10f); // テスト用
        }
    }

    // 引数に attacker を追加（前回の変更点）
    public void TakeDamage(float amount, Transform attacker = null)
    {
        if (isDead) return;

        if (amount > 0)
        {
            if (damageSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(damageSound);
            }
        }

        int damage = Mathf.RoundToInt(amount);

        if (armor != null)
        {
            damage = armor.AbsorbDamage(damage);
        }

        if (damage > 0)
        {
            currentHealth -= damage;
            Debug.Log("HP : " + currentHealth);
        }

        if (hudManager != null) hudManager.UpdateHP(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            // 死亡処理に攻撃者を渡す
            Die(attacker);
        }
    }

    void Die(Transform killer)
    {
        isDead = true;
        Debug.Log("プレイヤーが死亡しました");

        DisableControls();
        GetComponent<Player>().StealWeapon();

        // 1. 敵視点カメラ（観戦モード）
        StartSpectatorMode(killer);

        // 2. 負けUI（赤画面・LOSE・ボタン）を表示
        if (hudManager != null)
        {
            hudManager.ShowGameOver();
        }
    }

    void DisableControls()
    {
        if (TryGetComponent<Player>(out Player movement))
        {
            movement.enabled = false;
        }

        if (TryGetComponent<FPSCameraController>(out FPSCameraController cameraControl))
        {
            cameraControl.enabled = false;
        }

        // カーソル制御はHUDManager側でShowGameOver時に行うので、ここはコメントアウトでも良いが、念のため残してもOK
        // Cursor.lockState = CursorLockMode.None;
        // Cursor.visible = true;
    }

    void StartSpectatorMode(Transform target)
    {
        if (target == null)
        {
            EnemyPatrol randomEnemy = FindObjectOfType<EnemyPatrol>();
            if (randomEnemy != null) target = randomEnemy.transform;
        }

        if (target != null)
        {
            FPSCameraController camControl = GetComponent<FPSCameraController>();
            Transform mainCamera = null;

            if (camControl != null && camControl.cameraTransform != null)
            {
                mainCamera = camControl.cameraTransform;
            }
            else if (Camera.main != null)
            {
                mainCamera = Camera.main.transform;
            }

            if (mainCamera != null)
            {
                mainCamera.SetParent(target);
                mainCamera.localPosition = new Vector3(0f, 2.5f, -4.0f);
                mainCamera.LookAt(target.position + Vector3.up * 1.5f);
            }
        }
    }
}