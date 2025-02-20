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
    private int score;
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
    public int delay = 5; //遅延させたい秒数

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        score = 0;
        unkoflag = false;
        //UniRxで書いているよ。画面がクリックされたらsubscribeの中の処理が実行される。
        Observable.EveryUpdate().Where(_ => Input.GetMouseButtonDown(0) && !unkoflag).Subscribe(_ =>
        {
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
                score += 3;
                //Debug.Log("point 3!");
            }
            else if (difx <= 0.6)
            {
                score += 2;
                //Debug.Log("point 2!");
            }
            else
            {
                score += 1;
                //Debug.Log("point 1!");
            }
            scoreBoard.text = "Score: " + score.ToString();
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
            Invoke(nameof(SceneLoad), delay);
        }
    }
    void SceneLoad()
    {
        SceneManager.LoadScene("EndScreen");
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
