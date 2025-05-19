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

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        
    }

    void OnEnable()
    {
        switch (enemyName)
        {
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
    
    void Update()
    {
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
        spriteRenderer.sprite = sprites[1];
        //바꾼 스프라이트를 돌리기 위해 시간차 함수 호출
        Invoke("ReturnSprite",0.1f);
        
        //죽으면 파괴
        if (health <= 0)
        {
            Player playerLogic = player.GetComponent<Player>();
            playerLogic.score += enemyScore;
            
            //#. Random Ratio Item Drop
            int ran = Random.Range(0, 10);
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
        if (collision.gameObject.tag == "BorderBullet")
        {
            transform.rotation=Quaternion.identity;
            gameObject.SetActive(false);
        }

        else if (collision.gameObject.tag == "PlayerBullet")
        {
            Bullet bullet = collision.gameObject.GetComponent<Bullet>();
            OnHit(bullet.dmg);

            collision.gameObject.SetActive(false);
        }
    }
}
