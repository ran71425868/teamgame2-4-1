using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManeger : MonoBehaviour
{
    void Update()
    {
        // エンターキーが押された瞬間
        if (Input.GetKeyDown(KeyCode.Return))
        {
            SceneManager.LoadScene("Map_v1"); // シーン名
        }
    }
}