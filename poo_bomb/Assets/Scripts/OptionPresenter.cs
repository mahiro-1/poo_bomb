using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class OptionPresenter : MonoBehaviour
{
    [SerializeField] private Button returnButton;
    [SerializeField] private Slider offsetSlider;
    [SerializeField] private TextMeshProUGUI sliderValue;
    private float offset;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        offset = SaveManeger.GetOffset();
        Debug.Log(offset);
        offsetSlider.value = offset;
        ShowSliderValue();
        offsetSlider.OnValueChangedAsObservable().Subscribe(x => {
            offset = offsetSlider.value;
            ShowSliderValue();
        });
        returnButton.OnClickAsObservable().Subscribe(x => {
            SaveManeger.SetOffset(offset);
            SaveManeger.SaveSetting();
            SceneLoader.ReturnStartScreen();
        });
    }
    void ShowSliderValue(){
        sliderValue.text = offset.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
