using System;
using UnityEngine;

public class CookingPresenter : MonoBehaviour
{
    [Serializable] public class InputJson
    {
        public Notes[] notes;
        public int BPM;
    }
    [Serializable] public class Notes
    {
        public int num;
        public int block;
        public int LPB;
    }
    private int[] scoreNum;
    private int[] scoreBlock;
    private int BPM;
    private int LPB;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        MusicReading();
    }
    void MusicReading()
    {
        string inputString = Resources.Load<TextAsset>("testbgm").ToString();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
