using System.Collections;
using TMPro;
using UniRx;
using UniRx.Triggers;
using Unity.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StarSceenePresenter : MonoBehaviour
{
    [SerializeField] public Button start_button;
    [SerializeField] public Button option_button;
    [SerializeField] public Button score_button;
    [SerializeField] private SoundPlayer soundPlayer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        SceneLoader.Init();
        SaveManeger.Init();
        start_button.OnClickAsObservable().Subscribe(x =>
        {
            soundPlayer.PlaySound();
            Observable.Timer(System.TimeSpan.FromSeconds(0.3)) // 0.3秒待つ
        .Subscribe(__ => SceneLoader.NextScene());
        });
        option_button.OnClickAsObservable().Subscribe(x =>
        {
            soundPlayer.PlaySound();
            Observable.Timer(System.TimeSpan.FromSeconds(0.3)) // 0.3秒待つ
        .Subscribe(__ => SceneLoader.GoOptionScreen());
            SceneLoader.GoOptionScreen();
        });
        score_button.OnClickAsObservable().Subscribe(x =>
        {
            soundPlayer.PlaySound();
            Observable.Timer(System.TimeSpan.FromSeconds(0.3)) // 0.3秒待つ
        .Subscribe(__ => SceneLoader.GoScoreScreen());
        });
    }

    // Update is called once per frame
    void Update()
    {
    }
}
