using System.Collections;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UniRx;
using UniRx.Triggers;
using Unity.VisualScripting;
using UnityEngine;

public class DashPresenter : MonoBehaviour
{
    [SerializeField] private GameObject playerCamera;
    [SerializeField] private GameObject player;
    //[SerializeField] private GameObject moveSpace;
    private int routeFlag = 0;
    [SerializeField] private float moveSpeed = 20f;
    [SerializeField] private float turnSpeed = 10f;
    [SerializeField] private float swipeThreshold = 5f; // スワイプ判定の閾値
    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;
    [SerializeField] private float x_coordinate_from_camera = 0f;
    private int limit_flag = 0;
    private int countCoin = 0;
    private bool isMoveChange = true;

    enum TurnDirection {
        Right,
        Left
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        player.OnTriggerEnterAsObservable().Where(x => x.gameObject.tag == "coin").Subscribe(x =>
        {
            countCoin++;
            Debug.Log("coin get");
        });
    }
    private IEnumerator Turn(TurnDirection t){
        Vector3 turnVector = new Vector3();
        Vector3 maxTurn = new Vector3();
        if(t == TurnDirection.Right){
            turnVector = new Vector3(0f, turnSpeed, 0f);
            maxTurn = new Vector3(0f, 0f, 0f);
        }
        else if(t == TurnDirection.Left){
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
    /*
    private IEnumerator TurnLeft()
    {
        while (true)
        {
            moveSpace.transform.eulerAngles += new Vector3(0f, -turnSpeed, 0f);
            playerCamera.transform.eulerAngles += new Vector3(0f, -turnSpeed, 0f);
            yield return new WaitForFixedUpdate();
            if (moveSpace.transform.eulerAngles.y <= 270f)
            {
                moveSpace.transform.eulerAngles = new Vector3(0f, -90f, 0f);
                playerCamera.transform.eulerAngles = new Vector3(0f, -90f, 0f);
                yield break;
            }
        }
    }
    private IEnumerator TurnRight()
    {
        while (true)
        {
            moveSpace.transform.eulerAngles += new Vector3(0f, turnSpeed, 0f);
            //playerCamera.transform.eulerAngles += new Vector3(0f, turnSpeed, 0f);
            if (moveSpace.transform.eulerAngles.y <= turnSpeed)
            {
                moveSpace.transform.eulerAngles = new Vector3(0f, 0f, 0f);
                //playerCamera.transform.eulerAngles = new Vector3(0f,0f,0f);
                yield break;
            }
            yield return new WaitForFixedUpdate();
        }
    }
    */
    void FixedUpdate()
    {
        DetectSwipe();
        if(isMoveChange){
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
    }
    void LateUpdate()
    {
        playerCamera.transform.position = player.transform.position;
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
            ProcessSwipe();
        }
    }

    private void ProcessSwipe()
    {
        Vector2 swipeVector = endTouchPosition - startTouchPosition;

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
    }
}

