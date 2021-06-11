using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class flower : Character
{
    public float attackTime = 3.0f;

    GameObject player;
    Player playerScript;
    Animator animator;
    private Vector3 stopPos;
    private float setTime = 0;
    private bool seve = true;
    protected override void Start()
    {
        base.Start();//消さないでください。
        animator = GetComponent<Animator>();
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<Player>();
        //ここから処理を書いてください。
    }

    protected override void Update()
    {
        base.Update();//消さないでください。
        if (seve)
        {
            stopPos = player.transform.position;
            setTime = Time.time;
        }
        else
        {
            player.transform.position = stopPos;
        }

        Attack();
        //ここから処理を書いてください。

    }

    void Attack()
    {
        if (!seve)
        {
            float time = Time.time - setTime;
            if (attackTime < time)
            {
                playerScript.ApplyDamage(1);
                setTime = Time.time;
            }
        }
    }



    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player"
           && playerScript.CompareLayerAndColliderMask(gameObject.layer))
        {
            animator.SetBool("Attack", true);
            seve = false;
        }
    }
}
