using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class spore : MonoBehaviour
{
    public float speed = 1;
    public int DesTime = 3;


    Player playerScript;
    private GameObject player;
    private float move_x = 1;
    private float countup = 0.0f;
    private int ST = 0;
    private int S;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<Player>();
        if (player.transform.position.x < transform.position.x)
        {
            move_x = -1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        countup += Time.deltaTime;
        S = (int)countup;
        shot();
        if (transform.position.x < player.transform.position.x)
        {

            if (S - ST == DesTime)
            {
                Destroy(gameObject);
            }
        }
    }

    void shot()
    {

        Vector3 nowPos;
        nowPos = transform.position;
        nowPos.x += move_x * speed * Time.deltaTime;
        transform.position = nowPos;
    }

    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && playerScript.CompareLayerAndColliderMask(gameObject.layer))
        {
            playerScript.moveSpeed /= 2;
        }
        Destroy(gameObject);
    }
}
