using UnityEngine;
using TMPro; // TextMeshProを使うために必要

public class DamagePopup : MonoBehaviour
{
    private TextMeshPro textMesh;
    private float disappearTimer = 1f; // 消えるまでの時間
    private Color textColor;
    private Vector3 moveVector;

    void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
    }

    // 数字をセットアップするメソッド
    public void Setup(int damageAmount)
    {
        textMesh.SetText(damageAmount.ToString());
        textColor = textMesh.color;

        // 少しランダムな方向に飛び出す（モンハン風）
        moveVector = new Vector3(Random.Range(-0.5f, 0.5f), 1f, Random.Range(-0.5f, 0.5f)) * 2f;
    }

    void Update()
    {
        // 1. 上昇しながら少し移動
        transform.position += moveVector * Time.deltaTime;
        moveVector -= moveVector * 2f * Time.deltaTime; // 勢いを減衰させる

        // 2. 常にカメラの方を向く（ビルボード処理）
        if (Camera.main != null)
        {
            transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
        }

        // 3. 徐々に透明にして消す
        disappearTimer -= Time.deltaTime;
        if (disappearTimer < 0)
        {
            float disappearSpeed = 3f;
            textColor.a -= disappearSpeed * Time.deltaTime;
            textMesh.color = textColor;

            if (textColor.a < 0)
            {
                Destroy(gameObject);
            }
        }
    }
}