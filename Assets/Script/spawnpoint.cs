using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawnpoint : MonoBehaviour
{
    [Header("このポイントの制限")]
    public int maxItemCount = 3;      // この地点の最大数
    public float respawnTime = 5f;    // 再湧き時間（秒）

    [Header("配置調整")]
    public float spawnRadius = 0.7f;
    public float itemCheckRadius = 0.4f;
    public int maxTryCount = 10;

    private List<GameObject> spawnedItems = new List<GameObject>();
    private float timer = 0f;

    void Update()
    {
        // null掃除
        for (int i = spawnedItems.Count - 1; i >= 0; i--)
        {
            if (spawnedItems[i] == null)
                spawnedItems.RemoveAt(i);
        }

        // すでに最大なら何もしない
        if (spawnedItems.Count >= maxItemCount)
            return;

        timer += Time.deltaTime;
        if (timer >= respawnTime)
        {
            timer = 0f;
            TrySpawn();
        }
    }

    void TrySpawn()
    {
        weapon_spawn spawner = FindObjectOfType<weapon_spawn>();
        if (spawner == null)
            return;

        Vector3 spawnPos;
        if (!FindFreePosition(out spawnPos))
            return;

        GameObject prefab = spawner.GetRandomItemPrefab();
        if (prefab == null)
            return;

        GameObject newItem = Instantiate(
            prefab,
            spawnPos,
            Quaternion.identity
        );

        spawnedItems.Add(newItem);
    }

    bool FindFreePosition(out Vector3 result)
    {
        for (int i = 0; i < maxTryCount; i++)
        {
            Vector2 offset2D = Random.insideUnitCircle * spawnRadius;
            Vector3 pos = transform.position + new Vector3(offset2D.x, 0f, offset2D.y);

            if (!Physics.CheckSphere(pos, itemCheckRadius))
            {
                result = pos;
                return true;
            }
        }

        result = Vector3.zero;
        return false;
    }
}
