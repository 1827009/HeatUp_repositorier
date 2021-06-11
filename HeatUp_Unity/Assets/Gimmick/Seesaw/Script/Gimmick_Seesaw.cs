using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gimmick_Seesaw : Gimmick
{
    [TooltipAttribute("シーソーの速度")]
    public float speed = 20;
    [TooltipAttribute("動作させるレイヤー")]
    public LayerMask layerMask;

    Vector2 bSize;        //boundsSize
    Vector2 ufPos;        //回転させるための当たり判定 場所
    Vector2 ufSize;       //回転させるための当たり判定 サイズ
    bool operate = false; //動作可能か
    bool right = false;   //右側が地面についていたらtrue
    bool left = false;    //右側が地面についていたらtrue

    // Start is called before the first frame update
    void Start()
    {
        bSize = GetComponent<BoxCollider2D>().bounds.size;
        ufSize = new Vector2(bSize.x * 0.9f, bSize.y * 0.8f);
        ufPos = new Vector2(transform.position.x, transform.position.y + bSize.y / 2);
    }

    private void Update()
    {
        UnFreezeHit();
    }

    void UnFreezeHit()
    {
        if(!operate)
        {
            return;
        }
        RaycastHit2D hit = Physics2D.BoxCast(ufPos, ufSize, transform.rotation.eulerAngles.z, Vector2.zero, 0, layerMask);
        if(hit)
        {
            bool direction = transform.position.x - hit.point.x < 0;
            float distance = Vector2.Distance(transform.position, hit.point);
            float max = ufSize.x * 0.5f;
            float alpha = distance / max;
            float velocity = Mathf.Lerp(0, speed, alpha);
            if (!(direction ? right : left)) 
            {
                Vector3 rot = transform.eulerAngles;
                rot.z += direction ? -velocity * Time.deltaTime : velocity * Time.deltaTime;
                transform.eulerAngles = rot;
            }
        }
    }

    public override void OnJustSwitch()
    {
        operate = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == gameObject.layer) 
        {
            bool direction = transform.position.x - collision.GetContact(0).point.x < 0;
            right = direction ? true : false;
            left = direction ? false : true;

        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == gameObject.layer)
        {
            right = false;
            left = false;
        }
    }

    void OnDrawGizmos()
    {
        Vector2 bSize = GetComponent<SpriteRenderer>().size;
        Vector2 ufSize = new Vector2(bSize.x * 0.9f, bSize.y * 0.8f);
        Vector2 ufPos = new Vector2(0, bSize.y / 2);
        Gizmos.color = Color.green;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(ufPos, ufSize);
    }
}
