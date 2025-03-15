using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public static class SaveManeger
{
    [Serializable]
    public class OnceScore
    {
        public int CookingScore;
        public int DashScore;
        public int DartsScore;
    }
    [Serializable]
    public class SaveData
    {
        public List<OnceScore> scores;
    }
    [Serializable] public class Setting
    {
        public float offset; //Cookingのノーツと音楽のオフセットms
    }
    private static Setting setting;
    private static SaveData saveData;
    private static OnceScore onceScore;

    private static string fileName = "save_data.json";
    public static string filePath;
    private static string settingFileName = "setting.json";
    public static string settingFilePath;

    //新しくゲームが始まったときは必ず呼ぶこと！
    public static void Init()
    {
        filePath = Application.persistentDataPath + "/" + fileName;
        Debug.Log(filePath);
        saveData = new SaveData();
        onceScore = new OnceScore();
        onceScore.CookingScore = 0;
        onceScore.DashScore = 0;
        onceScore.DartsScore = 0;
        
        setting = new Setting();
        settingFilePath = Application.persistentDataPath + "/" + settingFileName;
        LoadSetting();
    }
    public static void LoadSetting(){
        if (!File.Exists(settingFilePath))
        {
            StreamWriter wr = new StreamWriter(settingFilePath, false);
            wr.WriteLine("{\"offset\":1}");
            wr.Close();
        }
        StreamReader rd = new StreamReader(settingFilePath);
        string json = rd.ReadToEnd();
        rd.Close();
        Debug.Log(json);
        setting = JsonUtility.FromJson<Setting>(json);
    }
    public static float GetOffset(){
        return setting.offset;
    }
    public static void SetOffset(float o){
        setting.offset = o;
    }
    public static void SaveSetting(){
        string json = JsonUtility.ToJson(setting);
        StreamWriter wr = new StreamWriter(settingFilePath, false);
        wr.WriteLine(json);
        wr.Close();
    }
    public static void LoadFile()
    {
        if (!File.Exists(filePath))
        {
            StreamWriter wr = new StreamWriter(filePath, false);
            wr.WriteLine("{\"scores\":[{\"CookingScore\":0,\"DashScore\":0,\"DartsScore\":0}]}");
            wr.Close();
        }
        StreamReader rd = new StreamReader(filePath);
        string json = rd.ReadToEnd();
        rd.Close();
        saveData = JsonUtility.FromJson<SaveData>(json);
    }
    public static void SaveFile()
    {
        LoadFile();
        saveData.scores.Add(onceScore);
        string json = JsonUtility.ToJson(saveData);
        StreamWriter wr = new StreamWriter(filePath, false);
        wr.WriteLine(json);
        wr.Close();
    }
    public static void SetCookingScore(int s)
    {
        onceScore.CookingScore = s;
    }
    public static void SetDashScore(int s)
    {
        onceScore.DashScore = s;
    }
    public static void SetDartsScore(int s)
    {
        onceScore.DartsScore = s;
    }
    public static int GetCookingScore()
    {
        return onceScore.CookingScore;
    }
    public static int GetDashScore()
    {
        return onceScore.DashScore;
    }
    public static int GetDartsScore()
    {
        return onceScore.DartsScore;
    }

    public static List<int> getLastScore()
    {
        LoadFile();
        int lastIndex = saveData.scores.Count - 1;
        return new List<int>() { saveData.scores[lastIndex].CookingScore, saveData.scores[lastIndex].DashScore, saveData.scores[lastIndex].DartsScore };
    }
    public static List<OnceScore> getRanking(){
        LoadFile();
        List<Tuple<int, int>> sortScore = new List<Tuple<int, int>>();
        for(int i = 0; i < saveData.scores.Count; i++){
            sortScore.Add(new Tuple<int, int>(saveData.scores[i].CookingScore + saveData.scores[i].DashScore + saveData.scores[i].DartsScore, i));
        }
        Tuple<int, int>[] sorted = sortScore.OrderBy(x => x.Item1).ToArray();
        List<OnceScore> rscore = new List<OnceScore>();
        int maxindex = Math.Min(10, sorted.Length);
        for(int i = maxindex - 1; i >= 0; i--){
            rscore.Add(saveData.scores[sorted[i].Item2]);
        }
        return rscore;
    }
    //セーブデータを全削除！！迂闊に使わないこと！
    public static void AllClear()
    {
        string json = "{\"scores\":[{\"CookingScore\":0,\"DashScore\":0,\"DartsScore\":0}]}";
        StreamWriter wr = new StreamWriter(filePath, false);
        wr.WriteLine(json);
        wr.Close();
        LoadFile();
    }
}
