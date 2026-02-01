using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// ゲーム開始時に、マップ上へ敵を指定数ランダム配置するクラス
/// ・NavMesh上のみ
/// ・プレイヤー近く＆視界内は避ける
/// ・敵同士が固まらない
/// </summary>
public class Enemy : MonoBehaviour
{
    [Header("Spawn Target")]
    public GameObject enemyPrefab;   // 配置する敵のPrefab
    public Transform player;         // プレイヤー（基準点）

    [Header("Spawn Settings")]
    public int enemyCount = 10;              // 初期配置する敵の数
    public float spawnRadius = 40f;           // プレイヤー周囲の配置半径
    public float minDistanceFromPlayer = 12f; // プレイヤーとの最低距離
    public float minDistanceBetweenEnemies = 2.5f; // 敵同士の最低距離

    public int maxTryCount = 30; // 1体配置するための最大試行回数

    // すでに配置した敵の位置リスト（重なり防止用）
    List<Vector3> usedPositions = new List<Vector3>();

    void Start()
    {
        // シーン開始時に初期配置を開始
        StartCoroutine(PlaceEnemies());
    }

    /// <summary>
    /// 敵を指定数だけ配置する
    /// </summary>
    IEnumerator PlaceEnemies()
    {
        // 1フレーム待って、NavMesh / Player / Camera の初期化を待つ
        yield return null;

        for (int i = 0; i < enemyCount; i++)
        {
            TryPlaceEnemy();
        }
    }

    /// <summary>
    /// 条件を満たすランダム位置を探して敵を1体配置する
    /// </summary>
    void TryPlaceEnemy()
    {
        for (int i = 0; i < maxTryCount; i++)
        {
            // プレイヤーを中心にランダムな位置を生成
            Vector3 randomPos = player.position + Random.insideUnitSphere * spawnRadius;
            randomPos.y = player.position.y;

            // NavMesh上の有効な位置に変換
            if (!NavMesh.SamplePosition(randomPos, out NavMeshHit hit, 3f, NavMesh.AllAreas))
                continue;

            // プレイヤーに近すぎる位置はNG
            if (Vector3.Distance(hit.position, player.position) < minDistanceFromPlayer)
                continue;

            // すでに配置済みの敵と近すぎないかチェック
            bool tooClose = false;
            foreach (var pos in usedPositions)
            {
                if (Vector3.Distance(hit.position, pos) < minDistanceBetweenEnemies)
                {
                    tooClose = true;
                    break;
                }
            }
            if (tooClose)
                continue;

            // プレイヤーの視界内に入る位置は避ける（FPSの違和感防止）
            if (IsInPlayerView(hit.position))
                continue;

            // 条件をすべて満たしたら敵を生成
            Instantiate(enemyPrefab, hit.position, Quaternion.identity);
            usedPositions.Add(hit.position);
            return;
        }
    }

    /// <summary>
    /// 指定したワールド座標がプレイヤーの画面内に入っているか判定
    /// </summary>
    bool IsInPlayerView(Vector3 worldPos)
    {
        // MainCameraタグが付いたカメラを取得
        Vector3 viewportPos = Camera.main.WorldToViewportPoint(worldPos);

        // 画面内（x,yが0～1）かつカメラ前方（z > 0）なら視界内
        return viewportPos.z > 0 &&
               viewportPos.x > 0 && viewportPos.x < 1 &&
               viewportPos.y > 0 && viewportPos.y < 1;
    }
}
