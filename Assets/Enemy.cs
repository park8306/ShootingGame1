using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed;
    public int health;
    public Sprite[] sprites;

    SpriteRenderer spriteRenderer;
    Rigidbody2D rigid;
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigid = GetComponent<Rigidbody2D>();
        rigid.velocity = Vector2.down * speed;
    }

    void OnHit(int dmg)
    {
        health -= dmg;
        StartCoroutine(ReturnSprite());

        if (health <= 0)
        {
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
        }
    }
}
