using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemController : Character
{
    public float jumpForce = 5;
    public float jumpPoint = 15;
    public float speed = 1;
    public float distance = 7;
    public float groundCheckSize = 1;
    public float attackTime = 1;
    public int junpTime = 3;
    public int StopTime = 2;
    public int damage = 30;

    Player playerScript;
    ResponeGolemturu spawnPoint1;
    ResponeGolemturu spawnPoint2;
    Spine.Unity.SkeletonAnimation skeletonAnimation;
    private GameObject player;
    public GameObject taru;
    private Animator animator;
    private int time;
    private int ST = 0;
    private int oldHp;
    private int turn = 0;
    private int TurnNumber = 2;
    private float M_time;
    private float attack_start;
    private float move = 1;
    private float pos;
    private float count;
    private float Speedup = 19;
    private bool oldIsGrounded;
    private bool jump = false;
    private bool moveON = false;
    private bool setTime = false;
    private bool attack = false;

    protected override void Start()
    {
        base.Start();//消さないでください。

        count += Time.deltaTime;
        time = (int)count;
        ST = time;
        //ここから処理を書いてください。
        player = GameObject.Find("Player");
        playerScript = player.gameObject.GetComponent<Player>();
        animator = GetComponent<Animator>();
        skeletonAnimation = GetComponent<Spine.Unity.SkeletonAnimation>();
        oldHp = hp;
        spawnPoint1 = transform.GetChild(0).gameObject.GetComponent<ResponeGolemturu>();
        spawnPoint2 = transform.GetChild(1).gameObject.GetComponent<ResponeGolemturu>();

    }


    protected override void Update()
    {
        base.Update();//消さないでください。
        if (attack)
        {
            PlayerDrop();
        }
        else
        {
            //ここから処理を書いてください。
            count += Time.deltaTime;
            time = (int)count;

            Vector3 player_pos = player.transform.position;
            if (player_pos.x < 0)
            {
                player_pos *= -1;
            }
            pos = transform.position.x - player_pos.x;
            if (pos < 0)
            {
                pos *= -1;

                skeletonAnimation.skeleton.FlipX = true;
            }
            else
            {

                skeletonAnimation.skeleton.FlipX = false;
            }

            if (playerScript.CompareLayerAndColliderMask(gameObject.layer))
            {
                if (!spawnPoint1.origin || !spawnPoint2.origin)
                {
                    Jump();
                    jump = true;
                }
            }



            if (IsGrounded() && oldIsGrounded == false && jump)
            {
                jump = false;
                spawnPoint1.Respaen = true;
                spawnPoint2.Respaen = true;
            }

            if (time - ST > StopTime && !jump)
            {
                if (pos < distance)
                {
                    if (!setTime)
                    {
                        attack_start = Time.time;
                        setTime = true;
                    }
                    Attack();

                }
                else
                {
                    moveON = true;
                    if (!setTime)
                    {
                        M_time = Time.time;
                        setTime = true;
                    }
                    animator.SetBool("isAttack", false);
                }
            }

            if (moveON && !jump)
            {
                B_move();
                animator.SetBool("isMoving", true);
            }
            if (Time.time - M_time > 2.0 && moveON)
            {
                moveON = false;
                ST = time;
                setTime = false;
                animator.SetBool("isMoving", false);
            }

            if (oldHp > hp)
            {
                attack = true;
                M_time = Time.time;
                speed *= Speedup;
                animator.SetBool("isAttack", false);
            }
        }

        oldHp = hp;
        oldIsGrounded = IsGrounded();
    }
    private void Attack()
    {

        if (Time.time - attack_start < attackTime)
        {
            animator.SetBool("isAttack", true);
        }
        else
        {
            ST = time;
            setTime = false;
            animator.SetBool("isAttack", false);
        }

    }
    private void Jump()
    {
        if (IsGrounded())
        {
            rigidbody2D.velocity = Vector2.up * jumpForce;
        }
    }

    private void B_move()
    {
        if (skeletonAnimation.skeleton.FlipX)
        {
            Vector3 Bmove = transform.position;
            Bmove.x += move * speed * Time.deltaTime;
            transform.position = Bmove;
        }
        else if (!skeletonAnimation.skeleton.FlipX)
        {

            Vector3 Bmove = transform.position;
            Bmove.x -= move * speed * Time.deltaTime;
            transform.position = Bmove;
        }
    }

    private void PlayerDrop()
    {
        Vector3 player_p;
        player_p = player.transform.position;
        if (TurnNumber <= turn)
        {
            if (Time.time - M_time < 0.5)
            {
                B_move();
                if (skeletonAnimation.skeleton.FlipX)
                {
                    player_p.x += move * speed * 2 * Time.deltaTime;
                }
                else if (!skeletonAnimation.skeleton.FlipX)
                {
                    player_p.x -= move * speed * 2 * Time.deltaTime;
                }
                player.transform.position = player_p;
                animator.SetBool("isMoving", true);
            }
            else
            {

                speed /= Speedup;
                TurnNumber++;
                attack = false;
                turn = 0;
                animator.SetBool("isMoving", false);
            }

        }
        else
        {
            if (Time.time - M_time < 0.3)
            {
                B_move();
                if (skeletonAnimation.skeleton.FlipX)
                {
                    player_p.x += move * speed / Speedup * Time.deltaTime;
                }
                else if (!skeletonAnimation.skeleton.FlipX)
                {
                    player_p.x -= move * speed / Speedup * Time.deltaTime;
                }
                player.transform.position = player_p;
                animator.SetBool("isMoving", true);
            }

            else
            {
                chack();
                M_time = Time.time;
                turn++;
            }
        }

    }
    public override void OnTriggerEnter2DOfAttacher(Collider2D collision)
    {
        if (!animator.GetBool("isAttack")) 
        {
            return;
        }
        Player player = collision.gameObject.GetComponent<Player>();
        if (player)
        {
            if (player.CompareLayerAndColliderMask(gameObject.layer))
            {
                player.ApplyDamage(damage);
                return;
            }
        }

    }

    protected override void OnDead()
    {
        animator.SetBool("isDeath", true);
    }

    private void chack()
    {
        if (!skeletonAnimation.skeleton.FlipX)
        {

            skeletonAnimation.skeleton.FlipX = true;
        }
        else
        {

            skeletonAnimation.skeleton.FlipX = false;
        }

    }
}
