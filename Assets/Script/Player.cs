using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    private float runSpeed = 7.0f;    // 通常時（走り）の速さ
    private float walkSpeed = 3.0f;   // シフト押下時（歩き）の速さ
    private float gravity = 9.81f;    // 重力の強さ
    private float jumpHeight = 1.5f;  // ジャンプの高さ

    private float soundTimer = 0f;

    private CharacterController controller;
    private Vector3 velocity;// 垂直方向の速度（重力用）
    private bool isGrounded;         // 地面に接しているか
    private bool wasGrounded;         // 前のフレームで地面にいたか（着地判定用）

    //音
    private AudioSource footstepSource;
    public AudioSource sfxSource;      // ジャンプ・着地音用の単発AudioSource
    public AudioClip jumpSound;        // ジャンプ時のSE
    public AudioClip landSound;        // 着地時のSE

    void Start()
    {
        controller = GetComponent<CharacterController>();
        footstepSource = GetComponent<AudioSource>();
        wasGrounded = true;
    }

    void Update()
    {
        // --- 地面判定 ---
        isGrounded = controller.isGrounded;

        // --- 着地処理 ---
        if (!wasGrounded && isGrounded)
        {
            // 着地音を鳴らす
            if (landSound != null && sfxSource != null) sfxSource.PlayOneShot(landSound);

            // 着地音を周囲の敵に通知（範囲を広めの20fに設定）
            NotifyEnemyOfAction(transform.position, 20f);

            // 着地した瞬間に垂直速度をリセット
            velocity.y = -2f;
        }

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // --- 速度の切り替え ---
        bool isShifting = Input.GetKey(KeyCode.LeftShift);
        float currentSpeed = isShifting ? walkSpeed : runSpeed;

        // --- WASD入力の取得 ---
        // GetAxisを使わず、直接キーを指定することで矢印キーを無効化します
        float moveX = 0;
        float moveZ = 0;

        if (Input.GetKey(KeyCode.W)) moveZ += 1;
        if (Input.GetKey(KeyCode.S)) moveZ -= 1;
        if (Input.GetKey(KeyCode.A)) moveX -= 1;
        if (Input.GetKey(KeyCode.D)) moveX += 1;

        // 斜め移動で速くならないように正規化(Normalize)
        Vector3 inputDir = new Vector3(moveX, 0, moveZ).normalized;

        // --- 移動計算 ---
        // transform.right と transform.forward を使うことで、
        // プレイヤーが向いている方向を基準に動けます。
        Vector3 move = transform.right * moveX + transform.forward * moveZ;

        // 移動の実行
        controller.Move(move * currentSpeed * Time.deltaTime);

        // --- 足音の制御 ---
        bool isMoving = inputDir.magnitude > 0;
        // 「地面にいる」かつ「動いている」かつ「シフトを押していない（走り）」なら音を出す
        if (isGrounded && isMoving && !isShifting)
        {
            if (!footstepSource.isPlaying) footstepSource.Play();
            // 敵に足音を知らせる処理
            NotifyEnemyOfFootsteps();
        }
        else
        {
            if (footstepSource.isPlaying) footstepSource.Stop();
        }

        // 簡単な重力処理 (空中に浮かないように)
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // 地面に吸い付かせる
        }
        // --- ジャンプ処理 ---
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * 1.5f * gravity);

            // ジャンプ音を鳴らす
            if (jumpSound != null && sfxSource != null) sfxSource.PlayOneShot(jumpSound);

            // ジャンプ音を周囲の敵に通知（範囲は足音と同じ15f）
            NotifyEnemyOfAction(transform.position, 15f);
        }

        // --- 重力の適用 ---
        velocity.y -= gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // 接地状態を記録
        wasGrounded = isGrounded;
    }

    //　足音を周囲の敵に通知するメソッド
    void NotifyEnemyOfFootsteps()
    {
        soundTimer += Time.deltaTime;
        if (soundTimer >= 0.3f) // 0.3秒ごとに周囲の敵に通知
        {
            soundTimer = 0f;
            float soundRadius = 15f; // 足音が届く範囲（インスペクターで調整可能にしてもOK）

            // 自分の周りのコライダーを取得
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, soundRadius);
            foreach (var hitCollider in hitColliders)
            {
                // 敵のHearSoundメソッドを呼び出す
                hitCollider.SendMessage("HearSound", transform.position, SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    // ジャンプ・着地・足音など共通の通知メソッド
    void NotifyEnemyOfAction(Vector3 position, float radius)
    {
        Collider[] hitColliders = Physics.OverlapSphere(position, radius);
        foreach (var hitCollider in hitColliders)
        {
            hitCollider.SendMessage("HearSound", position, SendMessageOptions.DontRequireReceiver);
        }
    }
}
