using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // 追加

public class TitleManager : MonoBehaviour
{
    public void OnStartButton()
    {
        SceneManager.LoadScene("Map_v1"); // "" の部分はシーンの名前に変更
    }
    public void OnTutorialButton()
    {
        SceneManager.LoadScene("Tutorial"); // "" の部分はシーンの名前に変更
    }
}