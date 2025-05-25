using System;
using UnityEngine;

public class Explosion : MonoBehaviour
{ 
    Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void Disable()
    {
        gameObject.SetActive(false);
    }
    
    //폭발 오브젝트가 스스로 비활성화 되도록 로직 작성
    private void OnEnable()
    {
        Invoke("Disable",2f);
    }

    public void StartExplosion(string target)
    {
        anim.SetTrigger("OnExplosion");

        switch (target)
        {
            case"S":
                transform.localScale = Vector3.one *0.7f;
                break;
            case "M":
            case "P":
                transform.localScale = Vector3.one * 1f;
                break;
            case "L":
                transform.localScale = Vector3.one * 2f;
                break;
            case "B":
                transform.localScale = Vector3.one * 3f;
                break;
                
        }
    }
}
