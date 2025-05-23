using System;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class Enemy : MonoBehaviour
{
    public string enemyName;
    public float speed;
    public int health;
    public Sprite[] sprites;

    public int enemyScore;

    public GameObject bulletObjectA;
    public GameObject bulletObjectB;
    
    public GameObject itemCoin;
    public GameObject itemBoom;
    public GameObject itemPower;
    
    public float maxShotDelay; //현재의 딜레이
    public float curShotDelay; //총알을 쏘고나서의 딜레이
    
    public GameObject player;
    public ObjectManager objectManager;
    
    SpriteRenderer spriteRenderer;
    Rigidbody2D rigid;
    Animator anim;

    public int patternIndex;
    public int curPatternCount;
    public int[] maxPatternCount;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (enemyName == "B")
            anim = GetComponent<Animator>();
    }

    void OnEnable()
    {
        switch (enemyName)
        {
            case "B":
                health = 3000;
                Invoke("Stop",2);
                break;
            case"L":
                health = 40;
                break;
            case "M":
                health = 10;
                break;
            case "S":
                health = 3;
                break;
        }
    }

    void Stop()
    {
        if(!gameObject.activeSelf)
            return;
        
        Rigidbody2D rigid = GetComponent<Rigidbody2D>();
        rigid.linearVelocity = Vector2.zero;
        
        Invoke("Think",2);
    }

    void Think()
    {
        patternIndex = patternIndex == 3 ? 0 : patternIndex + 1;
        curPatternCount = 0;

        switch (patternIndex)
        {
            case 0:
                FireFoward();
                break;
            case 1:
                FireShot();
                break;
            case 2:
                FireArc();
                break;
            case 3:
                FireAround();
                break;
        }
    }

    void FireFoward()
    {
        //#.Fire 4 Bullet Forward
        GameObject bulletR = objectManager.MakeObj("BulletBossA");
        bulletR.transform.position = transform.position+Vector3.right*0.3f;
        GameObject bulletRR = objectManager.MakeObj("BulletBossA");
        bulletRR.transform.position = transform.position+Vector3.right*0.45f;
        GameObject bulletL = objectManager.MakeObj("BulletBossA");
        bulletL.transform.position = transform.position+Vector3.left*0.3f;
        GameObject bulletLL = objectManager.MakeObj("BulletBossA");
        bulletLL.transform.position = transform.position+Vector3.left*0.45f;
            
        Rigidbody2D rigidR= bulletR.GetComponent<Rigidbody2D>();
        Rigidbody2D rigidRR= bulletRR.GetComponent<Rigidbody2D>();
        Rigidbody2D rigidL= bulletL.GetComponent<Rigidbody2D>();
        Rigidbody2D rigidLL= bulletLL.GetComponent<Rigidbody2D>();
        
        rigidR.AddForce(Vector2.down*8,ForceMode2D.Impulse);
        rigidRR.AddForce(Vector2.down*8,ForceMode2D.Impulse);
        rigidL.AddForce(Vector2.down*8,ForceMode2D.Impulse);
        rigidLL.AddForce(Vector2.down*8,ForceMode2D.Impulse);

        //#.Pattern Counting
        curPatternCount++;
        if(curPatternCount < maxPatternCount[patternIndex])
            Invoke("FireFoward",2f);
        else
            Invoke("Think",2);
        
    }

    void FireShot()
    {
        //#.Fire 5 Random Shotgun Bullet to Player
        for (int index = 0; index < 5; index++)
        {
            GameObject bullet = objectManager.MakeObj("BulletEnemyB");
            bullet.transform.position = transform.position;

            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
            Vector2 dirVec = player.transform.position - transform.position;
            Vector2 ranVec = new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(0f, 2f));
            dirVec += ranVec;
            rigid.AddForce(dirVec.normalized * 3, ForceMode2D.Impulse);
        }

        curPatternCount++;
            if(curPatternCount < maxPatternCount[patternIndex])
                Invoke("FireShot",3.5f);
            else
                Invoke("Think",3);
            
        
       
    }

    void FireArc()
    {
        // #.Fire Arc Continue Fire
        GameObject bullet = objectManager.MakeObj("BulletEnemyA");
        bullet.transform.position = transform.position;
        bullet.transform.rotation = Quaternion.identity;

        Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
        Vector2 dirVec = new Vector2(Mathf.Sin( Mathf.PI * 10 * curPatternCount / maxPatternCount[patternIndex]), -1);
        rigid.AddForce(dirVec.normalized * 5, ForceMode2D.Impulse);

        
        curPatternCount++;
        if(curPatternCount < maxPatternCount[patternIndex])
            Invoke("FireArc",0.15f);
        else
            Invoke("Think",3);
    }

    void FireAround()
    {
        // #.Fire Around
        int roundNumA = 50;
        int roundNumB = 40;
        int roundNum = curPatternCount % 2 == 0 ? roundNumA : roundNumB;
        

        for (int index = 0; index < roundNumA; index++)
        {
            GameObject bullet = objectManager.MakeObj("BulletBossB");
            bullet.transform.position = transform.position;
            bullet.transform.rotation = Quaternion.identity;

            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
            Vector2 dirVec = new Vector2(
                Mathf.Cos(Mathf.PI * 2 * index / roundNum),
                Mathf.Sin(Mathf.PI * 2 * index / roundNum)
            );

            rigid.AddForce(dirVec.normalized * 2, ForceMode2D.Impulse);

            Vector3 rotVec = Vector3.forward * 360 * index / roundNum + Vector3.forward * 90;
            bullet.transform.Rotate(rotVec);
        }

     
        curPatternCount++;
        
        if(curPatternCount < maxPatternCount[patternIndex])
            Invoke("FireAround",0.7f);
        else
            Invoke("Think",3);
    }
    
    void Update()
    {
        if (enemyName == "B")
            return;
        
        Fire();
        Reload();
    }
    
    void Fire()
    {
        
        //장전시간이 충족이 되었나?
        if(curShotDelay < maxShotDelay)
            return;
        if (enemyName == "S")
        {
            GameObject bullet = objectManager.MakeObj("BulletEnemyA");
            bullet.transform.position = transform.position;
            
            Rigidbody2D rigid= bullet.GetComponent<Rigidbody2D>();
            Vector3 dirVec = player.transform.position - transform.position;
            rigid.AddForce(dirVec.normalized*3,ForceMode2D.Impulse);
        }
        else if (enemyName == "L")
        {
            GameObject bulletR = objectManager.MakeObj("BulletEnemyB");
            bulletR.transform.position = transform.position+Vector3.right*0.3f;
            GameObject bulletL = objectManager.MakeObj("BulletEnemyB");
            bulletL.transform.position = transform.position+Vector3.left*0.3f;
            
            Rigidbody2D rigidR= bulletR.GetComponent<Rigidbody2D>();
            Rigidbody2D rigidL= bulletL.GetComponent<Rigidbody2D>();
            
            Vector3 dirVecR = player.transform.position - (transform.position+Vector3.right*0.3f);
            Vector3 dirVecL = player.transform.position - (transform.position+Vector3.left*0.3f);
            
            //normalized:벡터가 단위 값(1)로 변환된 변수 
            rigidR.AddForce(dirVecR.normalized*10,ForceMode2D.Impulse);
            rigidL.AddForce(dirVecL.normalized*10,ForceMode2D.Impulse);
        }
 
        
        curShotDelay = 0;
    }
    
    
//딜레이 변수에 time.deltatime을 계속 더하며 계산
    void Reload()
    {
        curShotDelay += Time.deltaTime;
    }

    
//총알을 맞았을때 데미지 
//맞았을때 스프라이트 변환
    public void OnHit(int dmg)
    {
        if(health <= 0)
            return;
        
        
        health -= dmg;
        if (enemyName == "B")
        {
            anim.SetTrigger("OnHit");
        }
        else
        {
            //바꾼 스프라이트를 돌리기 위해 시간차 함수 호출
            spriteRenderer.sprite = sprites[1];
            Invoke("ReturnSprite", 0.1f);
        }
        

        //죽으면 파괴
        if (health <= 0)
        {
            Player playerLogic = player.GetComponent<Player>();
            playerLogic.score += enemyScore;
            
            //#. Random Ratio Item Drop
            int ran = enemyName == "B" ? 0 : Random.Range(0, 10);
            if (ran < 3) //Not Item 30%
            {
                Debug.Log("Not Item");
            }
            else if (ran < 6) //Coin 30%
            {
                GameObject itemCoin=objectManager.MakeObj("ItemCoin");
                itemCoin.transform.position = transform.position;
                
            }
            else if (ran < 8) //Power 20%
            {
                GameObject itemPower=objectManager.MakeObj("ItemPower");
                itemPower.transform.position = transform.position;
                
            }
            else if (ran < 10) //boom 20%
            {
                GameObject itemBoom=objectManager.MakeObj("ItemBoom");
                itemBoom.transform.position = transform.position;
                
            }
            
            gameObject.SetActive(false);
            transform.rotation=Quaternion.identity;
        }
    }

    void ReturnSprite()
    {
        spriteRenderer.sprite = sprites[0];
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "BorderBullet" && enemyName != "B")
        {
            gameObject.SetActive(false);
            transform.rotation=Quaternion.identity;
        }

        else if (collision.gameObject.tag == "PlayerBullet")
        {
            Bullet bullet = collision.gameObject.GetComponent<Bullet>();
            OnHit(bullet.dmg);

            collision.gameObject.SetActive(false);
        }
    }
}
