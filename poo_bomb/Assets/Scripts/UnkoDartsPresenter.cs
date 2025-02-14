using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class UnkoDartsPresenter : MonoBehaviour
{
    [SerializeField] Button button;
    [SerializeField] private GameObject Board;

    [SerializeField] private GameObject Unko;



    private float MoveSpeed = 3.0f;
    void FixedUpdate()
    {
        Board.transform.position = new Vector3(Mathf.Sin(Time.time) * MoveSpeed, -4.3f, 0);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        //UniRxで書いているよ。buttonがクリックされたらsubscribeの中の処理が実行される。
        button.OnClickAsObservable().Subscribe(l => Hallo());
    }

    void Hallo()
    {
        Debug.Log("Hallo!");
    }
    // Update is called once per frame
    void Update()
    {

    }
}
