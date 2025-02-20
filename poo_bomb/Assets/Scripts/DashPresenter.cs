using UnityEngine;

public class DashPresenter : MonoBehaviour
{
    [SerializeField] private GameObject playerCamera;
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
        playerCamera.transform.position += new Vector3(0f,0f,0.02f);
    }
}
