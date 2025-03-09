using System.Collections;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class UnkoDartsPresenter : MonoBehaviour
{
    private bool isEnd;
    private float score;
    private float maxScore = 1000;
    [SerializeField] private float maxPoint = 20;
    private bool unkoflag;
    private const float timeOut = 0.04f;
    private int movePoleCount = 0;
    private float movePolePerCount;
    private Vector2 CapsulePos = new Vector2(0.0f, 4.0f);
    private float MoveSpeed = 3.0f;
    [SerializeField] GameObject Capsule;
    [SerializeField] Button button;
    [SerializeField] GameObject AxisChangerR;
    [SerializeField] GameObject AxisChangerL;
    [SerializeField] GameObject Board;
    [SerializeField] GameObject outSpace;
    [SerializeField] TextMeshProUGUI scoreBoard;
    [SerializeField] private float countdown = 60.0f;
    [SerializeField] public TextMeshProUGUI timeText;
    [SerializeField] private SoundPlayer soundPlayer;
    [SerializeField] private GameObject resultPanel;
    private int[] hitColor = {0, 0, 0};
    enum Colors{
        Red,
        Green,
        Blue
    }
    public int delay = 5; //遅延させたい秒数

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        score = 0f;
        unkoflag = false;
        isEnd = false;
        resultPanel.SetActive(false);
        resultPanel.transform.GetChild(2).gameObject.GetComponent<Button>().OnClickAsObservable().Subscribe(x => SceneLoader.NextScene());
        //UniRxで書いているよ。画面がクリックされたらsubscribeの中の処理が実行される。
        Observable.EveryUpdate().Where(_ => Input.GetMouseButtonDown(0) && !unkoflag).Subscribe(_ =>
        {
            soundPlayer.PlaySound();
            CreateCapsule();
            unkoflag = true;
        });

        Board.OnCollisionEnter2DAsObservable().Where(x => x.gameObject.tag == "Capsule").Subscribe(x =>
        {
            Vector2 hitPos = x.contacts[0].point;
            //Debug.Log(hitPos.x - Board.transform.position.x);
            float difx = hitPos.x - Board.transform.position.x;
            if (difx < 0) difx = -difx;
            if (difx <= 0.2)
            {
                score += maxScore / maxPoint * 3;
                hitColor[(int)Colors.Red]++;
                //Debug.Log("point 3!");
            }
            else if (difx <= 0.6)
            {
                score += maxScore / maxPoint * 2;
                hitColor[(int)Colors.Green]++;
                //Debug.Log("point 2!");
            }
            else
            {
                score += maxScore / maxPoint * 1;
                hitColor[(int)Colors.Blue]++;
                //Debug.Log("point 1!");
            }
            scoreBoard.text = "Score: " + Math.Floor(score).ToString();
            Destroy(x.gameObject);
            unkoflag = false;
        });
        outSpace.OnTriggerEnter2DAsObservable().Subscribe(x => { unkoflag = false; });
        //maxCount = (int)(1 / timeOut);
        movePoleCount = 1;
        movePolePerCount = 2;
        StartCoroutine(MovePole());
    }

    void CreateCapsule()
    {
        Instantiate(Capsule, CapsulePos, Quaternion.identity);
    }
    // Update is called once per frame
    void Update()
    {
        if (countdown >= 0)
        {
            countdown -= Time.deltaTime;
            timeText.text = countdown.ToString("f1") + "秒";
        }
        else
        {
            timeText.text = "終了!";
            unkoflag = true;
            if (!isEnd)
            {
                Invoke(nameof(ShowResult), delay);
                SaveManeger.SetDartsScore((int)score);
                isEnd = true;
            }
        }
    }
    void ShowResult()
    {
        resultPanel.SetActive(true);
        resultPanel.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text 
            = "赤にあてた回数："+ hitColor[(int)Colors.Red].ToString() +"\n緑にあてた回数："+hitColor[(int)Colors.Green].ToString()+"\n青にあてた回数："+ hitColor[(int)Colors.Blue].ToString() +"\n--------------------------\n合計得点	　："+ ((int)score).ToString();
    }

    //ポールをコルーチンを使って動かす
    IEnumerator MovePole()
    {
        while (true)
        {
            //timeOutの時間でループする
            if (movePoleCount <= 25)
            {
                //Debug.Log(1);
                AxisChangerL.transform.Rotate(0, 0, movePolePerCount);
                AxisChangerR.transform.Rotate(0, 0, movePolePerCount);
            }
            else if (movePoleCount <= 75)
            {
                //Debug.Log(2);
                AxisChangerL.transform.Rotate(0, 0, -movePolePerCount);
                AxisChangerR.transform.Rotate(0, 0, -movePolePerCount);
            }
            else if (movePoleCount <= 100)
            {
                //Debug.Log(3);
                AxisChangerL.transform.Rotate(0, 0, movePolePerCount);
                AxisChangerR.transform.Rotate(0, 0, movePolePerCount);
            }
            movePoleCount++;
            if (movePoleCount == 101) movePoleCount = 1;
            yield return new WaitForSeconds(timeOut);
        }
    }

    void FixedUpdate()
    {
        Board.transform.position = new Vector3(Mathf.Sin(Time.time) * MoveSpeed, -3.0f, 0);
    }
}
