using System;
using UnityEngine;
/// <summary>
/// 총알
/// </summary>
public class Bullet : MonoBehaviour
{
    public int dmg;

    public bool isRotate;

 void Update()
    {
        if(isRotate)
            transform.Rotate(Vector3.forward*10);
    }

    //총알이 화면 밖으로 나갔을때 사라지게 하기
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "BorderBullet")
        {
            gameObject.SetActive(false);;
            
        }
    }
}
