using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public WeaponController weaponScript; // Pickup.csからセットされる
    private bool isAttacking = false;     // 攻撃中フラグ

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
        // 左クリック(Fire1) かつ 武器を装備している かつ 攻撃中でないなら
        if (Input.GetButtonDown("Fire1") && weaponScript != null && !isAttacking)
        {
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
        if (animator != null)
        {
            animator.SetTrigger("Attack"); // AnimatorのTriggerを起動
        }
    }

    // --- アニメーションイベント用メソッド ---
    // Animationウィンドウで作成したイベントからこれらを呼び出します
    public void AE_StartHit()
    {
        if (weaponScript != null) weaponScript.EnableHitBox();
    }

    public void AE_EndHit()
    {
        if (weaponScript != null) weaponScript.DisableHitBox();
        isAttacking = false; // 攻撃終了、次の攻撃が可能になる
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