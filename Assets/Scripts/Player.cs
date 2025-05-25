using System;
using System.Collections;
using Mono.Cecil;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;
    public int power;
    public int maxPower;

    public int boom;
    public int maxBoom;
    public bool isTouchTop;
    public bool isTouchBottom;
    public bool isTouchRight;
    public bool isTouchLeft;
    public bool isHit;
    public bool isBoomTime;

    public GameObject[] followers;

    public int score;
    public int life;
    
    
    
    
    
    //총알 발사 딜레이 로직을 위한 변수
 

    public float maxShotDelay; //현재의 딜레이
    public float curShotDelay; //총알을 쏘고나서의 딜레이
    
    //총알 prefab을 저장할 변수 생성
    public GameObject bulletObjectA;
    public GameObject bulletObjectB;
    public GameManager gameManager;
    
    public GameObject boomEffect;
    
    public ObjectManager objectManager;
    
    Animator anim;

     void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        Move();
        Fire();
        Reload();
        Boom();
    }
    
    void Start()
    {
        life = 3;
    }

//캡슐화
     void Move()
    {
        //플레이어 이동 구현
        float h = Input.GetAxisRaw("Horizontal");
        if ((isTouchRight && h == 1) || (isTouchLeft && h == -1))
            h = 0;
        float v = Input.GetAxisRaw("Vertical");
        if ((isTouchTop && v == 1) || (isTouchBottom && v == -1))
            v = 0;

        Vector3 curPos = transform.position;
        Vector3 nextPos = new Vector3(h, v, 0) * speed * Time.deltaTime;

        transform.position = curPos + nextPos;
        
        //플레이어 이동시 애니메이터 
        if(Input.GetButtonDown("Horizontal")||
           Input.GetButtonDown("Horizontal"))
        {
            anim.SetInteger("Input", (int)h); 
        }
    }

     //Instantiate-매개변수 오브젝트 생성하는 함수 (오버로드 많음?)
     //transform-위치,회전 
     
     //총알을 직접 만들어서 쏘는 로직
    void Fire()
    {
        //누르지 않았다면!! 리턴
        if(!Input.GetButton("Fire1"))
            return;
        
        //장전시간이 충족이 되었나?
        if(curShotDelay < maxShotDelay)
            return;


        switch (power)
        {
            case 1:
                //PowerOne
                GameObject bullet = objectManager.MakeObj("BulletPlayerA");
                bullet.transform.position = transform.position;
                
                Rigidbody2D rigid= bullet.GetComponent<Rigidbody2D>();
                rigid.AddForce(Vector2.up*10,ForceMode2D.Impulse);
                break;
            case 2:
                //PowerTwo
                GameObject bulletR = objectManager.MakeObj("BulletPlayerA"); 
                bulletR.transform.position = transform.position+Vector3.right*0.1f;
                
                GameObject bulletL = objectManager.MakeObj("BulletPlayerA");
                bulletL.transform.position = transform.position+Vector3.left*0.1f;
                
                Rigidbody2D rigidR= bulletR.GetComponent<Rigidbody2D>();
                Rigidbody2D rigidL= bulletL.GetComponent<Rigidbody2D>();
                rigidR.AddForce(Vector2.up*10,ForceMode2D.Impulse);
                rigidL.AddForce(Vector2.up*10,ForceMode2D.Impulse);
                break;
            default:
                GameObject bulletRR = objectManager.MakeObj("BulletPlayerA");
                bulletRR.transform.position = transform.position+Vector3.right*0.35f; 
                
                GameObject bulletCC = objectManager.MakeObj("BulletPlayerB");
                bulletCC.transform.position = transform.position;
                
                GameObject bulletLL = objectManager.MakeObj("BulletPlayerA");
                bulletLL.transform.position = transform.position+Vector3.left*0.35f;
                
                Rigidbody2D rigidRR= bulletRR.GetComponent<Rigidbody2D>();
                Rigidbody2D rigidCC= bulletCC.GetComponent<Rigidbody2D>();
                Rigidbody2D rigidLL= bulletLL.GetComponent<Rigidbody2D>();
                rigidRR.AddForce(Vector2.up*10,ForceMode2D.Impulse);
                rigidCC.AddForce(Vector2.up*10,ForceMode2D.Impulse);
                rigidLL.AddForce(Vector2.up*10,ForceMode2D.Impulse);
                break;
                
                
        }
        

        curShotDelay = 0;
    }
//딜레이 변수에 time.deltatime을 계속 더하며 계산
    void Reload()
    {
        curShotDelay += Time.deltaTime;
    }

    void Boom()
    {
        if(!Input.GetButton("Fire2"))
            return;
        if(isBoomTime)
            return;
        if(boom ==0)
            return;

        boom--;
        isBoomTime = true;
        gameManager.UpdateBoomIcon(boom);
        
        
        //#1 이펙트 켜기
        boomEffect.SetActive(true);
        Invoke("OffBoomEffect",4f);
                    
        //#2 Enemy 지우기
        GameObject[] enemiesL = objectManager.GetPool("EnemyL");
        GameObject[] enemiesM = objectManager.GetPool("EnemyM");
        GameObject[] enemiesS = objectManager.GetPool("EnemyS");
        for (int index = 0; index < enemiesL.Length; index++)
        {
            if (enemiesL[index].activeSelf)
            {
                Enemy enemyLogic = enemiesL[index].GetComponent<Enemy>();
                enemyLogic.OnHit(1000);
            }
        }
        for (int index = 0; index < enemiesM.Length; index++)
        {
            if (enemiesM[index].activeSelf)
            {
                Enemy enemyLogic = enemiesM[index].GetComponent<Enemy>();
                enemyLogic.OnHit(1000);
            }
        }
        for (int index = 0; index < enemiesS.Length; index++)
        {
            if (enemiesS[index].activeSelf)
            {
                Enemy enemyLogic = enemiesS[index].GetComponent<Enemy>();
                enemyLogic.OnHit(1000);
            }
        }
                    
        //#3 총알도 같이 지우기
        GameObject[] bulletsA = objectManager.GetPool("BulletEnemyA");
        GameObject[] bulletsB = objectManager.GetPool("BulletEnemyB");
        for (int index = 0; index < bulletsA.Length; index++)
        {
            if (bulletsA[index].activeSelf)
            {
                bulletsA[index].SetActive(false);
            }
            
        }
        for (int index = 0; index < bulletsB.Length; index++)
        {
            if (bulletsB[index].activeSelf)
            {
                bulletsB[index].SetActive(false);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Border")
        {
            switch (collision.gameObject.name)
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
        else if (collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "EnemyBullet")
        {
            
            if (isHit)
                return;
            
            isHit = true;
            life--;
            gameManager.UpdateLifeIcon(life);
            gameManager.CallExplosion(transform.position,"P");

            if (life == 0)
            {
                gameManager.GameOver();
            }
            else
            {
                gameManager.RespawnPlayer();
                
            }
            
            
            gameObject.SetActive(false);
            collision.gameObject.SetActive(false);
        }
        else if (collision.gameObject.tag == "Item")
        {
            Item item = collision.gameObject.GetComponent<Item>();
            switch (item.type)
            {
                case "Coin":
                    score += 1000;
                    break;
                case "Power" :
                    if (power == maxPower)
                        score += 500;
                    else
                    {
                        power++;
                        AddFollower();
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
                    
            } //아이템 먹으면 삭제
            collision.gameObject.SetActive(false);
        }
    }

    void AddFollower()
    {
        if (power == 4)
            followers[0].SetActive(true);
        else if (power == 5)
            followers[1].SetActive(true);
        else if (power == 6)
            followers[2].SetActive(true);
    }

    
    void OffBoomEffect()
    {
        boomEffect.SetActive(false);
        isBoomTime = false;
    }

    public IEnumerator OffHit()
    {
        yield return new WaitForSeconds(2f);
        isHit = false;
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Border")
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

