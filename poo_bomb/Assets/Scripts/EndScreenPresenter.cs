using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UniRx.Triggers;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndscreenPresenter : MonoBehaviour
{

    [SerializeField] public Button end_button;
    [SerializeField] private string Startscreen; //シーン名を記述
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        end_button.OnClickAsObservable().Subscribe(x => {
            //テスト用！後で消してねーーーーーーーーーーーーーー
            //SaveManeger.Init();
            //ーーーーーーーーーーーーーーーーーーーーーーーーー
            SaveManeger.SaveFile();
            //SceneManager.LoadScene("StartScreen");
            SceneLoader.NextScene();
        });
    }

    // Update is called once per frame
    void Update()
    {

    }
}
