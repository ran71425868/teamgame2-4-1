using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrol : MonoBehaviour
{

    [Header("移動速度の設定")]
    private float walkSpeed = 3.0f;       // 通常時の速さ
    private float dashSpeed = 8.0f;       // 追跡時の速さ

    [Header("散策の設定")]
    private float walkRadius = 100f; // ランダムに移動する範囲
   private float waitTime = 2.0f;     // 待機時間（秒）

    [Header("索敵の設定")]
    private float searchRange = 20f;      // 索敵距離
    private float searchAngle = 90f;      // 視界の角度（左右に30度ずつ）
    private Transform player;             // プレイヤーのTransform

    private NavMeshAgent agent;
    private bool isWaiting = false;   // 待機中かどうか
    private bool isChasing = false;
    private Animator anim; // 追加



    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>(); // Animatorを取得
        agent.speed = walkSpeed; // 最初は歩き速度に設定

        // プレイヤーをタグで自動取得（タグが"Player"に設定されていること）
        if (player == null) player = GameObject.FindGameObjectWithTag("Player").transform;

        SetRandomDestination();
    }

    void Update()
    {

        // 1. プレイヤーが視界に入っているかチェック
        if (CanSeePlayer())
        {
            // 現在の速度をAnimatorに伝える
            float currentSpeed = agent.velocity.magnitude;
            anim.SetFloat("Speed", currentSpeed);
            if (!isChasing)
            {
                isChasing = true;
                agent.speed = dashSpeed; // 速度をダッシュに切り替え
                StopAllCoroutines(); // 待機処理などを中断
                isWaiting = false;
            }
            agent.destination = player.position; // 追跡
        }
        else
        {
            // 2. プレイヤーを見失った後の処理
            if (isChasing)
            {
                isChasing = false;
                agent.speed = walkSpeed; // 速度を歩きに戻す
                SetRandomDestination(); // 再び散策へ
            }

            // 3. 通常のランダム散策
            if (!agent.pathPending && agent.remainingDistance < 0.5f && !isWaiting)
            {
                StartCoroutine(WaitAndMove());
            }
        }
    }

    bool CanSeePlayer()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance < searchRange)
        {
            // プレイヤーへの方向
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            // 自分の正面との角度差
            float angle = Vector3.Angle(transform.forward, directionToPlayer);

            if (angle < searchAngle)
            {
                // 間に障害物がないかレイキャストで確認
                RaycastHit hit;
                if (Physics.Raycast(transform.position + Vector3.up, directionToPlayer, out hit, searchRange))
                {
                    if (hit.transform == player) return true;
                }
            }
        }
        return false;
    }

    IEnumerator WaitAndMove()
    {
        isWaiting = true;

        // ここで待機（waitTime秒だけ処理を中断する）
        yield return new WaitForSeconds(waitTime);

        // 新しい目的地を設定
        SetRandomDestination();

        isWaiting = false;
    }


    void SetRandomDestination()
    {
        // 現在地を中心に、walkRadiusの範囲内でランダムな方向を決定
        Vector3 randomDirection = Random.insideUnitSphere * walkRadius;
        randomDirection += transform.position;

        NavMeshHit hit;
        // 指定した座標から一番近い「歩けるNavMesh」上の点を取得
        if (NavMesh.SamplePosition(randomDirection, out hit, walkRadius, 1))
        {
            agent.destination = hit.position;
        }
    }
}
