using System;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

public class ScoreScreenPresenter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Button return_button;
    [SerializeField] private SoundPlayer soundPlayer;
    [SerializeField] private TextMeshProUGUI rankingText;
    [SerializeField] private Button saveClearButton;
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;
    private int nowRank;
    List<SaveManeger.OnceScore> scores;
    private bool isClearButtonPressed = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        nowRank = 1;
        scores = SaveManeger.getRanking();
        ShowScore();

        leftButton.OnClickAsObservable().Subscribe(x => {
            nowRank++;
            if(nowRank > scores.Count) nowRank = scores.Count;
            ShowScore();
            soundPlayer.PlaySound();
        });
        rightButton.OnClickAsObservable().Subscribe(x => {
            nowRank--;
            if(nowRank < 1) nowRank = 1;
            ShowScore();
            soundPlayer.PlaySound();
        });

        saveClearButton.OnPointerDownAsObservable()
            .SelectMany(_ => Observable.Interval(TimeSpan.FromSeconds(2)))
            .TakeUntil(saveClearButton.OnPointerUpAsObservable())
            .DoOnCompleted(() =>
            {
                Debug.Log("released!");
                if(isClearButtonPressed){
                    soundPlayer.PlaySound();
                    SaveManeger.AllClear();
                    scores = SaveManeger.getRanking();
                    isClearButtonPressed = false;
                }
            })
            .RepeatUntilDestroy(saveClearButton)
            .Subscribe(time =>
            {
                Debug.Log("pressing..." + time);
                isClearButtonPressed = true;
            });

        //戻るボタンの処理
        return_button.OnClickAsObservable().Subscribe(x =>
        {
            soundPlayer.PlaySound();
            Observable.Timer(System.TimeSpan.FromSeconds(0.3)) // 0.3秒待つ
        .Subscribe(__ => SceneLoader.ReturnStartScreen());
        });

    }
    void ShowScore(){
        rankingText.text = nowRank.ToString() + "位";
        scoreText.text = "料理	：" + scores[nowRank-1].CookingScore.ToString() + "\nダッシュ	：" + scores[nowRank-1].DashScore.ToString() + "\nピンボール	：" + scores[nowRank-1].DartsScore.ToString();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
