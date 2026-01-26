using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Club : MonoBehaviour
{
    private int hit = 5;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("enemy_mob"))
        {
            enemy_HP enemyHP = collision.gameObject.GetComponent<enemy_HP>();

            if (enemyHP != null)
            {
                enemy_HP.HitPoint -= hit;
            }
        }
    }
}
