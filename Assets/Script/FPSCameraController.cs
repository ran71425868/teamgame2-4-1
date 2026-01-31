using UnityEngine;

public class FPSCameraController : MonoBehaviour
{
    [Header("設定")]
    public float sensitivity = 2.0f;
    public Transform cameraTransform;

    private float minVerticalAngle = -90f; // 上を向く限界（-90で真上）
    private float maxVerticalAngle = 60f;  // 下を向く限界（90で真下。ここを小さくする！）

    private float rotationX = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (GameManager.isPaused) return;

        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        transform.Rotate(Vector3.up * mouseX);

        rotationX -= mouseY;

        // ---  固定の数値ではなく、設定した変数を使う ---
        // 元: rotationX = Mathf.Clamp(rotationX, -90f, 90f);
        rotationX = Mathf.Clamp(rotationX, minVerticalAngle, maxVerticalAngle);

        cameraTransform.localRotation = Quaternion.Euler(rotationX, 0, 0);
    }

    public void SetSensitivity(float newSensitivity)
    {
        sensitivity = newSensitivity;
    }
}