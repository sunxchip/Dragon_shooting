using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;
using System.Collections.Generic;
using System.IO;


/// <summary>
/// 적 스폰
/// </summary>
public class GameManager : MonoBehaviour
{
   public string[] enemyObjs;
   public Transform[] spawnPoints;

   public float nextSpawnDelay;
   public float curSpawnDelay;

   //UI
   public GameObject player;
   public Text scoreText;
   public Image[] lifeImage;
   public Image[] boomImage;
   public GameObject gameOverSet;
   public ObjectManager objectManager;


   public List<Spawn> spawnList;
   public int spawnIndex;
   public bool spawnEnd;
   

   void Awake()
   {
      spawnList = new List<Spawn>();
      enemyObjs = new string[] { "EnemyS", "EnemyM", "EnemyL" };
      ReadSpawnFile();
   }

   void ReadSpawnFile()
   {
       //변수 초기화
       spawnList.Clear();
       spawnIndex = 0;
       spawnEnd = false;
       
       //리스폰 파일 읽기
       TextAsset textFile = Resources.Load("Stage 0") as TextAsset;
       StringReader stringReader = new StringReader(textFile.text);

       //한줄씩 데이터 저장
       while (stringReader != null)
       {
          string line = stringReader.ReadLine();
          Debug.Log(line);
         
          if (line == null)
             break;

          //리스폰 데이터 생성
          Spawn spawnData = new Spawn();
          spawnData.delay = float.Parse(line.Split(',')[0]);
          spawnData.type = line.Split(',')[1];
          spawnData.point = int.Parse(line.Split(',')[2]);
          spawnList.Add(spawnData);
       }
       //텍스트 파일 닫기 
       stringReader.Close();
       //첫번째 스폰 딜레이 적용
       nextSpawnDelay = spawnList[0].delay;
   }

   void Update()
   {
      curSpawnDelay += Time.deltaTime;

      if (curSpawnDelay > nextSpawnDelay && ! spawnEnd)
      {
         SpawnEnemy();
         curSpawnDelay = 0;
      }
      
      //UI score update
      Player playerLogic = player.GetComponent<Player>();
      scoreText.text=string.Format("{0:n0}", playerLogic.score);
   }

   void SpawnEnemy()
   {
      int enemyIndex = 0;
      switch (spawnList[spawnIndex].type)
      {
         case"S":
            enemyIndex = 0;
            break;
         case"M":
            enemyIndex = 1;
            break;
         case"L":
            enemyIndex = 2;
            break;
      }
      int enemyPoint = spawnList[spawnIndex].point;
      
      GameObject enemy = objectManager.MakeObj(enemyObjs[enemyIndex]);
      enemy.transform.position = spawnPoints[enemyPoint].position;
      
      Rigidbody2D rigid = enemy.GetComponent<Rigidbody2D>();
      Enemy enemyLogic = enemy.GetComponent<Enemy>();
      enemyLogic.player = player;
      enemyLogic.objectManager = objectManager;

      if (enemyPoint == 5 || enemyPoint==6) //오른쪽 적 스폰
      {
         enemy.transform.Rotate(Vector3.back*90);
         rigid.linearVelocity = new Vector2(enemyLogic.speed*(-1),1);
      }
      else if (enemyPoint == 7 || enemyPoint == 8) //왼쪽 적 스폰
      {
         enemy.transform.Rotate(Vector3.forward*90);
         rigid.linearVelocity = new Vector2(enemyLogic.speed, -1);
      }
      else //중앙 적 스폰
      {
         rigid.linearVelocity = new Vector2(0, enemyLogic.speed * (-1));
      }
      
      //#.리스폰 인덱스 증가 
      spawnIndex++;
      if (spawnIndex == spawnList.Count)
      {
         spawnEnd=true;
         return;
      }
      //#.다음 리스폰 갱신
      nextSpawnDelay = spawnList[spawnIndex].delay;
   }

   public void UpdateLifeIcon(int life)
   {
      //UI life init Disable
      for (int index = 0; index < 3; index++)
      {
         lifeImage[index].color=new Color(1,1,1,0);
      }
      //UI life Active
      for (int index = 0; index < life; index++)
      {
         lifeImage[index].color=new Color(1,1,1,1);
      }
   }

   public void UpdateBoomIcon(int boom)
   {
      //UI boom init Disable
      for (int index = 0; index < 3; index++)
      {
         boomImage[index].color=new Color(1,1,1,0);
      }
      //UI boom Active
      for (int index = 0; index < boom; index++)
      {
         boomImage[index].color=new Color(1,1,1,1);
      }
   }
   public void RespawnPlayer()
   {
      Invoke("RespawnPlayerExe",2f);
   }
   
   //한번에 두발 동시에 맞을 시 두번 죽지 않도록 예외처리 추가 
  void RespawnPlayerExe()
   {
      player.transform.position = Vector3.down * 2f;
      player.SetActive(true);
      
      Player playerLogic = player.GetComponent<Player>();
      playerLogic.isHit = true;
      playerLogic.StartCoroutine(playerLogic.OffHit());
   }

   public void GameOver()
   {
      gameOverSet.SetActive(true);
   }

   public void GameRetry()
   {
      SceneManager.LoadScene(0);
   }
}
