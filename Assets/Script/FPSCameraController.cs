using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSCameraController : MonoBehaviour
{
    [Header("設定")]
    public float sensitivity = 2.0f; // マウス感度
    public Transform cameraTransform; // Main Cameraをここにドラッグ&ドロップ

    private float rotationX = 0f; // 上下の回転量を保持する変数

    void Start()
    {
        // マウスカーソルを画面中央に固定して消す
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // 1. マウスの移動量を取得
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        // 2. 【左右の回転】
        // プレイヤーの体（このスクリプトがついたオブジェクト）をY軸中心に回す
        transform.Rotate(Vector3.up * mouseX);

        // 3. 【上下の回転】
        // マウスの縦の動きに合わせて角度を計算（-= にするのが一般的）
        rotationX -= mouseY;
        // 真上・真下を向きすぎて首が1回転しないように制限
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);

        // 4. 【カメラ（首）に反映】
        // カメラのローカルの回転（X軸）だけを書き換える
        cameraTransform.localRotation = Quaternion.Euler(rotationX, 0, 0);
    }
}