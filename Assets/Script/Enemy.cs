using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    public GameObject enemy;

    int num = 0;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        // 100フレーム毎にシーンにプレハブを生成
        if (Time.frameCount % 700 == 0)
        {
            // プレハブの位置をランダムで設定
            float x = Random.Range(-2.0f, -4.0f);
            float z = Random.Range(15.0f, 8.0f);
            Vector3 pos = new Vector3(x, -13.0f, z);

            // プレハブを生成
            Instantiate(enemy, pos, Quaternion.identity);
        }
    }
}