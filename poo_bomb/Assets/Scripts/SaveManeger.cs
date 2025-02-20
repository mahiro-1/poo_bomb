using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
using JetBrains.Annotations;
using Unity.Android.Gradle.Manifest;
using UnityEngine;

public static class SaveManeger
{
    [Serializable] public class OnceScore
    {
        public int CookingScore;
        public int DashScore;
        public int DartsScore;
    }
    [Serializable] public class SaveData
    {
        public List<OnceScore> scores;
    }
    private static SaveData saveData;
    private static OnceScore onceScore;

    private static string fileName = "save_data.json";
    private static string filePath;

    //新しくゲームが始まったときは必ず呼ぶこと！
    public static void Init()
    {
        filePath = UnityEngine.Application.dataPath + "/" + fileName;
        saveData = new SaveData();
        onceScore = new OnceScore();
        onceScore.CookingScore = 0;
        onceScore.DashScore = 0;
        onceScore.DartsScore = 0;
        if (!File.Exists(filePath)){
            string json = "";
            StreamWriter wr = new StreamWriter(filePath, false);
            wr.WriteLine(json);
            wr.Close();
        }
    }
    public static void LoadFile(){
        StreamReader rd = new StreamReader(filePath);
        string json = rd.ReadToEnd();
        rd.Close();
        saveData = JsonUtility.FromJson<SaveData>(json);
    }
    public static void SaveFile(){
        LoadFile();
        saveData.scores.Add(onceScore);
        string json = JsonUtility.ToJson(saveData);
        StreamWriter wr = new StreamWriter(filePath, false);
        wr.WriteLine(json);
        wr.Close();
    }
    public static void SetCookingScore(int s){
        onceScore.CookingScore = s;
    }
    public static void SetDashScore(int s){
        onceScore.DashScore = s;
    }
    public static void SetDartsScore(int s){
        onceScore.DartsScore = s;
    }

    public static List<int> getLastScore(){
        LoadFile();
        int lastIndex = saveData.scores.Count - 1;
        return new List<int>(){saveData.scores[lastIndex].CookingScore, saveData.scores[lastIndex].DashScore, saveData.scores[lastIndex].DartsScore};
    }
    //セーブデータを全削除！！迂闊に使わないこと！
    public static void AllClear(){
        string json = "";
        StreamWriter wr = new StreamWriter(filePath, false);
        wr.WriteLine(json);
        wr.Close();
    }
}
