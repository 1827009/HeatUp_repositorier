using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bee_script : Character
{
    public int speed;
    public float chaseSpeed = 4;
    public float turn;
    public float distance = 3;


    GameObject player;
    Player playerScript;
    private Animator animator;
    private float move = 1;
    private float pos;
    private float time;
    private bool moveFrge = true;
    private bool reset = false;
    private bool set = true;
    private bool chaseON = false;
    private Vector3 Setpos;
    private Vector3 start;
    private Vector3 end;
    private Vector3 half;
    Spine.Unity.SkeletonAnimation skeletonAnimation;
    protected override void Start()
    {
        base.Start();//消さないでください。

        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<Player>();
        animator = GetComponent<Animator>();
        Setpos = transform.position;
        skeletonAnimation = GetComponent<Spine.Unity.SkeletonAnimation>();
    }

    protected override void Update()
    {
        base.Update();//消さないでください。

        if (hp <= 0)
        {
            return;
        }
        pos = transform.position.x - player.transform.position.x;
        if (pos < 0)
        {
            pos *= -1;
        }

        if (moveFrge)
        {
            if (pos < distance && playerScript.CompareLayerAndColliderMask(gameObject.layer))
            {

                attack();
            }
            else
            {
                Move();
            }
        }

        if (transform.position.y < player.transform.position.y)
        {
            chaseON = true;
            moveFrge = false;
        }
        if (chaseON)
        {
            Chase();
        }
        Back();
    }

    protected override void OnDead()
    {
        animator.SetBool("isDead", true);
        base.OnDead();
    }

    void Move()
    {
        direction();
        Vector3 Move = transform.position;
        Move.x += move * speed * Time.deltaTime;
        transform.position = Move;
    }

    void attack()
    {
        Vector3 set_pos;

        if (set)
        {
            time = 0;
            start = transform.position;
            end = player.transform.position;
            half.x = (start.x + end.x) / 2;
            half.y = end.y;
            set = false;
        }
        time += Time.deltaTime;
        postionCheck();

        set_pos = Point(start, half, end, time);
        set_pos.z = Setpos.z;
        transform.position = set_pos;

    }
    void Back()
    {
        if (reset)
        {
            if (transform.position.y < Setpos.y)
            {
                animator.SetBool("Attack", false);
                Vector3 Move = transform.position;
                Move.y += 1 * speed * Time.deltaTime;
                transform.position = Move;
            }
            else
            {
                moveFrge = true;
                reset = false;
                chaseON = false;
                animator.SetBool("Attack", false);

            }
        }
    }

    void postionCheck()
    {
        if (transform.position.x < player.transform.position.x)
        {

            skeletonAnimation.skeleton.FlipX = false;

        }
        else if (player.transform.position.x < transform.position.x)
        {

            skeletonAnimation.skeleton.FlipX = true;

        }
    }
    void direction()
    {
        if (transform.position.x - Setpos.x > turn)
        {
            move = -1;
        }
        else if (Setpos.x - transform.position.x > turn)
        {
            move = 1;
        }

        if (move == 1)
        {
            skeletonAnimation.skeleton.FlipX = false;
        }
        else
        {
            skeletonAnimation.skeleton.FlipX = true;
        }
    }

    Vector3 Point(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        Vector3 a = Vector2.Lerp(p0, p1, t);
        Vector3 b = Vector2.Lerp(p1, p2, t);
        return Vector3.Lerp(a, b, t);
    }

    public override void OnTriggerEnter2DOfAttacher(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player"
                    && playerScript.CompareLayerAndColliderMask(gameObject.layer))
        {
            playerScript.ApplyDamage(1);
            animator.SetBool("Attack", true);
            moveFrge = false;
            chaseON = true;
        }

    }

    private void Chase()
    {
        if (pos < 10)
        {
            postionCheck();
            Vector3 Move = transform.position;
            if (skeletonAnimation.skeleton.FlipX)
            {
                if (player.transform.position.x + 1 < transform.position.x)
                {
                    move = -1;
                    Move.x += move * chaseSpeed * Time.deltaTime;
                }
            }
            else
            {
                if (transform.position.x < player.transform.position.x - 1)
                {
                    move = 1;
                    Move.x += move * chaseSpeed * Time.deltaTime;
                }
            }
            Move = UpDown(Move);
            transform.position = Move;
        }
        else
        {
            reset = true;
            set = true;
        }

    }

    private Vector3 UpDown(Vector3 Move)
    {

        if (transform.position.y < player.transform.position.y)
        {
            Move.y += 1 * speed * 4 * Time.deltaTime;
        }
        else if (transform.position.y > player.transform.position.y)
        {
            Move.y += -1 * speed * 4 * Time.deltaTime;
        }

        return Move;
    }
}
