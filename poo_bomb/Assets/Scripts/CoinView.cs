using UnityEngine;

public class CoinView : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.gameObject.GetComponent<Rigidbody>().angularVelocity = new Vector3(0f, 8f, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
