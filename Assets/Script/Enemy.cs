using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// 【初期配置専用】ゲーム開始時に敵をまとめて生成するクラス
/// ※ Update等で継続的に湧かせる処理はないため、Start時の一回のみ実行されます
/// </summary>
public class Enemy : MonoBehaviour
{
    [Header("Spawn Target")]
    public GameObject enemyPrefab;   // 生成する敵のプレハブ
    public Transform player;         // プレイヤー（ここを基準に距離を測る）

    [Header("Spawn Settings")]
    public int enemyCount = 10;              // 合計で何体出すか
    public float spawnRadius = 40f;           // プレイヤーからどのくらい離れた範囲に出すか
    public float minDistanceFromPlayer = 12f; // プレイヤーに近すぎると不自然なので最低これだけ離す
    public float minDistanceBetweenEnemies = 2.5f; // 敵同士が重ならないように離す距離

    public int maxTryCount = 30; // 1体配置する場所を決めるのに何回までやり直すか

    // すでに配置が決まった場所を記録しておくリスト
    List<Vector3> usedPositions = new List<Vector3>();

    void Start()
    {
        // ゲーム開始時に1回だけ実行
        StartCoroutine(PlaceEnemies());
    }
    private bool hasSpawned = false; // 湧いたかどうかのフラグ

    IEnumerator PlaceEnemies()
    {
        // すでに湧かせた後なら、二度と実行しない
        if (hasSpawned) yield break;
        hasSpawned = true;

        yield return null;

        if (GameClear.instance != null)
        {
            GameClear.instance.SetEnemyCount(enemyCount);
        }

        for (int i = 0; i < enemyCount; i++)
        {
            TryPlaceEnemy();
        }

        // 配置が終わったら、このスポーン機能を自爆（無効化）させる
        this.enabled = false;
        Destroy(this.gameObject);
    }

    /// <summary>
    /// 1体分の最適なスポーン地点を探して生成する
    /// </summary>
    void TryPlaceEnemy()
    {
        for (int i = 0; i < maxTryCount; i++)
        {
            // 1. プレイヤー周辺の空中も含めた球体状のランダムな座標を計算
            Vector3 randomPos = player.position + Random.insideUnitSphere * spawnRadius;
            randomPos.y = player.position.y; // 高さはプレイヤーと同じにする

            // 2. その座標の近くにNavMesh（歩ける床）があるか確認
            if (!NavMesh.SamplePosition(randomPos, out NavMeshHit hit, 3f, NavMesh.AllAreas))
                continue; // 床がなければやり直し

            // 3. 建物の中（壁や屋根の近く）かどうかを判定
            bool isInside = false;
            // 足元から少し上(3m)から半径5mの球体で「Building」タグのものを探す
            Collider[] hitColliders = Physics.OverlapSphere(hit.position + Vector3.up * 3f, 5.0f);
            foreach (var col in hitColliders)
            {
                if (col.CompareTag("Building"))
                {
                    isInside = true;
                    break;
                }
            }
            if (isInside) continue; // 建物内ならNG、やり直し

            // 4. プレイヤーとの距離チェック
            if (Vector3.Distance(hit.position, player.position) < minDistanceFromPlayer)
                continue; // 近すぎたらやり直し

            // 5. 他の敵との距離チェック（密集防止）
            bool tooClose = false;
            foreach (var pos in usedPositions)
            {
                if (Vector3.Distance(hit.position, pos) < minDistanceBetweenEnemies)
                {
                    tooClose = true;
                    break;
                }
            }
            if (tooClose) continue; // 他の敵と近すぎたらやり直し

            // 6. プレイヤーの視界に入っているかチェック
            if (IsInPlayerView(hit.position)) continue; // 見ている前でパッと出ないようにやり直し

            // 全てのチェックをクリア！敵を生成
            Instantiate(enemyPrefab, hit.position, Quaternion.identity);

            // 生成した場所を記録（次の敵の距離チェックに使う）
            usedPositions.Add(hit.position);
            return; // 1体出せたらこの関数のループを抜ける
        }
    }

    /// <summary>
    /// カメラの視界内かどうかを数学的に判定する
    /// </summary>
    bool IsInPlayerView(Vector3 worldPos)
    {
        // 3D座標を画面上の座標(0～1)に変換
        Vector3 viewportPos = Camera.main.WorldToViewportPoint(worldPos);

        // Zがプラス（カメラの前方）かつ、XとYが0～1の間なら「画面に映っている」
        return viewportPos.z > 0 &&
               viewportPos.x > 0 && viewportPos.x < 1 &&
               viewportPos.y > 0 && viewportPos.y < 1;
    }
}