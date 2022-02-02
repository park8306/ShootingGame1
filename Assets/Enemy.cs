using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public string enemyName;
    public int enemyScore;
    public float speed;
    public int maxHP;
    public int HP;
    public Sprite[] sprites;

    SpriteRenderer spriteRenderer;
    Rigidbody2D rigid;

    public GameObject EnemyBulletA;
    public GameObject EnemyBulletB;
    public GameObject itemCoin;
    public GameObject itemBoom;
    public GameObject itemPower;
    public GameObject player;
    public ObjectManager objectManager;

    public float maxShotDelay;
    public float curShotDelay;

    Transform fTransform;
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>(); // 스프라이트 렌더러를 통해 피격효과 구현
        fTransform = transform;
        HP = maxHP;
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
            GameObject bullet = objectManager.MakeObj("BulletEnemyA"); // 총알 생성하는 코드
            bullet.transform.position = transform.position;
            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>(); // 총알의 rigidbody2D 값을 가져옴

            Vector3 dirVec = player.transform.position - transform.position; // 플레이어 방향을 구함
            rigid.AddForce(dirVec.normalized * 3, ForceMode2D.Impulse);   // addForce를 통해 총알에 힘을 부여
        }
        else if(enemyName == "L")
        {
            GameObject bulletR = objectManager.MakeObj("BulletEnemyB"); // 총알 생성하는 코드
            bulletR.transform.position = transform.position + Vector3.right *0.3f;

            GameObject bulletL = objectManager.MakeObj("BulletEnemyB"); // 총알 생성하는 코드
            bulletL.transform.position = transform.position + Vector3.left * 0.3f;

            Rigidbody2D rigidR = bulletR.GetComponent<Rigidbody2D>(); // 총알의 rigidbody2D 값을 가져옴
            Rigidbody2D rigidL = bulletL.GetComponent<Rigidbody2D>(); // 총알의 rigidbody2D 값을 가져옴
            Vector3 dirVecR = player.transform.position - (transform.position + Vector3.right * 0.3f);
            Vector3 dirVecL = player.transform.position - (transform.position + Vector3.left * 0.3f);
            rigidR.AddForce(dirVecR.normalized * 4, ForceMode2D.Impulse);   // addForce를 통해 총알에 힘을 부여
            rigidL.AddForce(dirVecL.normalized * 4, ForceMode2D.Impulse);   // addForce를 통해 총알에 힘을 부여
        }

        curShotDelay = 0;
    }
    Coroutine handle;
    public void OnHit(int dmg)
    {
        if (HP <= 0)
            return;
        HP -= dmg;
        if(gameObject.activeSelf)
            handle = StartCoroutine(ReturnSprite());
        else
        {
            handle = null;
        }

        if (HP <= 0)
        {
            Player playerLogic = player.GetComponent<Player>();
            playerLogic.score += enemyScore;

            // 랜덤 확률로 아이템 드롭
            int ran = Random.Range(0, 10);
            if(ran < 3)
            {
                Debug.Log("Not Item");
            }
            else if (ran < 6)
            {
                GameObject itemCoin = objectManager.MakeObj("ItemCoin");
                itemCoin.transform.position = transform.position;
                ItemDown(itemCoin);
            }
            else if (ran < 8)
            {
                GameObject itemPower = objectManager.MakeObj("ItemPower");
                itemPower.transform.position = transform.position;
                ItemDown(itemPower);
            }
            else if (ran < 10)
            {
                GameObject itemBoom = objectManager.MakeObj("ItemBoom");
                itemBoom.transform.position = transform.position;
                ItemDown(itemBoom);
            }

            HP = maxHP;
            spriteRenderer.sprite = sprites[0];
            transform.rotation = Quaternion.Euler(0, 0, 0);
            if(handle != null)
                StopCoroutine(handle);
            gameObject.SetActive(false);
        }
    }

    private static void ItemDown(GameObject item)
    {
        Rigidbody2D rigid = item.GetComponent<Rigidbody2D>();
        rigid.velocity = Vector2.down * 3;
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
            HP = maxHP;
            transform.rotation = Quaternion.Euler(0,0,0);
            if (handle != null)
                StopCoroutine(handle);
            gameObject.SetActive(false);
        }
        // 적이 총알에 맞으면 OnHit함수를 실행하도록 구현
        else if (collision.gameObject.CompareTag("PlayerBullet"))
        {
            Bullet bullet = collision.gameObject.GetComponent<Bullet>();
            OnHit(bullet.dmg);

            collision.gameObject.SetActive(false);
        }
    }
}
