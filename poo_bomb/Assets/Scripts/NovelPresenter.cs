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
    [SerializeField] private SoundPlayer soundPlayer;
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
            .Subscribe(_ =>
            {
                soundPlayer.PlaySound();
                Observable.Timer(System.TimeSpan.FromSeconds(0.5)) // 0.5秒待つ
            .Subscribe(__ => OnNextText());
            })
            .AddTo(this);
    }
    private void OnNextText()
    {
        endflag = true;
        textIndex++;
        storyText.text = "";
        if (textIndex >= storyDatas[storyIndex].stories.Count)
        {
            SceneLoader.NextScene();
        }
        else
        {
            SetStoryElement(storyIndex, textIndex);
        }
        Observable.Timer(System.TimeSpan.FromSeconds(2.0)) // 2.0秒待つ
        .Subscribe(__ => endflag = false);
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
