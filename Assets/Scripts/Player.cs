using System;
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

    public int score;
    public int life;
    
    
    
    
    //총알 발사 딜레이 로직을 위한 변수
 

    public float maxShotDelay; //현재의 딜레이
    public float curShotDelay; //총알을 쏘고나서의 딜레이
    
    //총알 prefab을 저장할 변수 생성
    public GameObject bulletObjectA;
    public GameObject bulletObjectB;
    public GameManager manager;
    
    public GameObject boomEffect;
    
    
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
                GameObject bullet = Instantiate(bulletObjectA, transform.position,transform.rotation);
                Rigidbody2D rigid= bullet.GetComponent<Rigidbody2D>();
                rigid.AddForce(Vector2.up*10,ForceMode2D.Impulse);
                break;
            case 2:
                //PowerTwo
                GameObject bulletR = Instantiate(bulletObjectA, transform.position+Vector3.right*0.1f ,transform.rotation); 
                GameObject bulletL = Instantiate(bulletObjectA, transform.position+Vector3.left*0.1f ,transform.rotation);
                
                Rigidbody2D rigidR= bulletR.GetComponent<Rigidbody2D>();
                Rigidbody2D rigidL= bulletL.GetComponent<Rigidbody2D>();
                rigidR.AddForce(Vector2.up*10,ForceMode2D.Impulse);
                rigidL.AddForce(Vector2.up*10,ForceMode2D.Impulse);
                break;
            case 3:
                GameObject bulletRR = Instantiate(bulletObjectA, transform.position+Vector3.right * 0.35f ,transform.rotation); 
                GameObject bulletCC = Instantiate(bulletObjectB, transform.position ,transform.rotation);
                GameObject bulletLL = Instantiate(bulletObjectA, transform.position+Vector3.left*0.35f ,transform.rotation);
                
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
        manager.UpdateBoomIcon(boom);
        
        
        //#1 이펙트 켜기
        boomEffect.SetActive(true);
        Invoke("OffBoomEffect",4f);
                    
        //#2 Enemy 지우기
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        for (int index = 0; index < enemies.Length; index++)
        {
            Enemy enemyLogic = enemies[index].GetComponent<Enemy>();
            enemyLogic.OnHit(1000);
        }
                    
        //#3 총알도 같이 지우기
        GameObject[] bullets = GameObject.FindGameObjectsWithTag("EnemyBullet");
        for (int index = 0; index < bullets.Length; index++)
        {
            Destroy(bullets[index]);
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
            manager.UpdateLifeIcon(life);

            if (life == 0)
            {
                manager.GameOver();
            }
            else
            {
                manager.RespawnPlayer();
                
            }
            
            
            gameObject.SetActive(false);
            Destroy(collision.gameObject);
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
                        power++;
                    break;
                case "Boom":
                    if (boom == maxBoom)
                        score += 500;
                    else
                    {
                        boom++;
                        manager.UpdateBoomIcon(boom);
                    }
                    

                    break;
                    
            } //아이템 먹으면 삭제
            Destroy(collision.gameObject);
        }
    }

    void OffBoomEffect()
    {
        boomEffect.SetActive(false);
        isBoomTime = false;
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

