using System;
using UnityEngine;

public class BGMPlayer : MonoBehaviour
{
    private static BGMPlayer instance;
    private static AudioSource bgm;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if(instance == null){
            instance = this;
            DontDestroyOnLoad(gameObject);
            bgm = this.gameObject.GetComponent<AudioSource>();        }
        else{
            Destroy(gameObject);
        }
    }
    public static void Play(){
        bgm.Play();
    }
    public static void Stop(){
        bgm.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
