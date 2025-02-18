using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Video;
using UniRx;
using UniRx.Triggers;

public class CookingPresenter : MonoBehaviour
{
    //Json読み込みに必要なクラス、ていうかデータ構造
    [Serializable] public class InputJson
    {
        public Notes[] notes;
        public int BPM;
    }
    //これも同じ
    [Serializable] public class Notes
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

    private int gameCount;
    private List<List<int>> notesAddress;
    private Queue<GameObject> nowNotes;
    [SerializeField] GameObject Note;
    [SerializeField] GameObject DestroyArea;
    private float moveSpeed = 0.18f;
    private float timeOut = 0.03f;
    private float[] blockPos = {-3f,-1.5f,0f,1.5f,3f};
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        DestroyArea.OnTriggerEnter2DAsObservable().Subscribe(x => {
            Destroy(x.gameObject);
            nowNotes.Dequeue();
        });
        
        MusicReading();
        //音ゲーを開始する関数（後でボタンを押されたときに始めるとかにするで）
        GameStart();
    }
    void MusicReading()
    {
        //ノーツの配置を読み込み、成型
        string inputString = Resources.Load<TextAsset>("testbgm").ToString();
        InputJson inputJson = JsonUtility.FromJson<InputJson>(inputString);

        scoreNum = new int[inputJson.notes.Length];
        scoreBlock = new int[inputJson.notes.Length];
        BPM = inputJson.BPM;
        LPB = inputJson.notes[0].LPB;
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
        for(int i = 0; i < inputJson.notes[inputJson.notes.Length - 1].num + 1; i++) notesAddress.Add(new List<int>());
        foreach(Notes a in inputJson.notes){
            notesAddress[a.num].Add(a.block);
        }
        //foreach(var a in notesAddress) foreach(var b in a) Debug.Log(b);
    }

    void GameStart(){
        gameCount = 0;
        nowNotes = new Queue<GameObject>();
        //ノーツを生成する関数を繰り返し呼び出すやつ。繰り返す秒数は音楽のLPBに合わせる
        InvokeRepeating("CreateNotes", 0f, 0.05f);
        StartCoroutine(MoveNotes());
    }

    //ロードしたデータをもとにノーツを生成
    void CreateNotes(){
        if(notesAddress.Count <= gameCount) return;
        foreach(int block in notesAddress[gameCount]){
            GameObject nnote = Instantiate(Note, new Vector3(blockPos[block],1.2f,0f), Quaternion.Euler(0,0,90));
            nowNotes.Enqueue(nnote);

        }
        gameCount++;
    }
    IEnumerator MoveNotes()
    {
        while (true)
        {
            int destroyCount = 0;
            //timeOutの時間でループする
            foreach(var x in nowNotes){
                x.transform.position += new Vector3(0f, -moveSpeed, 0f);
                if(x.transform.position.y < -5.0f){
                    destroyCount++;
                }
            }
            for(int i = 0; i < destroyCount; i++) Destroy(nowNotes.Dequeue());
            yield return new WaitForSeconds(timeOut);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
