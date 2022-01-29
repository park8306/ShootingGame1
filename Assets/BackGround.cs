using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGround : MonoBehaviour
{
    public int speed;
    // Update is called once per frame
    void Update()
    {
        Vector3 pos;
        pos = transform.position;
        if(pos.y <= -10)
        {
            pos.y = Camera.main.orthographicSize * 2;
            transform.position = pos;
        }
        else
        {
            pos.y = pos.y - Time.deltaTime * speed;
            transform.position = pos;
        }
    }
}
