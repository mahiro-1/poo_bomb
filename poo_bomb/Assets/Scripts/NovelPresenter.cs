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

    public int storyIndex { get; private set; }
    public int textIndex { get; private set; }

    private void Awake()
    {
        storyIndex = 0;
        textIndex = 0;
        SetStoryElement(storyIndex, textIndex);
    }

    private void Start()
    {
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

        // 現在のストーリーが最後の要素に到達した場合
        if (textIndex >= storyDatas[storyIndex].stories.Count)
        {
            storyIndex++;
            textIndex = 0;

            if (storyIndex >= storyDatas.Length) // ストーリーが終了した場合
            {
                endflag = true;
            }
        }

        if (storyIndex < storyDatas.Length && textIndex < storyDatas[storyIndex].stories.Count)
        {
            storyText.text = "";
            SetStoryElement(storyIndex, textIndex);
        }
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
