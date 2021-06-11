using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Barrels : MonoBehaviour
{
    public float activateTime = 0.5f;
    public GameObject effect;

    public Sprite lid;
    public GameObject vfx;
    float v0;
    float destroyTime;

    private Rigidbody2D rigidbody2D;
    SpriteRenderer spriteRenderer;
    bool taru_jump;
    bool player_jump;
    GameObject player;
    Player playerScript;
    float player_hight;
    float barrels_hight;
    public float Gravity = 9.8f;
    public float min_y;
    float[] now = new float[2];
    Controller2D controller;
    float start_pos_y;
    Vector3 player_pos;
    public float playerJumpPower = 0.7f;
    public float taruJumpPower = 1.5f;
    // Start is called before the first frame update
    void Start()
    {
        controller = gameObject.GetComponent<Controller2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        taru_jump = false;
        player_jump = true;
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<Player>();
        rigidbody2D = this.gameObject.GetComponent<Rigidbody2D>();
        rigidbody2D.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        player_hight = player.GetComponent<BoxCollider2D>().bounds.size.y;
        barrels_hight = this.gameObject.GetComponent<SpriteRenderer>().bounds.size.y;
    }

    private void Update()
    {
        if (taru_jump)
        {
            if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)))
            {
                playerScript.inputStop = false;
                playerScript.TaruJump(playerJumpPower);

                player_jump = false;
                //playerScript.ignoreGravity = false;
            }
            now[0] += Time.deltaTime;
            Vector3 B_pos = this.transform.position;
            Vector3 P_pos = player.transform.position;
            B_pos.y = ((v0 * now[0]) - (Gravity * now[0] * now[0]) / 2) + start_pos_y;
            if (player_jump) {
                P_pos = new Vector3(B_pos.x, B_pos.y + ((barrels_hight) / 2), B_pos.z);
                player.transform.position = P_pos;
            }
            this.transform.position = B_pos;

        }

    }

    void OnDestroy()
    {
        playerScript.inputStop = false;
        playerScript.ignoreGravity = false;
    }

    IEnumerator StartApplyDamage(float temperature)
    {
        yield return new WaitForSeconds(activateTime);
        float pad = Input.GetAxisRaw("Horizontal");
        Sword sword = player.GetComponent<Sword>();
        sword.UpdateTemperature(-200);
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.LeftArrow) ||
            pad != 0) 
        {
            Damage();
        }
        else
        {
            Damage(temperature);
        }
        yield break;
    }

    bool applyDamage = false;

    public void ApplyDamage(float temperature)
    {
        if(applyDamage)
        {
            return;
        }

        applyDamage = true;
        StartCoroutine(StartApplyDamage(temperature));
    }

    public void CancelDamage(int temperature)
    {
        StopCoroutine(StartApplyDamage(temperature));
    }

    void Damage(float temperature)
    {
        spriteRenderer.sprite = lid;
        GameObject g = Instantiate(vfx);
        g.transform.position = transform.position;
        taru_jump = true;
        
        playerScript.inputStop = true;
        playerScript.ignoreGravity = true;

        float max_y = min_y + (barrels_hight * temperature );
        v0 = Mathf.Sqrt(2*Gravity*max_y) ;
        destroyTime = v0/Gravity;
        start_pos_y = this.transform.position.y;
        Instantiate(effect,this.transform.position,Quaternion.identity);
        GameObject.Destroy(this.gameObject, destroyTime);
    }

    void Damage()
    {
        GameObject g = Instantiate(effect, transform.position, Quaternion.identity);
        playerScript.controller.collisions.below = false;
        playerScript.TaruJump(taruJumpPower);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" &&
            playerScript.CompareLayerAndColliderMask(gameObject.layer) &&
            collision.transform.position.y > this.transform.position.y)
        {
            Vector3 pos = collision.transform.position;
            pos.x = this.transform.position.x;
            collision.transform.position = pos;
        }
    }
  
}
