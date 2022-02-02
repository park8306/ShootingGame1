using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Update is called once per frame
    public float speed;
    public int power;
    public int maxPower;
    public int boom;
    public int maxBoom;
    public float maxShotDelay;
    public float curShotDelay;

    public int life;
    public int score;

    public bool isTouchTop; // 경계에 닿았는지 안닿았는지 확인하기 위한 변수
    public bool isTouchBottom;
    public bool isTouchRight;
    public bool isTouchLeft;

    public GameObject PlayerBulletA;
    public GameObject PlayerBulletB;
    public GameObject BoomEffect;

    public GameManager gameManager;
    public ObjectManager objectManager;
    public bool isHit;
    public bool isBoomTime;

    Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        Move();
        Fire();
        Boom();
        Reload();
    }

    private void Boom()
    {
        if (!Input.GetButton("Fire2"))
            return;
        if (isBoomTime)
            return;

        if (boom == 0)
            return;

        boom--;
        gameManager.UpdateBoomIcon(boom);
        isBoomTime = true;
        BoomEffect.SetActive(true);
        StartCoroutine(OffBoomEffect()); // 시간 지나면 폭탄 이펙트 사라지게 하기

        // 적 사라지게 하기
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        for (int index = 0; index < enemies.Length; index++)
        {
            Enemy enemyLogic = enemies[index].GetComponent<Enemy>();
            enemyLogic.OnHit(1000);
        }
        // 총알 사라지게 하기
        GameObject[] bullets = GameObject.FindGameObjectsWithTag("EnemyBullet");
        for (int index = 0; index < bullets.Length; index++)
        {
            bullets[index].SetActive(false);
        }
    }

    private void Reload()
    {
        curShotDelay += Time.deltaTime;
    }

    private void Fire()
    {
        if (!Input.GetButton("Fire1")) // fire1 버튼을 누르지 않으면 리턴! 실행 시키지 않음
            return;
        if (curShotDelay < maxShotDelay)
            return;

        switch (power)
        {
            case 1:
                // Power One
                GameObject bullet = objectManager.MakeObj("BulletPlayerA"); // 총알 생성하는 코드
                bullet.transform.position = transform.position;
                Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>(); // 총알의 rigidbody2D 값을 가져옴
                rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);   // addForce를 통해 총알에 힘을 부여
                break;
            case 2:
                GameObject bulletR = objectManager.MakeObj("BulletPlayerA"); // 총알 생성하는 코드
                GameObject bulletL = objectManager.MakeObj("BulletPlayerA"); // 총알 생성하는 코드
                bulletR.transform.position = transform.position + Vector3.right * 0.2f;
                bulletL.transform.position = transform.position + Vector3.left * 0.2f;
                Rigidbody2D rigidR = bulletR.GetComponent<Rigidbody2D>(); // 총알의 rigidbody2D 값을 가져옴
                Rigidbody2D rigidL = bulletL.GetComponent<Rigidbody2D>(); // 총알의 rigidbody2D 값을 가져옴
                rigidR.AddForce(Vector2.up * 10, ForceMode2D.Impulse);   // addForce를 통해 총알에 힘을 부여
                rigidL.AddForce(Vector2.up * 10, ForceMode2D.Impulse);   // addForce를 통해 총알에 힘을 부여
                break;
            case 3:
                GameObject bulletRR = objectManager.MakeObj("BulletPlayerA"); 
                GameObject bulletCC = objectManager.MakeObj("BulletPlayerB"); 
                GameObject bulletLL = objectManager.MakeObj("BulletPlayerA"); 
                bulletRR.transform.position = transform.position + Vector3.right * 0.35f;
                bulletCC.transform.position = transform.position;
                bulletLL.transform.position = transform.position + Vector3.left * 0.35f;
                Rigidbody2D rigidRR = bulletRR.GetComponent<Rigidbody2D>(); // 총알의 rigidbody2D 값을 가져옴
                Rigidbody2D rigidCC = bulletCC.GetComponent<Rigidbody2D>(); // 총알의 rigidbody2D 값을 가져옴
                Rigidbody2D rigidLL = bulletLL.GetComponent<Rigidbody2D>(); // 총알의 rigidbody2D 값을 가져옴
                rigidRR.AddForce(Vector2.up * 10, ForceMode2D.Impulse);   // addForce를 통해 총알에 힘을 부여
                rigidCC.AddForce(Vector2.up * 10, ForceMode2D.Impulse);   // addForce를 통해 총알에 힘을 부여
                rigidLL.AddForce(Vector2.up * 10, ForceMode2D.Impulse);   // addForce를 통해 총알에 힘을 부여
                break;
        }

        curShotDelay = 0;
    }

    private void Move()
    {
        float h = Input.GetAxisRaw("Horizontal"); // -1, 0, 1 딱 3가지 값만 들어옴.
        anim.SetInteger("Input", (int)h);
        if ((isTouchRight && h == 1) || (isTouchLeft && h == -1)) // 경계선에 닿았고 입력값이 있으면
            h = 0;  // 0으로 변경
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
        else if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("EnemyBullet"))
        {
            if (isHit)
                return;
            isHit = true;
            life--;
            gameManager.UpdateLifeIcon(life);
            if (life == 0)
            {
                gameManager.GameOver();
            }
            else
            {
                gameManager.RespawnPlayer();
            }
            gameObject.SetActive(false); // 플레이어
            collision.gameObject.SetActive(false);
        }
        else if( collision.gameObject.CompareTag("Item"))
        {
            Item item = collision.gameObject.GetComponent<Item>();
            switch(item.type)
            {
                case "Coin":
                    score += 1000;
                    break;
                case "Power":
                    if (power == maxPower)
                        score += 500;
                    else
                    {
                        power++;
                    }
                    break;
                case "Boom":
                    if (boom == maxBoom)
                        score += 500;
                    else
                    {
                        boom++;
                        gameManager.UpdateBoomIcon(boom);
                    }
                    break;
            }
            collision.gameObject.SetActive(false);
        }
    }

    private IEnumerator OffBoomEffect()
    {
        yield return new WaitForSeconds(2f);
        BoomEffect.SetActive(false);
        isBoomTime = false;
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
