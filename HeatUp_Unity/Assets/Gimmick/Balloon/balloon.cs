using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class balloon : MonoBehaviour
{
    public float speed = 1;
    public int height=10;
    public LayerMask playerMask;

    Player playerScript;
    GameObject player;
    private Vector3 freez;
    private Vector3 end;
    private Vector3 pPos;
    float ver_y;
    float ver_p;
    float startTime=0;
    private BoxCollider2D boxCollider2d;
    private bool set = false;
    private float diff;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<Player>();
        boxCollider2d = GetComponent<BoxCollider2D>();
        freez = transform.position;
        end = new Vector3(transform.position.x, transform.position.y + height, transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        Hit();
        move();
    }

    void move()
    {
        if (set)
        {
            float alpha = (transform.position.y - freez.y) / height;
            alpha += speed * 0.1f * Time.deltaTime;
            if (alpha <= 1)
            {
                Vector3 pos = Vector3.Lerp(freez, end, alpha);
                transform.position = pos;
                pPos = player.transform.position;
                pPos.y = transform.position.y- diff;
                player.transform.position = pPos;
            }
        }
        else
        {
            if (freez.y < transform.position.y)
            {
                Vector3 move = transform.position;
                move.y -= 1 * 1 * Time.deltaTime;
                transform.position = move;
            }
        }
    }

    bool oldHit = false;
    void Hit()
    {
        RaycastHit2D hit = Physics2D.BoxCast(boxCollider2d.bounds.center, boxCollider2d.bounds.size, 0, Vector2.zero, 0, playerMask);
        if (hit && !oldHit)
        {
            if (playerScript.CompareLayerAndColliderMask(gameObject.layer))
            {
                oldHit = true;
                playerScript.ignoreGravity = true;
                set = true;
                diff = transform.position.y - player.transform.position.y;
            }
        }
        if (!hit && oldHit)
        {
            if (playerScript.CompareLayerAndColliderMask(gameObject.layer))
            {
                oldHit = false;
                playerScript.ignoreGravity = false;
                set = false;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(0, height, 0));
    }
}
