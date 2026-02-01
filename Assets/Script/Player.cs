using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    private float runSpeed = 7.0f;
    private float walkSpeed = 3.0f;
    private float gravity = 9.81f;
    private float jumpHeight = 1.5f;

    private float soundTimer = 0f;
    private float jumpCooldown = 1.1f;
    private bool canJump = true;

    // --- 攻撃用変数 ---
    [Header("Combat Settings")]
    public WeaponController weaponScript;
    private bool isAttacking = false;

    // --- ★ここを修正しました（名前をPickup.csと統一） ---
    [Header("Weapon Models")]
    public GameObject currentCameraWeapon; // 普段見える武器
    public GameObject currentHandWeapon;   // 攻撃時に見える武器

    private Coroutine resetWeaponCoroutine;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private bool wasGrounded;

    private AudioSource footstepSource;
    public AudioSource sfxSource;
    public AudioClip jumpSound;
    public AudioClip landSound;

    private Animator animator;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        footstepSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        if (animator == null) animator = GetComponentInChildren<Animator>();
        wasGrounded = true;
    }

    void Update()
    {
        if (GameManager.isPaused) return;

        // --- 攻撃入力 ---
        if (Input.GetButtonDown("Fire1") && weaponScript != null)
        {
            StartAttack();
        }

        HandleMovement();
    }

    // --- 攻撃開始 ---
    void StartAttack()
    {
        isAttacking = true;

        // 1. 武器の表示切り替え（名前修正済み）
        if (currentCameraWeapon != null) currentCameraWeapon.SetActive(false);
        if (currentHandWeapon != null) currentHandWeapon.SetActive(true);

        // 2. アニメーション再生
        if (animator != null)
        {
            animator.Play("Attack", -1, 0f);
        }

        // 3. 当たり判定リセット
        if (weaponScript != null) weaponScript.DisableHitBox();

        // 4. 強制リセット予約
        if (resetWeaponCoroutine != null) StopCoroutine(resetWeaponCoroutine);

        // アニメーションの長さに合わせて時間を調整してください（例: 0.6f）
        resetWeaponCoroutine = StartCoroutine(ForceResetWeapon(0.6f));
    }

    // --- 強制リセット ---
    IEnumerator ForceResetWeapon(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (isAttacking) AE_EndHit();
    }

    // --- 攻撃終了処理 ---
    public void AE_EndHit()
    {
        if (resetWeaponCoroutine != null)
        {
            StopCoroutine(resetWeaponCoroutine);
            resetWeaponCoroutine = null;
        }

        if (weaponScript != null) weaponScript.DisableHitBox();

        isAttacking = false;

        // ★表示を元に戻す（名前修正済み）
        if (currentHandWeapon != null) currentHandWeapon.SetActive(false);
        if (currentCameraWeapon != null) currentCameraWeapon.SetActive(true);
    }

    // --- 移動関係 ---
    void HandleMovement()
    {
        isGrounded = controller.isGrounded;
        if (animator != null) animator.SetBool("IsGrounded", isGrounded);

        if (!wasGrounded && isGrounded)
        {
            if (landSound != null && sfxSource != null) sfxSource.PlayOneShot(landSound);
            NotifyEnemyOfAction(transform.position, 20f);
            velocity.y = -2f;
            StartCoroutine(JumpCooldownRoutine());
        }
        wasGrounded = isGrounded;

        if (isGrounded && velocity.y < 0) velocity.y = -2f;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        bool isShifting = Input.GetKey(KeyCode.LeftShift);
        float currentSpeed = isShifting ? walkSpeed : runSpeed;

        Vector3 move = transform.right * h + transform.forward * v;
        controller.Move(move * currentSpeed * Time.deltaTime);

        // 重力
        if (Input.GetButtonDown("Jump") && isGrounded && canJump)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * -9.81f);
            if (jumpSound) sfxSource.PlayOneShot(jumpSound);
            NotifyEnemyOfAction(transform.position, 15f);
            if (animator) animator.SetTrigger("Jump");
            canJump = false;
        }

        velocity.y += -9.81f * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // アニメーション
        if (animator != null)
        {
            animator.SetFloat("InputX", h * currentSpeed);
            animator.SetFloat("InputZ", v * currentSpeed);
        }

        // 足音
        if (isGrounded && move.magnitude > 0 && !isShifting)
        {
            if (!footstepSource.isPlaying) footstepSource.Play();
            NotifyEnemyOfFootsteps();
        }
        else
        {
            if (footstepSource.isPlaying) footstepSource.Stop();
        }
    }

    IEnumerator JumpCooldownRoutine()
    {
        canJump = false;
        yield return new WaitForSeconds(jumpCooldown);
        canJump = true;
    }

    void NotifyEnemyOfFootsteps()
    {
        soundTimer += Time.deltaTime;
        if (soundTimer >= 0.3f)
        {
            soundTimer = 0f;
            NotifyEnemyOfAction(transform.position, 15f);
        }
    }

    void NotifyEnemyOfAction(Vector3 position, float radius)
    {
        Collider[] hitColliders = Physics.OverlapSphere(position, radius);
        foreach (var hitCollider in hitColliders)
        {
            hitCollider.SendMessage("HearSound", position, SendMessageOptions.DontRequireReceiver);
        }
    }
}