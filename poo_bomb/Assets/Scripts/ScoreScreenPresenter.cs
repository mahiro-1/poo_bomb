using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class ScoreScreenPresenter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Button return_button;
    [SerializeField] private SoundPlayer soundPlayer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        List<int> scores = SaveManeger.getLastScore();
        scoreText.text = "料理	：" + scores[0].ToString() + "\nダッシュ	：" + scores[1].ToString() + "\nピンボール	：" + scores[2].ToString();

        return_button.OnClickAsObservable().Subscribe(x =>
        {
            soundPlayer.PlaySound();
            Observable.Timer(System.TimeSpan.FromSeconds(0.5)) // 0.5秒待つ
        .Subscribe(__ => SceneLoader.ReturnStartScreen());
        });

    }

    // Update is called once per frame
    void Update()
    {

    }
}
