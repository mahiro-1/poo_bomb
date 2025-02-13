using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class UnkoDartsPresenter : MonoBehaviour
{
    [SerializeField] Button button;
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
