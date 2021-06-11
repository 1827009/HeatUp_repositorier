using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UIElements;

public class Hati : MonoBehaviour
{
    // Start is called before the first frame update
    public int speed;
    public float turn;
    public float distance = 3;
    public int hp = 10;


    GameObject player;
    Player playerScript;
    private Animator animator;
    private float move = 1;
    private float pos;
    private float height;
    private bool moveFrge = true;
    private bool reset = false;
    private Vector3 Setpos;
    private Vector3 start;
    private Vector3 end;
    private Vector3 half;
    Spine.Unity.SkeletonAnimation skeletonAnimation;
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<Player>();
        animator = GetComponent<Animator>();
        Setpos = transform.position;
        skeletonAnimation = GetComponent<Spine.Unity.SkeletonAnimation>();

    }

    // Update is called once per frame
    void Update()
    {
        pos = transform.position.x - player.transform.position.x;
        if (pos < 0)
        {
            pos *= -1;
        }
        
        if (moveFrge)
        {
            if (pos < distance)
            {
                start = transform.position;
                end = player.transform.position;
                postionCheck();
                if (skeletonAnimation.skeleton.FlipX)
                {
                    end.x += 3.0f;
                }
                else
                {
                    end.x -= 3.0f;
                }
                start.y += 2.0f;
                height = -(end.y - start.y);
                Half();
                moveFrge = false;
                animator.SetBool("Attack", true);
            }
            else
            {
                Move();
            }
        }

        if (transform.position.y< player.transform.position.y)
        {
            animator.SetBool("Attack", false);
            reset = true;
        }
        Back();
    }

    public void ApplyDamage(int damage)
    {
        hp -= damage;
        if (hp <= 0) 
        {
            Destroy(gameObject);
        }
    }

    void Move()
    {
        direction(0);
        Vector3 Move = transform.position;
        Move.x += move * speed * Time.deltaTime;
        transform.position = Move;
    }
    void Back()
    {
        if (reset)
        {
            if (transform.position.y < start.y)
            {
              
                Vector3 Move = transform.position;
                Move.y += 1 * speed * Time.deltaTime;
                transform.position = Move;
            }
            else
            {
                moveFrge = true;
                reset = false;
            }
        }
    }

    void postionCheck()
    {
        if (transform.position.x <player.transform.position.x)
        {
            move = -1;
            skeletonAnimation.skeleton.FlipX = false;

        }
        else if (player.transform.position.x< transform.position.x)
        {
            move = 1;
            skeletonAnimation.skeleton.FlipX = true;

        }
    }
    void direction(int number)
    {
        if (transform.position.x - Setpos.x > turn)
        {
            move = -1;
           skeletonAnimation.skeleton.FlipX = true ;
            
        }
        else if (Setpos.x - transform.position.x > turn)
        {
            move = 1;
            skeletonAnimation.skeleton.FlipX = false;
            
        }
    }

    public void Half()
    {

        Vector3 half = end - start * 0.5f + start;
        half.y += Vector3.up.y + height;

        StartCoroutine(LerpPoint());
    }

    IEnumerator LerpPoint()
    {
        float startTime = Time.timeSinceLevelLoad;
        float rate = 0f;
        float freem = 120 / speed;
        while (true)
        {
            if (1.0f <= rate)
            {
                yield break;
            }

            float diff = Time.timeSinceLevelLoad - startTime;
            rate = diff / (freem / 60f);
            transform.position = Point(start, half, end, rate);

            yield return null;
        }
    }

    Vector3 Point(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        Vector3 a = Vector3.Lerp(p0, p1, t);
        Vector3 b = Vector3.Lerp(p1, p2, t);
        return Vector3.Lerp(a, b, t);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" 
            && playerScript.CompareLayerAndColliderMask(gameObject.layer)) 
        {
            playerScript.ApplyDamage(1);
            animator.SetBool("Attack", false);
            reset = true;
        }
    }
}
