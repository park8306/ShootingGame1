using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public string enemyName;
    public int enemyScore;
    public float speed;
    public int health;
    public Sprite[] sprites;

    SpriteRenderer spriteRenderer;
    Rigidbody2D rigid;

    public GameObject EnemyBulletA;
    public GameObject EnemyBulletB;
    public GameObject player;

    public float maxShotDelay;
    public float curShotDelay;
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>(); // 스프라이트 렌더러를 통해 피격효과 구현
    }

    private void Update()
    {
        Fire();
        Reload();
    }

    private void Reload()
    {
        curShotDelay += Time.deltaTime;
    }

    private void Fire()
    {
        if (curShotDelay < maxShotDelay)
            return;

        if(enemyName == "S")
        {
            GameObject bullet = Instantiate(EnemyBulletA, transform.position, transform.rotation); // 총알 생성하는 코드
            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>(); // 총알의 rigidbody2D 값을 가져옴

            Vector3 dirVec = player.transform.position - transform.position; // 플레이어 방향을 구함
            rigid.AddForce(dirVec.normalized * 3, ForceMode2D.Impulse);   // addForce를 통해 총알에 힘을 부여
        }
        else if(enemyName == "L")
        {
            GameObject bulletR = Instantiate(EnemyBulletB, transform.position + Vector3.right * 0.3f, transform.rotation); // 총알 생성하는 코드
            GameObject bulletL = Instantiate(EnemyBulletB, transform.position + Vector3.left * 0.3f, transform.rotation); // 총알 생성하는 코드
            Rigidbody2D rigidR = bulletR.GetComponent<Rigidbody2D>(); // 총알의 rigidbody2D 값을 가져옴
            Rigidbody2D rigidL = bulletL.GetComponent<Rigidbody2D>(); // 총알의 rigidbody2D 값을 가져옴
            Vector3 dirVecR = player.transform.position - (transform.position + Vector3.right * 0.3f);
            Vector3 dirVecL = player.transform.position - (transform.position + Vector3.left * 0.3f);
            rigidR.AddForce(dirVecR.normalized * 4, ForceMode2D.Impulse);   // addForce를 통해 총알에 힘을 부여
            rigidL.AddForce(dirVecL.normalized * 4, ForceMode2D.Impulse);   // addForce를 통해 총알에 힘을 부여
        }

        curShotDelay = 0;
    }

    public void OnHit(int dmg)
    {
        health -= dmg;
        StartCoroutine(ReturnSprite());

        if (health <= 0)
        {
            Player playerLogic = player.GetComponent<Player>();
            playerLogic.score += enemyScore;
            Destroy(gameObject);
        }
    }
    IEnumerator ReturnSprite()
    {
        spriteRenderer.sprite = sprites[1];
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.sprite = sprites[0];
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 적이 총알의 경계선에 닿으면 삭제 되도록 구현
        if(collision.gameObject.CompareTag("BorderBullet"))
        {
            Destroy(gameObject);
        }
        // 적이 총알에 맞으면 OnHit함수를 실행하도록 구현
        else if (collision.gameObject.CompareTag("PlayerBullet"))
        {
            Bullet bullet = collision.gameObject.GetComponent<Bullet>();
            OnHit(bullet.dmg);

            Destroy(collision.gameObject);
        }
    }
}
