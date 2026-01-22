using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    public float runSpeed = 7.0f;    // 通常時（走り）の速さ
    public float walkSpeed = 3.0f;   // シフト押下時（歩き）の速さ
    public float gravity = 9.81f;    // 重力の強さ
    public float jumpHeight = 2.0f;  // ジャンプの高さ

    private CharacterController controller;
    private Vector3 velocity;// 垂直方向の速度（重力用）
    private bool isGrounded;         // 地面に接しているか

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // --- 地面判定 ---
        // CharacterControllerが地面についているか確認
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            // 地面にいる時は垂直方向の速度をリセット（少し押し付ける）
            velocity.y = -2f;
        }

        // --- 速度の切り替え ---
        // LeftShift を押している間は walkSpeed、そうでない時は runSpeed を使う
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? walkSpeed : runSpeed;

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

        // 簡単な重力処理 (空中に浮かないように)
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // 地面に吸い付かせる
        }

        // --- ジャンプ処理 ---
        // 地面にいて、かつジャンプボタン（デフォルトはSpace）が押されたら
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            // 物理公式: 速度 = √(ジャンプの高さ * 1.5 * 重力)
            velocity.y = Mathf.Sqrt(jumpHeight * 1.5f * gravity);
        }

        // --- 重力の適用 ---
        velocity.y -= gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);


    }
}
