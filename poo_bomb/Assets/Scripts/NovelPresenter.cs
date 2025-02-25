using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UnityEngine.SceneManagement;
using System;

public class Presenter : MonoBehaviour
{
    [SerializeField] private StoryData[] storyDatas;//ストーリー全体のデータ（配列で管理）
    [SerializeField] private Image background;
    [SerializeField] private Image characterImage;
    [SerializeField] private TextMeshProUGUI storyText;
    [SerializeField] private TextMeshProUGUI characterName;
    private Boolean endflag = false;
    public int textIndex { get; private set; }
    private int storyIndex;

    private void Awake()
    {
        textIndex = 0;
        storyIndex = SceneLoader.GetStoryIndex();
        SetStoryElement(storyIndex, textIndex);
        // 一度だけクリックイベントを購読
        Observable.EveryUpdate()
            .Where(_ => Input.GetMouseButtonDown(0) && endflag == false)
            .ThrottleFirst(TimeSpan.FromMilliseconds(200)) // 0.2秒以内の連続クリックを無視し、意図しない二重クリックを防ぐ
            .Subscribe(_ => OnNextText())
            .AddTo(this);
    }
    private void OnNextText()
    {
        textIndex++;
        if (textIndex >= storyDatas[storyIndex].stories.Count)
        {
            SceneLoader.NextScene();
        }
        storyText.text = "";
        SetStoryElement(storyIndex, textIndex);
    }

    private void SetStoryElement(int _storyIndex, int _textIndex)
    {
        var storyElement = storyDatas[_storyIndex];
        background.sprite = storyElement.stories[_textIndex].Background;
        characterImage.sprite = storyElement.stories[_textIndex].CharacterImage;
        storyText.text = storyElement.stories[_textIndex].StoryText;
        characterName.text = storyElement.stories[_textIndex].CharacterName;
    }
}
