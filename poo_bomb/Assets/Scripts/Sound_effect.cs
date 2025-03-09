using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    public AudioClip soundClip; // 再生する音声クリップ
    private AudioSource audioSource;

    void Start()
    {
        // AudioSourceを取得または追加
        audioSource = gameObject.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.clip = soundClip;
    }

    // 呼び出されたときに音を鳴らすメソッド
    public void PlaySound()
    {
        if (soundClip != null)
        {
            audioSource.Play();
        }
    }
}