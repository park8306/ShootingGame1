using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public GameObject[] enemyObjs;  // 적 프리펩 저장
    public Transform[] spawnPoints; // 생성할 위치 저장

    public float maxSpawnDelay; // 생성 딜레이
    public float curSpawnDelay;

    public GameObject player; // 게임 메니저가 플레이어를 들고 있어야 함
    public Text scoreText;
    public GameObject[] lifeImageObjs;
    public GameObject[] boomImageObjs;
    public GameObject gameOverSet;

    private void Awake()
    {
        UpdateBoomIcon(player.GetComponent<Player>().boom);
    }

    private void Update()
    {
        curSpawnDelay += Time.deltaTime;
        if(curSpawnDelay > maxSpawnDelay)
        {
            SpawnEnemy();   // 적 소환
            maxSpawnDelay = Random.Range(0.5f, 3f); // 생성 딜레이 값을 랜덤으로 조정
            curSpawnDelay = 0;
        }

        Player playerLogic = player.GetComponent<Player>();
        scoreText.text = string.Format("{0:n0}", playerLogic.score);
    }

    private void SpawnEnemy()
    {
        int ranEnemy = Random.Range(0, 3); // 0~2 랜덤한 적 생성
        int ranPoint = Random.Range(0, 9); // 0~4 랜덤한 위치에서 생성
        GameObject enemy = Instantiate(enemyObjs[ranEnemy], spawnPoints[ranPoint].position, spawnPoints[ranPoint].rotation);
        Rigidbody2D rigid = enemy.GetComponent<Rigidbody2D>();
        Enemy enemyLogic = enemy.GetComponent<Enemy>();
        enemyLogic.player = player; // 적에게 플레이어 정보를 넘겨 줌 (플레이어의 위치로 발사하기위해 필요하기 때문)
        if(ranPoint == 5 || ranPoint == 6)
        {
            enemy.transform.Rotate(Vector3.back * 90);
            rigid.velocity = new Vector2(enemyLogic.speed * (-1), -1);
        }
        else if (ranPoint == 7 || ranPoint == 8)
        {
            enemy.transform.Rotate(Vector3.forward * 90);
            rigid.velocity = new Vector2(enemyLogic.speed, -1);
        }
        else
        {
            rigid.velocity = new Vector2(0, enemyLogic.speed * (-1));
        }
    }
    public void RespawnPlayer()
    {
        StartCoroutine(RespawnPlayerExe());
    }

    IEnumerator RespawnPlayerExe()
    {
        yield return new WaitForSeconds(1.5f);
        player.transform.position = Vector3.up * -4; // 위치 초기화
        player.SetActive(true); // 플레이어 다시 살리기
        Player playerLogic = player.GetComponent<Player>();
        playerLogic.isHit = false;
    }
    internal void UpdateLifeIcon(int life)
    {
        if (life < 0)
            return;
        lifeImageObjs[life].SetActive(false);
    }
    internal void UpdateBoomIcon(int boom)
    {
        if (boom < 0)
            return;
        boomImageObjs[boom].SetActive(false);
    }
    internal void GameOver()
    {
        gameOverSet.SetActive(true);
    }

    public void GameRetry()
    {
        SceneManager.LoadScene(0);
    }
}
