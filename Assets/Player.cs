using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Update is called once per frame
    public float speed;
    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal"); // -1, 0, 1 딱 3가지 값만 들어옴
        float v = Input.GetAxisRaw("Vertical");
        Vector3 curPos = transform.position; // 현재 플레이어의 위치
        Vector3 nextPos = new Vector3(h, v, 0) * speed * Time.deltaTime; // 다음에 갈 위치 * 속도

        transform.position = curPos + nextPos;
    }
}
