using UnityEngine;

public class SimpleWeapon : MonoBehaviour
{
    [Header("この武器の攻撃力")]
    public int damage = 10; // ★ここをPublicにするのがポイント！

    // 何かに当たった時の処理
    private void OnCollisionEnter(Collision collision)
    {
        // "Enemy" タグがついている相手に当たった場合
        if (collision.gameObject.CompareTag("enemy_mob"))
        {
            // 相手のHPスクリプトを取得（名前はあなたのプロジェクトに合わせてね）
            enemy_HP targetHP = collision.gameObject.GetComponent<enemy_HP>();

            if (targetHP != null)
            {
                // 設定したダメージを与える
                // 注: enemy_HP側のHitPointがstaticだと全敵共通になっちゃうので注意
                // 今回は書き方として参考にしてください
                enemy_HP.HitPoint -= damage;

                // 当たったことがわかるようにログを出す
                Debug.Log(damage + " ダメージ与えた！");
            }
        }
    }
}