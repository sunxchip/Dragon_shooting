using System;
using UnityEngine;

public class Item : MonoBehaviour
{
   public string type;
   private Rigidbody2D rigid;

    void Awake()
   {
       rigid = GetComponent<Rigidbody2D>();
   }

    void OnEnable()
    {
        rigid.linearVelocity = Vector2.down * 1.5f;
    }
}
