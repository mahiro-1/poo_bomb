using System.Collections;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using TMPro;
using UniRx;
using UniRx.Triggers;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DashPresenter : MonoBehaviour
{
    [SerializeField] private GameObject playerCamera;
    [SerializeField] private GameObject player;
    [SerializeField] private TextMeshProUGUI coinText;
    //[SerializeField] private GameObject moveSpace;
    private int routeFlag = 0;
    [SerializeField] private float moveSpeed = 20f;
    [SerializeField] private float turnSpeed = 10f;
    [SerializeField] private float swipeThreshold = 5f; // スワイプ判定の閾値
    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;
    [SerializeField] private float x_coordinate_from_camera = 0f;
    [SerializeField] private int maxCoin = 20;
    [SerializeField] private GameObject resultPanel;
    private int limit_flag = 0;
    private int countCoin = 0;
    private bool isMoveChange = true;
    private bool isSwipe = false;
    private int CountHitObstacle = 0;

    enum TurnDirection
    {
        Right,
        Left
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        resultPanel.SetActive(false);
        resultPanel.transform.GetChild(3).gameObject.GetComponent<Button>().OnClickAsObservable().Subscribe(x => SceneLoader.NextScene());
        player.OnTriggerEnterAsObservable().Subscribe(x =>
        {
            if (x.gameObject.tag == "coin")
            {
                countCoin++;
                Destroy(x.gameObject);
            }
            else if (x.gameObject.tag == "unko")
            {
                countCoin -= 3;
                CountHitObstacle++;
                if (countCoin < 0) countCoin = 0;
                Destroy(x.gameObject);
            }
        }).AddTo(this);
    }
    private IEnumerator Turn(TurnDirection t)
    {
        Vector3 turnVector = new Vector3();
        Vector3 maxTurn = new Vector3();
        if (t == TurnDirection.Right)
        {
            turnVector = new Vector3(0f, turnSpeed, 0f);
            maxTurn = new Vector3(0f, 0f, 0f);
        }
        else if (t == TurnDirection.Left)
        {
            turnVector = new Vector3(0f, -turnSpeed, 0f);
            maxTurn = new Vector3(0f, 270f, 0f);
        }
        float count = 0f;
        while (true)
        {
            count += turnSpeed;
            player.transform.eulerAngles += turnVector;
            playerCamera.transform.eulerAngles += turnVector;
            yield return new WaitForFixedUpdate();
            if (count >= 90f)
            {
                player.transform.eulerAngles = maxTurn;
                playerCamera.transform.eulerAngles = maxTurn;
                yield break;
            }
        }
    }
    private void GameEnd(){
        SaveManeger.SetDashScore(GetScore());
        BGMPlayer.Play();
        resultPanel.SetActive(true);
        resultPanel.transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text 
            = "取れたコイン	："+ countCoin.ToString() +"\n障害物	："+ CountHitObstacle.ToString() +"\n--------------------------\n合計得点	：" + GetScore().ToString();
    }
    int GetScore(){
        float score = countCoin * (1000f / maxCoin);
        return (int)score;
    }
    
    void FixedUpdate()
    {
        if(routeFlag == 3){
            GameEnd();
        }
        ProcessSwipe();
        if (isMoveChange)
        {
            Vector3 moveVector = new Vector3(0f, 0f, 0f);
            if (routeFlag == 0) moveVector = new Vector3(0f, 0f, moveSpeed);
            else if (routeFlag == 1) moveVector = new Vector3(-moveSpeed, 0f, 0f);
            else if (routeFlag == 2) moveVector = new Vector3(0f, 0f, moveSpeed);
            else if (routeFlag == 3) moveVector = new Vector3(0f, 0f, 0f);
            player.GetComponent<Rigidbody>().linearVelocity = moveVector;
            isMoveChange = false;
        }


        Vector3 pos = player.transform.position;
        if (routeFlag == 0 && pos.z >= -147f)
        {
            routeFlag++;
            limit_flag = 0;
            pos.z = -147f;
            isMoveChange = true;
            StartCoroutine(Turn(TurnDirection.Left));
        }
        if (routeFlag == 1 && pos.x <= 160f)
        {
            routeFlag++;
            limit_flag = 0;
            pos.x = 160f;
            isMoveChange = true;
            StartCoroutine(Turn(TurnDirection.Right));
        }
        if (routeFlag == 2 && pos.z >= 280f)
        {
            routeFlag++;
            limit_flag = 0;
            pos.z = 280f;
            isMoveChange = true;
        }
        player.transform.position = pos;
        playerCamera.transform.position = player.transform.position;
    }
    void LateUpdate()
    {

    }
    void Update()
    {
        coinText.text = "Coin :" + countCoin.ToString();
        DetectSwipe();
    }

    private void DetectSwipe()
    {
        if (Input.GetMouseButtonDown(0)) // タッチ開始
        {
            startTouchPosition = Input.mousePosition;
        }
        if (Input.GetMouseButtonUp(0)) // タッチ終了
        {
            endTouchPosition = Input.mousePosition;
            isSwipe = true;
        }
    }

    private void ProcessSwipe()
    {
        if (isSwipe)
        {
            Vector3 swipeVector = endTouchPosition - startTouchPosition;

            if (swipeVector.magnitude < swipeThreshold) return; // 閾値以下なら無視

            Vector3 newPosition = player.transform.position;

            if (Mathf.Abs(swipeVector.x) > Mathf.Abs(swipeVector.y))
            {
                if (swipeVector.x > 0 && limit_flag < 1) // 右スワイプ
                {
                    if (routeFlag == 0) newPosition.x += 5f;
                    else if (routeFlag == 1) newPosition.z += 5f;
                    else if (routeFlag == 2) newPosition.x += 5f;
                    limit_flag += 1;
                }
                if (swipeVector.x < 0 && limit_flag > -1)// 左スワイプ
                {
                    if (routeFlag == 0) newPosition.x -= 5f;
                    else if (routeFlag == 1) newPosition.z -= 5f;
                    else if (routeFlag == 2) newPosition.x -= 5f;
                    limit_flag -= 1;
                }
            }
            else
            {
                return;
            }

            player.transform.position = newPosition;
            isSwipe = false;
        }

    }
}

