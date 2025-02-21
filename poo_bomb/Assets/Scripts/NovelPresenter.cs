using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UniRx;
using UniRx.Triggers;
using UnityEngine.SceneManagement;
using System;
public class Presenter : MonoBehaviour
{
    [SerializeField] private StoryData[] storyDatas;
    [SerializeField] private Image background;
    [SerializeField] private Image characterImage;
    [SerializeField] private TextMeshProUGUI storyText;
    [SerializeField] private TextMeshProUGUI characterName;
    //ストーリーのエレメント配列番号が必要なのでプロパティを
    public int storyIndex { get; private set; }
    public int textIndex { get; private set; }
    private void Awake()
    {
        SetStoryElement(storyIndex, textIndex);
    }

    private void Update()
    {
        Observable.EveryUpdate().Where(_ => Input.GetMouseButtonDown(0)).Subscribe(_ =>
        {
            textIndex++;
            storyText.text = "";
            SetStoryElement(storyIndex, textIndex);
        });

    }
    private void SetStoryElement(int _storyIndex, int _textIndex)
    {
        var storyElement = storyDatas[_storyIndex].stories[_textIndex];
        background.sprite = storyElement.Background;
        characterImage.sprite = storyElement.CharacterImage;
        storyText.text = storyElement.StoryText;
        characterName.text = storyElement.CharacterName;
    }
}
