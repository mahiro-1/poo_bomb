using System.Collections;
using UniRx;
using UniRx.Triggers;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UnkoDartsPresenter : MonoBehaviour
{
    private const float timeOut = 0.04f;
    private int movePoleCount = 0;
    private float movePolePerCount;
    private Vector2 CapsulePos = new Vector2(0.0f, 1.3f);
    private float MoveSpeed = 3.0f;
    [SerializeField] GameObject Capsule;
    [SerializeField] Button button;
    [SerializeField] GameObject AxisChangerR;
    [SerializeField] GameObject AxisChangerL;
    [SerializeField] GameObject Board;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        //UniRxで書いているよ。buttonがクリックされたらsubscribeの中の処理が実行される。
        button.OnClickAsObservable().Subscribe(l => CreateCapsule());

        Board.OnCollisionEnter2DAsObservable().Where(x => x.gameObject.tag == "Capsule").Subscribe(x => Destroy(x.gameObject));
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
        Board.transform.position = new Vector3(Mathf.Sin(Time.time) * MoveSpeed, -4.0f, 0);
    }
}
