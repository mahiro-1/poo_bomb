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
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private SoundPlayer soundPlayer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        scoreText.text = "スコア\nCooking	:" + SaveManeger.GetCookingScore().ToString() + "\nDash	:" + SaveManeger.GetDashScore().ToString() + "\nDarts	:" + SaveManeger.GetDartsScore().ToString() + "\nTotal	:" + (SaveManeger.GetCookingScore() + SaveManeger.GetDashScore() + SaveManeger.GetDartsScore()).ToString();
        end_button.OnClickAsObservable().Subscribe(_ =>
        {
            SaveManeger.SaveFile();
            soundPlayer.PlaySound();
            Observable.Timer(System.TimeSpan.FromSeconds(0.5)) // 0.5秒待つ
        .Subscribe(__ => SceneLoader.NextScene());

        });
    }

    // Update is called once per frame
    void Update()
    {

    }
}
