using System;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class Enemy : MonoBehaviour
{
    public string enemyName;
    public float speed;
    public int health;
    public Sprite[] sprites;

    public GameObject bulletObjectA;
    public GameObject bulletObjectB;
    
    public float maxShotDelay; //현재의 딜레이
    public float curShotDelay; //총알을 쏘고나서의 딜레이
    
    public GameObject player;
    
    SpriteRenderer spriteRenderer;
    Rigidbody2D rigid;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        
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
            GameObject bullet = Instantiate(bulletObjectA, transform.position,transform.rotation);
            Rigidbody2D rigid= bullet.GetComponent<Rigidbody2D>();
            Vector3 dirVec = player.transform.position - transform.position;
            rigid.AddForce(dirVec.normalized*3,ForceMode2D.Impulse);
        }
        else if (enemyName == "L")
        {
            GameObject bulletR = Instantiate(bulletObjectB, transform.position+Vector3.right*0.3f,transform.rotation);
            GameObject bulletL = Instantiate(bulletObjectB, transform.position+Vector3.left*0.3f,transform.rotation);
            
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
    void OnHit(int dmg)
    {
        health -= dmg;
        spriteRenderer.sprite = sprites[1];
        //바꾼 스프라이트를 돌리기 위해 시간차 함수 호출
        Invoke("ReturnSprite",0.1f);
        
        //죽으면 파괴
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    void ReturnSprite()
    {
        spriteRenderer.sprite = sprites[0];
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag=="BorderBullet")
           Destroy(gameObject);
           
        else if (collision.gameObject.tag == "PlayerBullet")
        {
            Bullet bullet = collision.gameObject.GetComponent<Bullet>();
            OnHit(bullet.dmg);

            Destroy(collision.gameObject);
        }
    }
}
