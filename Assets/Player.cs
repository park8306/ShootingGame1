﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Update is called once per frame
    public float speed;
    public bool isTouchTop; // 경계에 닿았는지 안닿았는지 확인하기 위한 변수
    public bool isTouchBottom;
    public bool isTouchRight;
    public bool isTouchLeft;

    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal"); // -1, 0, 1 딱 3가지 값만 들어옴
        if ((isTouchRight && h == 1) || (isTouchLeft && h == -1))
            h = 0;

        float v = Input.GetAxisRaw("Vertical");
        if ((isTouchTop && v == 1) || (isTouchBottom && v == -1))
            v = 0;
        Vector3 curPos = transform.position; // 현재 플레이어의 위치
        Vector3 nextPos = new Vector3(h, v, 0) * speed * Time.deltaTime; // 다음에 갈 위치 * 속도

        transform.position = curPos + nextPos;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Border")) // CompareTag를 이용하여 비교하는 것이 성능향상에 좋음
        {
            switch (collision.gameObject.name) // 경계영역의 이름을 통해 비교
            {
                case "Top":
                    isTouchTop = true;
                    break;
                case "Bottom":
                    isTouchBottom = true;
                    break;
                case "Right":
                    isTouchRight = true;
                    break;
                case "Left":
                    isTouchLeft = true;
                    break;
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Border")) // CompareTag를 이용하여 비교하는 것이 성능향상에 좋음
        {
            switch (collision.gameObject.name)
            {
                case "Top":
                    isTouchTop = false;
                    break;
                case "Bottom":
                    isTouchBottom = false;
                    break;
                case "Right":
                    isTouchRight = false;
                    break;
                case "Left":
                    isTouchLeft = false;
                    break;
            }
        }
    }
}
