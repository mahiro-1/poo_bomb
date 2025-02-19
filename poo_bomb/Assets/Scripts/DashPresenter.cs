using UnityEngine;

public class DashPresenter : MonoBehaviour
{
    [SerializeField] private GameObject camera;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        camera.transform.position += new Vector3(0f,0f,0.02f);
    }
}
