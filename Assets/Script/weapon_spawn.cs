using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weapon_spawn : MonoBehaviour
{
    public GameObject[] itemPrefabs;

    public GameObject GetRandomItemPrefab()
    {
        if (itemPrefabs.Length == 0)
            return null;

        return itemPrefabs[Random.Range(0, itemPrefabs.Length)];
    }

}
