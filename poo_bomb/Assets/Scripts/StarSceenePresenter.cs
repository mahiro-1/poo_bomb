using System.Collections;
using TMPro;
using UniRx;
using UniRx.Triggers;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StarSceenePresenter : MonoBehaviour
{
    [SerializeField] public Button start_button;
    [SerializeField] public Button option_button;
    [SerializeField] public Button score_button;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        start_button.OnClickAsObservable().Subscribe(x => Debug.Log("START！"));
        option_button.OnClickAsObservable().Subscribe(x => Debug.Log("OPTION！"));
        score_button.OnClickAsObservable().Subscribe(x => Debug.Log("SCORE！"));
    }

    // Update is called once per frame
    void Update()
    {
    }
}
