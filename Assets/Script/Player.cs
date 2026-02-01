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
    public WeaponController weaponScript; // Pickup.csからセットされる（攻撃判定用）
    private bool isAttacking = false;     // 攻撃中フラグ

    // ---  表示切り替え用の武器オブジェクト参照 ---
    [Header("Weapon Models")]
    public GameObject cameraWeaponObj; // カメラ追従用の武器（普段見える）
    public GameObject handWeaponObj;   // 手追従用の武器（攻撃時に見える）

    private Coroutine resetWeaponCoroutine; // リセット処理を管理する変数

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
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        wasGrounded = true;
    }

    void Update()
    {
        if (GameManager.isPaused) return;

        isGrounded = controller.isGrounded;

        if (animator != null)
        {
            animator.SetBool("IsGrounded", isGrounded);
        }

        // --- 攻撃入力の検知 ---
        if (Input.GetButtonDown("Fire1") && weaponScript != null)
        {
            if (animator != null)
            {
                animator.Play("Attack", -1, 0f);
            }
            StartAttack();
        }

        if (!wasGrounded && isGrounded)
        {
            if (landSound != null && sfxSource != null) sfxSource.PlayOneShot(landSound);
            NotifyEnemyOfAction(transform.position, 20f);
            velocity.y = -2f;
            StartCoroutine(JumpCooldownRoutine());
        }

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        bool isShifting = Input.GetKey(KeyCode.LeftShift);
        float currentSpeed = isShifting ? walkSpeed : runSpeed;

        float moveX = 0;
        float moveZ = 0;
        if (Input.GetKey(KeyCode.W)) moveZ += 1;
        if (Input.GetKey(KeyCode.S)) moveZ -= 1;
        if (Input.GetKey(KeyCode.A)) moveX -= 1;
        if (Input.GetKey(KeyCode.D)) moveX += 1;

        Vector3 inputDir = new Vector3(moveX, 0, moveZ).normalized;

        if (animator != null)
        {
            float animX = inputDir.x * currentSpeed;
            float animZ = inputDir.z * currentSpeed;
            animator.SetFloat("InputX", animX, 0.1f, Time.deltaTime);
            animator.SetFloat("InputZ", animZ, 0.1f, Time.deltaTime);
        }

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        controller.Move(move * currentSpeed * Time.deltaTime);

        bool isMoving = inputDir.magnitude > 0;
        if (isGrounded && isMoving && !isShifting)
        {
            if (!footstepSource.isPlaying) footstepSource.Play();
            NotifyEnemyOfFootsteps();
        }
        else
        {
            if (footstepSource.isPlaying) footstepSource.Stop();
        }

        if (Input.GetButtonDown("Jump") && isGrounded && canJump)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * 1.5f * gravity);
            if (jumpSound != null && sfxSource != null) sfxSource.PlayOneShot(jumpSound);
            NotifyEnemyOfAction(transform.position, 15f);

            if (animator != null)
            {
                animator.SetTrigger("Jump");
            }
            canJump = false;
        }

        velocity.y -= gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        wasGrounded = isGrounded;
    }

    // --- 攻撃開始メソッド ---
    void StartAttack()
    {
        isAttacking = true;

        // 1. 武器の表示切り替え（手元を表示）
        if (cameraWeaponObj != null) cameraWeaponObj.SetActive(false);
        if (handWeaponObj != null) handWeaponObj.SetActive(true);

        // 2. アニメーション再生
        if (animator != null)
        {
            // 強制的に最初から再生
            animator.Play("Attack", -1, 0f);
            animator.SetTrigger("Attack");
        }

        // 3. 当たり判定のリセット
        if (weaponScript != null)
        {
            weaponScript.DisableHitBox();
        }

        // 4. 強制リセットの予約
        // もし前のリセット待ちが残っていたらキャンセルして、新しく予約し直す
        if (resetWeaponCoroutine != null) StopCoroutine(resetWeaponCoroutine);

        //「0.6f」を、あなたのアニメーションの長さに合わせて調整してください！
        // （少し短めに設定するのがコツです）
        resetWeaponCoroutine = StartCoroutine(ForceResetWeapon(1.0f));
    }

    // ★追加: 強制リセット用のコルーチン
    IEnumerator ForceResetWeapon(float delay)
    {
        yield return new WaitForSeconds(delay);

        // もし攻撃中なら、強制的に終了処理を呼ぶ
        if (isAttacking)
        {
            AE_EndHit();
        }
    }

    // --- アニメーションイベント用メソッド ---
    public void AE_StartHit()
    {
        if (weaponScript != null) weaponScript.EnableHitBox();
    }

    public void AE_EndHit()
    {
        // 処理が走ったら、待機中のコルーチンは破棄する（二重実行防止）
        if (resetWeaponCoroutine != null)
        {
            StopCoroutine(resetWeaponCoroutine);
            resetWeaponCoroutine = null;
        }

        if (weaponScript != null) weaponScript.DisableHitBox();

        isAttacking = false;

        // ★武器の表示を元に戻す（カメラ武器を表示）
        if (cameraWeaponObj != null) cameraWeaponObj.SetActive(true);
        if (handWeaponObj != null) handWeaponObj.SetActive(false);
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