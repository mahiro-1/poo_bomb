using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.RightArrow))
        {
            Vector2 pos;
            pos.x = 1.5f;
            pos.y = -3.5f;
            transform.position = pos;
        }
        else if(Input.GetKey(KeyCode.LeftArrow))
        {
            Vector2 pos;
            pos.x = -1.5f;
            pos.y = -3.5f;
            transform.position = pos;
        }
        else
        {
            Vector2 pos;
            pos.x = 0.0f;
            pos.y = -3.5f;
            transform.position = pos;
        }
    }
}
