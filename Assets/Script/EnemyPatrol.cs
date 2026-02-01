using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrol : MonoBehaviour
{
    //[Header("移動速度の設定")]
    private float walkSpeed = 3.0f;       // 通常時の速さ
    private float dashSpeed = 8.0f;       // 追跡時の速さ

    // [Header("散策の設定")]
    private float walkRadius = 100f; // ランダムに移動する範囲
    private float waitTime = 2.0f;     // 待機時間（秒）

    //[Header("索敵の設定")]
    private float searchRange = 20f;      // 索敵距離
    private float searchAngle = 90f;      // 視界の角度（左右に30度ずつ）
    private float alertWaitTime = 2.0f;   // 音の場所に着いた後の警戒時間
    private Transform player;             // プレイヤーのTransform

    // --- 状態管理用フラグ ---
    private Transform targetWeapon; // 見つけた武器
    private bool isHeadingToWeapon = false;
    public bool hasWeapon = false;
    private bool isWaiting = false;   // 待機中かどうか
    private bool isChasing = false;
    private bool isInvestigating = false; // 音を調査中か

    private float attackRange = 2.0f; // 攻撃が届く距離
    private float lastAttackTime;
    private float attackCooldown = 1.5f;

    private Transform targetItem; // 追加
    private bool isHeadingToItem = false; // 追加

    private NavMeshAgent agent;
    private EnemyEquipment equipment;
    private Animator anim;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        equipment = GetComponent<EnemyEquipment>();
        agent.speed = walkSpeed;
        if (player == null) player = GameObject.FindGameObjectWithTag("Player").transform;
        SetRandomDestination();
    }

    void Update()
    {
        // ★修正: エージェントが無効、またはNavMeshに乗っていないなら処理を中断する（死亡時エラー対策）
        if (agent == null || !agent.isActiveAndEnabled || !agent.isOnNavMesh) return;

        if (anim != null) anim.SetFloat("Speed", agent.velocity.magnitude);

        // 1. プレイヤーを視界で捉えている場合
        if (hasWeapon && CanSeeObject(player))
        {
            // 発見した瞬間の初期設定
            if (!isChasing)
            {
                isChasing = true;
                isHeadingToWeapon = false;
                targetWeapon = null;
                isInvestigating = false;
                agent.speed = dashSpeed;
                agent.isStopped = false; // 追跡開始時は動けるようにする
                StopAllCoroutines();
                isWaiting = false;
            }

            float distance = Vector3.Distance(transform.position, player.position);

            // --- 攻撃の間合い判定 ---
            if (distance <= attackRange)
            {
                // 攻撃範囲内：足を止める
                agent.isStopped = true;
                agent.velocity = Vector3.zero;

                // プレイヤーの方を向く
                Vector3 lookPos = player.position - transform.position;
                lookPos.y = 0;
                if (lookPos != Vector3.zero)
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookPos), Time.deltaTime * 10f);

                // クールタイムが終わっていれば攻撃
                if (Time.time > lastAttackTime + attackCooldown)
                {
                    Attack();
                }
            }
            else
            {
                // 攻撃範囲外：追いかける
                agent.isStopped = false; // ここで移動を許可する
                agent.destination = player.position;
            }
        }
        // 2. プレイヤーが視界にいない場合
        else
        {
            // 攻撃中だったかもしれないので、移動停止を解除
            agent.isStopped = false;

            // 追跡中だったが見失った場合のリセット
            if (isChasing)
            {
                isChasing = false;
                agent.speed = walkSpeed;
                SetRandomDestination();
            }

            // 武器を探す・拾う・巡回する
            if (!hasWeapon)
            {
                SearchForWeapon();
            }
            else
            {
                SearchForItem(); // 武器があるならアイテムを探す
            }

            // アイテムへ向かう処理
            if (isHeadingToItem && targetItem != null)
            {
                agent.destination = targetItem.position;
                if (!agent.pathPending && agent.remainingDistance < 1.0f)
                {
                    PickupItem();
                }
            }

            // 武器へ向かう処理
            if (isHeadingToWeapon && targetWeapon != null)
            {
                agent.destination = targetWeapon.position;
                if (!agent.pathPending && agent.remainingDistance < 1.0f)
                {
                    EquipWeapon();
                }
            }
            else if (isInvestigating)
            {
                if (!agent.pathPending && agent.remainingDistance < 0.5f)
                {
                    StartCoroutine(InvestigateAndResume());
                }
            }
            else if (!agent.pathPending && agent.remainingDistance < 0.5f && !isWaiting)
            {
                StartCoroutine(WaitAndMove());
            }
        }
    }

    // アイテムを探すメソッド
    void SearchForWeapon()
    {
        if (hasWeapon || isHeadingToWeapon) return;

        GameObject[] items = GameObject.FindGameObjectsWithTag("Weapon");
        foreach (GameObject item in items)
        {
            // ★追加条件: 親がいない（＝地面に落ちている）ものだけを対象にする
            if (item.transform.parent == null && CanSeeObject(item.transform))
            {
                targetWeapon = item.transform;
                isHeadingToWeapon = true;
                isInvestigating = false;
                agent.speed = walkSpeed;
                break;
            }
        }
    }

    void EquipWeapon()
    {
        if (hasWeapon) return;
        if (targetWeapon == null) return;

        // ★追加条件: ターゲットに親ができていたら（＝プレイヤーに拾われたら）諦める
        if (targetWeapon.parent != null)
        {
            targetWeapon = null;
            isHeadingToWeapon = false;
            return;
        }
        string weaponName = targetWeapon.name;
        Debug.Log(weaponName + " を拾いました！");

        // 装備スクリプトに「この名前の武器を表示して」と命令する
        equipment.ChangeWeaponVisual(targetWeapon.name);

        hasWeapon = true;
        Destroy(targetWeapon.gameObject);
        targetWeapon = null;
        isHeadingToWeapon = false;
        agent.isStopped = false;
        StartCoroutine(InvestigateAndResume());
    }

    // 視界判定の汎用メソッド
    bool CanSeeObject(Transform target)
    {
        if (target == null) return false;

        float distance = Vector3.Distance(transform.position, target.position);
        if (distance < searchRange)
        {
            Vector3 eyePosition = transform.position + Vector3.up * 1.5f + transform.forward * 0.5f;
            Vector3 targetCenter = target.position + Vector3.up * 0.5f;
            Vector3 directionToTarget = (targetCenter - eyePosition).normalized;

            float angle = Vector3.Angle(transform.forward, directionToTarget);

            if (angle < searchAngle)
            {
                RaycastHit hit;
                Debug.DrawRay(eyePosition, directionToTarget * distance, Color.green);

                if (Physics.Raycast(eyePosition, directionToTarget, out hit, searchRange))
                {
                    if (hit.transform == target || hit.transform.IsChildOf(target) || target.IsChildOf(hit.transform))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    // --- プレイヤーから呼ばれる足音受信メソッド ---
    public void HearSound(Vector3 soundPosition)
    {
        // 追跡中なら音は無視する
        if (!hasWeapon || isChasing) return;

        if (isHeadingToWeapon) return;
        isInvestigating = true;
        isWaiting = false;
        StopAllCoroutines();

        agent.speed = walkSpeed; // 警戒しつつ移動
        agent.destination = soundPosition;
    }

    // 音の場所に到着した後のキョロキョロ処理
    IEnumerator InvestigateAndResume()
    {
        isInvestigating = false;
        isWaiting = true;
        yield return new WaitForSeconds(alertWaitTime);
        SetRandomDestination();
        isWaiting = false;
    }

    // 待機してから新しい目的地に移動するコルーチン
    IEnumerator WaitAndMove()
    {
        isWaiting = true;
        yield return new WaitForSeconds(waitTime);
        SetRandomDestination();
        isWaiting = false;
    }

    // ランダムな目的地を設定するメソッド
    void SetRandomDestination()
    {
        Vector3 randomDirection = Random.insideUnitSphere * walkRadius;
        randomDirection += transform.position;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, walkRadius, 1))
        {
            agent.destination = hit.position;
        }
    }

    void Attack()
    {
        lastAttackTime = Time.time;
        if (anim != null)
        {
            anim.SetTrigger("Attack");
        }
    }

    void SearchForItem()
    {
        if (isHeadingToItem || isChasing) return;

        // "Item"タグがついたオブジェクトを探す
        GameObject[] items = GameObject.FindGameObjectsWithTag("Item");
        foreach (GameObject item in items)
        {
            if (CanSeeObject(item.transform))
            {
                targetItem = item.transform;
                isHeadingToItem = true;
                agent.speed = walkSpeed;
                break;
            }
        }
    }

    void PickupItem()
    {
        if (targetItem == null) return;

        equipment.EquipItemVisual(targetItem.name);

        Destroy(targetItem.gameObject);
        targetItem = null;
        isHeadingToItem = false;

        StartCoroutine(InvestigateAndResume());
    }
}