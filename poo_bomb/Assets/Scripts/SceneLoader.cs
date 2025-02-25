using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;

public static class SceneLoader
{
    private static int storyIndex = 0;

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
        storyIndex = 0;
    }
    public static int GetStoryIndex(){
        return storyIndex;
    }
    public static void NextScene(){
        if(sceneNameOrder[nowIndex] == "NovelScreen") storyIndex++;
        nowIndex++;
        if(nowIndex >= sceneNameOrder.Length){
            nowIndex = 0;
            storyIndex = 0;
        }
        
        SceneManager.LoadScene(sceneNameOrder[nowIndex]);
    }
    public static void GoScoreScreen(){
        SceneManager.LoadScene("ScoreScreen");
    }
    public static void GoOptionScreen(){
        SceneManager.LoadScene("OptionScreen");
    }
    
}
