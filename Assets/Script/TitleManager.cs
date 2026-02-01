using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] private AudioClip bgmClip; // BGMのオーディオクリップ
    [SerializeField] private AudioClip seClip;  // ボタンのSEオーディオクリップ

    void Start()
    {
        // BGMの設定と再生
        if (bgmClip != null)
        {
            // BGM用のAudioSourceを追加して設定
            AudioSource bgmSource = gameObject.AddComponent<AudioSource>();
            bgmSource.clip = bgmClip;
            bgmSource.loop = true; // ループ再生
            bgmSource.Play();
        }
    }

    public void OnStartButton()
    {
        Debug.Log("ボタンが押されました！音を鳴らす命令を出します！"); // この行を追加
        PlaySeAndLoadScene("Map_v1");
    }

    public void OnTutorialButton()
    {
        PlaySeAndLoadScene("Tutorial");
    }

    // SEを鳴らしてシーン移動する共通の処理
    private void PlaySeAndLoadScene(string sceneName)
    {
        if (seClip != null)
        {
            // SEを鳴らすための空のオブジェクトを新しく作る
            GameObject soundObj = new GameObject("ButtonSE");

            // AudioSourceをつけて設定する
            AudioSource seSource = soundObj.AddComponent<AudioSource>();
            seSource.clip = seClip;
            seSource.Play();

            // ★重要：シーンが変わってもこのオブジェクト（音）が消えないようにする
            DontDestroyOnLoad(soundObj);

            // 音の長さ秒後に自動で削除されるように予約しておく
            Destroy(soundObj, seClip.length);
        }

        // 待たずにすぐシーン移動
        SceneManager.LoadScene(sceneName);
    }
}