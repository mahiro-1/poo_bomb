using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using TMPro;
using UnityEngine.UI;
using System.Collections;


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
    private float score = 0;
    private float maxScore = 1000f;
    private float elapsedTime = 0;
    private int gameCount;
    private bool isGameStart = false;
    private float countBasedTime;
    private bool isMusicPlay;
    private float scorePerNotes;
    [SerializeField] private TextMeshProUGUI comboText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Button startButton;
    [SerializeField] private GameObject buttonPanel;
    [SerializeField] private TextMeshProUGUI jadgeText;
    [SerializeField] private Canvas canvas;
    [SerializeField] private GameObject resultPanel;
    enum Jadges : int{
        Perfect,
        Good,
        Miss
    }
    private int[] resultColor = {0, 0, 0};
    List<TextMeshProUGUI> jadgeTexts;
    private bool isEnd = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        resultPanel.SetActive(false);
        resultPanel.transform.GetChild(2).gameObject.GetComponent<Button>().OnClickAsObservable().Subscribe(x => SceneLoader.NextScene());
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
        scorePerNotes = maxScore / (float)inputJson.notes.Length;
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
        jadgeTexts = new List<TextMeshProUGUI>();
        isGameStart = true;
        isMusicPlay = false;
    }
    IEnumerator GameEnd(){
        yield return new WaitForSeconds(3.0f);
        rythmGameAudio.Stop();
        BGMPlayer.Play();
        isGameStart = false;
        resultPanel.SetActive(true);
        resultPanel.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text 
            = "Perfect	："+ resultColor[(int)Jadges.Perfect].ToString() +"\nGood	："+ resultColor[(int)Jadges.Good].ToString() +"\nMiss	："+ resultColor[(int)Jadges.Miss].ToString() +"\n--------------------------\n合計得点	：" + ((int)score).ToString();
        SaveManeger.SetCookingScore((int)score);
    }
    void ShowJadgeText(Jadges j){
        TextMeshProUGUI ntext = Instantiate<TextMeshProUGUI>(jadgeText,new Vector3(0f,0f,0f),Quaternion.identity);
        ntext.transform.SetParent(canvas.transform, false);
        ntext.transform.position = new Vector3(2.0f,-1.0f,1.0f);
        if(j == Jadges.Perfect){
            ntext.text = "Perfect!";
            ntext.color = new Color(44f/256f,180f/256f,173f/256f);
        }
        else if(j == Jadges.Good){
            ntext.text = "Good";
            ntext.color = new Color(237f/256f,222f/256f,123f/256f);
        }
        else if(j == Jadges.Miss){
            ntext.text = "Miss...";
            ntext.color = new Color(0f/256f,77f/256f,37f/256f);
        }
        jadgeTexts.Add(ntext);
    }
    void MoveJadgeText(){
        List<TextMeshProUGUI> destroyText = new List<TextMeshProUGUI>();
        foreach(TextMeshProUGUI ntext in jadgeTexts){
            ntext.transform.position += new Vector3(0f,0.05f,0f);
            ntext.color += new Color(0f,0f,0f,-0.07f);
            if(ntext.color.a <= 0f){
                destroyText.Add(ntext);
            }
        }
        foreach(TextMeshProUGUI ntext in destroyText){
            Destroy(ntext.gameObject);
            jadgeTexts.Remove(ntext);
        }
    }

    // Update is called once per frame
    void Update()
    {
        comboText.text = "Combo: " + combo.ToString();
        scoreText.text = "Score: " + Math.Floor(score).ToString();

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
                        //nowNotesの中で一番バーに近いやつを見つける
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
                        float y = CandidateNote.transform.position.y;
                        if(y < -4.5f){
                            //Good
                            ShowJadgeText(Jadges.Good);
                            Destroy(CandidateNote);
                            nowNotes.Remove(CandidateNote);
                            combo++;
                            score += scorePerNotes * 0.5f;
                            resultColor[(int)Jadges.Good]++;
                        }
                        else if(y < -4f){
                            //Perfect
                            ShowJadgeText(Jadges.Perfect);
                            Destroy(CandidateNote);
                            nowNotes.Remove(CandidateNote);
                            combo++;
                            score += scorePerNotes * 1.0f;
                            resultColor[(int)Jadges.Perfect]++;
                        }
                        else if(y < -3.5f){
                            //Good
                            ShowJadgeText(Jadges.Good);
                            Destroy(CandidateNote);
                            nowNotes.Remove(CandidateNote);
                            combo++;
                            score += scorePerNotes * 0.5f;
                            resultColor[(int)Jadges.Good]++;
                        }
                        else if(y < -3f){
                            //Miss
                            ShowJadgeText(Jadges.Miss);
                            Destroy(CandidateNote);
                            nowNotes.Remove(CandidateNote);
                            combo = 0;
                            resultColor[(int)Jadges.Miss]++;
                        }

                    }
                }
            }
        }
    }
    void FixedUpdate()
    {
        if(!isGameStart) return;
        if(isEnd) return;
        MoveJadgeText();
        if(elapsedTime >= musicOffset && !isMusicPlay){
            rythmGameAudio.Play();
            isMusicPlay = true;
        }
        //intでキャストすれば自動的に小数点以下切り捨て
        int ngameCount = (int)(elapsedTime / countBasedTime);
        //同じカウントで複数回ノーツ生成処理が呼ばれないようにする分岐処理
        if(gameCount != ngameCount){
            gameCount = ngameCount;
            if(gameCount >= endGameCount && !isEnd){
                //ゲーム終了処理
                isEnd = true;
                StartCoroutine(GameEnd());
                return;
            }
            //ノーツ生成判定
            foreach(int block in notesAddress[gameCount])
            {
                GameObject nnote = Instantiate(Note, new Vector3(blockPos[block], 1.75f, 0f), Quaternion.Euler(0, 0, 90));
                nnote.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(0f, -moveSpeed);
                nowNotes.Add(nnote);
            }
        }

        //ノーツ移動処理＆破壊処理
        List<GameObject> destroyNotes = new List<GameObject>();
        foreach (var x in nowNotes)
        {
            if (x.transform.position.y < -5.0f)
            {
                //Miss
                ShowJadgeText(Jadges.Miss);
                destroyNotes.Add(x);
                combo = 0;
                resultColor[(int)Jadges.Miss]++;
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
