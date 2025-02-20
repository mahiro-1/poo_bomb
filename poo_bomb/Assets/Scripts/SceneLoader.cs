using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;

public static class SceneLoader
{
    private static string[] sceneNameOrder = {
        "StartScreen",
        "NovelScreen",
        "Cooking",
        "NovelScreen",
        "Dash",
        "NovelScreen",
        "UnkoDarts",
        "NovelScreen",
        "EndScreen"
    };
    private static int nowIndex;

    public static void Init(){
        nowIndex = 0;
    }
    public static void NextScene(){
        nowIndex++;
        if(nowIndex >= sceneNameOrder.Length) nowIndex = 0;
        SceneManager.LoadScene(sceneNameOrder[nowIndex]);
    }
    public static void GoScoreScreen(){
        SceneManager.LoadScene("ScoreScreen");
    }
    public static void GoOptionScreen(){
        SceneManager.LoadScene("OptionScreen");
    }
    
}
