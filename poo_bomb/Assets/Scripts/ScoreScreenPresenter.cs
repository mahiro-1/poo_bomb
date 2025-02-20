using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreScreenPresenter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        List<int> scores = SaveManeger.getLastScore();
        scoreText.text = "料理	："+ scores[0].ToString() +"\nダッシュ	："+ scores[1].ToString() +"\nピンボール	：" + scores[2].ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
