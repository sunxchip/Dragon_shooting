using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 적 스폰
/// </summary>
public class GameManager : MonoBehaviour
{
   public GameObject[] enemyObjs;
   public Transform[] spawnPoints;

   public float maxSpawnDelay;
   public float curSpawnDelay;

   //UI
   public GameObject player;
   public Text ScoreText;
   public Image[] lifeImages;
   public GameObject gameOverSet;
   
   void Update()
   {
      curSpawnDelay += Time.deltaTime;

      if (curSpawnDelay > maxSpawnDelay)
      {
         SpawnEnemy();
         maxSpawnDelay = Random.Range(0.5f,3f);
         curSpawnDelay = 0;
      }
      
      //UI score update
      Player playerLogic = player.GetComponent<Player>();
      ScoreText.text=string.Format("{0:n0}", playerLogic.score);
   }

   void SpawnEnemy()
   {
      int ranEnemy = Random.Range(0, 3);
      int ranPoint = Random.Range(0, 9);
      GameObject enemy =Instantiate(enemyObjs[ranEnemy],
                                    spawnPoints[ranPoint].position,
                                    spawnPoints[ranPoint].rotation);

      Rigidbody2D rigid = enemy.GetComponent<Rigidbody2D>();
      Enemy enemyLogic = enemy.GetComponent<Enemy>();
      enemyLogic.player = player;

      if (ranPoint == 5 || ranPoint==6) //오른쪽 적 스폰
      {
         enemy.transform.Rotate(Vector3.back*90);
         rigid.linearVelocity = new Vector2(enemyLogic.speed*(-1),1);
      }
      else if (ranPoint == 7 || ranPoint == 8) //왼쪽 적 스폰
      {
         enemy.transform.Rotate(Vector3.forward*90);
         rigid.linearVelocity = new Vector2(enemyLogic.speed, -1);
      }
      else //중앙 적 스폰
      {
         rigid.linearVelocity = new Vector2(0, enemyLogic.speed * (-1));
      }
   }

   public void UpdateLifeIcon(int life)
   {
      //UI life init Disable
      for (int index = 0; index < 3; index++)
      {
         lifeImages[index].color=new Color(1,1,1,0);
      }
      //UI life Active
      for (int index = 0; index < life; index++)
      {
         lifeImages[index].color=new Color(1,1,1,1);
      }
   }

   public void RespawnPlayer()
   {
      Invoke("RespawnPlayerExe",2f);
   }
   
  void RespawnPlayerExe()
   {
      player.transform.position = Vector3.down * 2f;
      player.SetActive(true);
   }

   public void GameOver()
   {
      gameOverSet.SetActive(true);
   }
}
