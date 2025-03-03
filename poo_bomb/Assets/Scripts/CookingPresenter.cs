using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Video;
using UniRx;
using UniRx.Triggers;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using TMPro;
using UnityEngine.UI;

public class CookingPresenter : MonoBehaviour
{
    //Json読み込みに必要なクラス、ていうかデータ構造
    [Serializable]
    public class InputJson
    {
        public Notes[] notes;
        public int BPM;
    }
    //これも同じ
    [Serializable]
    public class Notes
    {
        public int num;
        public int block;   //ノーツの場所(左右)
        public int LPB;
    }
    //ノーツの位置がぶち込まれた配列
    private int[] scoreNum;
    //ノーツの左右の場所がぶち込まれた配列
    private int[] scoreBlock;
    private int BPM;
    private int LPB;

    //private int gameCount;
    private List<List<int>> notesAddress;
    private List<GameObject> nowNotes;
    [SerializeField] GameObject Note;
    [SerializeField] GameObject TouchBar;
    [SerializeField] AudioSource rythmGameAudio;
    [SerializeField] private float moveSpeed = 0.19f;
    [SerializeField] private int endGameCount = 516;
    [SerializeField] private float musicOffset;
    private float[] blockPos = { -2f, -1f, 0f, 1f, 2f };
    private float[] touchBarPosX = { -2.5f, -1.5f, -0.5f, 0.5f, 1.5f, 2.5f };
    private float touchBarPosYMax = -3.0f;
    private float touchBarPosYMin = -4.9f;
    private int combo = 0;
    private float elapsedTime = 0;
    private int gameCount;
    private bool isGameStart = false;
    private float countBasedTime;
    private bool isMusicPlay;
    [SerializeField] private TextMeshProUGUI comboText;
    [SerializeField] private Button startButton;
    [SerializeField] private GameObject buttonPanel;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        MusicReading();
        startButton.OnClickAsObservable().Subscribe(x => {
            GameStart();
            buttonPanel.SetActive(false);
            Destroy(startButton);
        });
    }
    void MusicReading()
    {
        //ノーツの配置を読み込み、成型
        string inputString = Resources.Load<TextAsset>("shining_star").ToString();
        InputJson inputJson = JsonUtility.FromJson<InputJson>(inputString);

        scoreNum = new int[inputJson.notes.Length];
        scoreBlock = new int[inputJson.notes.Length];
        BPM = inputJson.BPM;
        LPB = inputJson.notes[0].LPB;
        countBasedTime = 60.0f / BPM / LPB;
        /*
        for (int i = 0; i < inputJson.notes.Length; i++)
        {
            //ノーツがある場所を入れる
            scoreNum[i] = inputJson.notes[i].num;
            //ノーツの種類を入れる(scoreBlock[i]はscoreNum[i]の種類)
            scoreBlock[i] = inputJson.notes[i].block;
        }
        */
        //Debug.Log(inputJson.notes[inputJson.notes.Length - 1].num + 1);
        notesAddress = new List<List<int>>();
        for (int i = 0; i < inputJson.notes[inputJson.notes.Length - 1].num + 1; i++) notesAddress.Add(new List<int>());
        foreach (Notes a in inputJson.notes)
        {
            notesAddress[a.num].Add(a.block);
        }
        //foreach(var a in notesAddress) foreach(var b in a) Debug.Log(b);
    }

    void GameStart()
    {
        BGMPlayer.Stop();
        gameCount = -1;
        elapsedTime = 0;
        nowNotes = new List<GameObject>();
        isGameStart = true;
        isMusicPlay = false;
    }

    // Update is called once per frame
    void Update()
    {
        comboText.text = "Combo: " + combo.ToString();

        //画面タッチ判定
        int touchCount = Input.touchCount;
        for (int i = 0; i < touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);
            if (touch.phase == TouchPhase.Began)
            {
                Vector2 touchpos = touch.position;
                Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(touchpos.x, touchpos.y, 1.0f));
                //Debug.Log("tap!!" + "positon:" + pos.x + "," + pos.y);
                if (pos.y < touchBarPosYMin || touchBarPosYMax < pos.y) continue;
                for (int j = 0; j < 5; j++)
                {
                    if (touchBarPosX[j] <= pos.x && pos.x < touchBarPosX[j + 1])
                    {
                        //Debug.Log("touch Bar" + blockPos[j]);
                        //jがブロックの位置の配列の場所を表す
                        GameObject CandidateNote = new GameObject();
                        CandidateNote.transform.position = new Vector3(0f, 10f, 0f);
                        foreach (var x in nowNotes)
                        {
                            if (x.transform.position.x == blockPos[j])
                            {
                                if (CandidateNote.transform.position.y > x.transform.position.y)
                                {
                                    CandidateNote = x;
                                }
                            }
                        }

                        //ノーツ判定＆ノーツ破壊
                        if (CandidateNote.transform.position.y < -3.5f && CandidateNote.transform.position.y > -4.5f)
                        {
                            combo++;
                            //Debug.Log("Good!");
                        }
                        Destroy(CandidateNote);
                        nowNotes.Remove(CandidateNote);
                    }
                }
            }
        }
    }
    void FixedUpdate()
    {

        if(!isGameStart) return;
        if(elapsedTime >= musicOffset && !isMusicPlay){
            rythmGameAudio.Play();
            isMusicPlay = true;
        }
        //intでキャストすれば自動的に小数点以下切り捨て
        int ngameCount = (int)(elapsedTime / countBasedTime);
        //同じカウントで複数回ノーツ生成処理が呼ばれないようにする分岐処理
        if(gameCount != ngameCount){
            gameCount = ngameCount;
            if(gameCount >= endGameCount){
                //ゲーム終了処理
                isGameStart = false;
                SceneLoader.NextScene();
                return;
            }
            //ノーツ生成判定
            foreach(int block in notesAddress[gameCount])
            {
                GameObject nnote = Instantiate(Note, new Vector3(blockPos[block], 1.75f, 0f), Quaternion.Euler(0, 0, 90));
                nowNotes.Add(nnote);
            }
        }

        //ノーツ移動処理＆破壊処理
        List<GameObject> destroyNotes = new List<GameObject>();
        foreach (var x in nowNotes)
        {
            x.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(0f, -moveSpeed);
            if (x.transform.position.y < -5.0f)
            {
                destroyNotes.Add(x);
                combo = 0;
            }
        }
        foreach (var x in destroyNotes)
        {
            nowNotes.Remove(x);
            Destroy(x);
        }
        elapsedTime += Time.fixedDeltaTime;
    }
}
