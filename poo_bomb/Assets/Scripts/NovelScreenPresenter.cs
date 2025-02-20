using UnityEngine;

public class NovelScreenPresenter : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        Invoke("TestNext", 5.0f);
    }
    void TestNext(){
        SceneLoader.NextScene();
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
