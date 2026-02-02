using UnityEngine;

public class ClosestEnemyPointer : MonoBehaviour
{
    public string enemyTag = "Enemy";
    public float rotationSpeed = 10f;
    private MeshRenderer meshRenderer;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    void Update()
    {
        Transform closestEnemy = GetClosestEnemy();

        if (closestEnemy != null)
        {
            // 敵がいる場合は矢印を表示し、その方向を向く
            meshRenderer.enabled = true;

            Vector3 direction = closestEnemy.position - transform.position;
            direction.y = 0; // 上下方向の回転を防ぐ

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            }
        }
        else
        {
            // 敵がいない場合は矢印を隠す
            meshRenderer.enabled = false;
        }
    }

    // シーン内の"Enemy"タグを持つオブジェクトから一番近いものを探す
    Transform GetClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        Transform closest = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = transform.position;

        foreach (GameObject enemy in enemies)
        {
            // 敵が死んでいない（Activeな）場合のみ対象にする
            if (enemy.activeInHierarchy)
            {
                float dist = Vector3.Distance(enemy.transform.position, currentPos);
                if (dist < minDist)
                {
                    closest = enemy.transform;
                    minDist = dist;
                }
            }
        }
        return closest;
    }
}