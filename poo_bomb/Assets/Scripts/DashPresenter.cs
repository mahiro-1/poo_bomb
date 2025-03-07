using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class DashPresenter : MonoBehaviour
{
    [SerializeField] private GameObject playerCamera;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject moveSpace;
    private int routeFlag = 0;
    [SerializeField] private float moveSpeed = 20f;
    [SerializeField] private float turnSpeed = 10f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }
    private IEnumerator TurnLeft()
    {
        while (true)
        {
            moveSpace.transform.eulerAngles += new Vector3(0f, -turnSpeed, 0f);
            //playerCamera.transform.eulerAngles += new Vector3(0f, -turnSpeed, 0f);
            yield return new WaitForFixedUpdate();
            if (moveSpace.transform.eulerAngles.y <= 270f)
            {
                moveSpace.transform.eulerAngles = new Vector3(0f, -90f, 0f);
                //playerCamera.transform.eulerAngles = new Vector3(0f, -90f, 0f);
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
    void FixedUpdate()
    {
        if (routeFlag == 0) moveSpace.transform.position += new Vector3(0f, 0f, moveSpeed);
        else if (routeFlag == 1) moveSpace.transform.position += new Vector3(-moveSpeed, 0f, 0f);
        else if (routeFlag == 2) moveSpace.transform.position += new Vector3(0f, 0f, moveSpeed);
        else if (routeFlag == 3) moveSpace.transform.position += new Vector3(0f, 0f, 0f);

        Vector3 pos = moveSpace.transform.position;
        if (routeFlag == 0 && pos.z >= -147f)
        {
            routeFlag++;
            pos.z = -147f;
            StartCoroutine(TurnLeft());
        }
        if (routeFlag == 1 && pos.x <= 160f)
        {
            routeFlag++;
            pos.x = 160f;
            StartCoroutine(TurnRight());
        }
        if (routeFlag == 2 && pos.z >= 280f)
        {
            routeFlag++;
            pos.z = 280f;
        }
        moveSpace.transform.position = pos;
    }
    void LateUpdate()
    {
        //playerCamera.transform.position = new Vector3(0f, 14.9f, -10) + player.transform.position;
    }
}
