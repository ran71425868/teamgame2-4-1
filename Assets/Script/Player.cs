using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    public float speed = 5.0f;       // 歩く速さ
    public float gravity = 9.81f;    // 重力の強さ

    private CharacterController controller;
    private Vector3 velocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // 1. キーボード入力を取得 (W,A,S,D または 矢印キー)
        float moveX = Input.GetAxis("Horizontal"); // 左右 (A/D)
        float moveZ = Input.GetAxis("Vertical");   // 前後 (W/S)

        // 2. 「プレイヤーの正面と右」を基準に移動方向を計算
        // transform.right と transform.forward を使うことで、
        // プレイヤーが向いている方向を基準に動けます。
        Vector3 move = transform.right * moveX + transform.forward * moveZ;

        // 3. 移動の実行
        controller.Move(move * speed * Time.deltaTime);

        // 4. 簡単な重力処理 (空中に浮かないように)
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // 地面に吸い付かせる
        }

        velocity.y -= gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
