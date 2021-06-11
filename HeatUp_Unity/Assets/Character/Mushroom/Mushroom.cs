using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mushroom : Character
{
    public GameObject spore_shot;
    public Transform spawnPoint;
    public float distance = 10;
    public float SlowEffectTime = 2;
    public int SponeTime = 2;
    

    Spine.Unity.SkeletonAnimation skeletonAnimation;
    Player playerScript;
    private GameObject play;
    private GameObject appearance;
    private float count;
    private float pos;
    private float slowTime = 0;
    private float playerSpeed_old;
    private int time;
    private int ST = 0;
    private bool slow = false;
    protected override void Start()
    {
        base.Start();//消さないでください。
        play = GameObject.FindWithTag("Player");
        playerScript =play.GetComponent<Player>();
        count += Time.deltaTime;
        time = (int)count;
        ST = time;
        skeletonAnimation = GetComponent<Spine.Unity.SkeletonAnimation>();
        skeletonAnimation.skeleton.FlipX = false;
        //ここから処理を書いてください。
    }

    protected override void Update()
    {
        base.Update();//消さないでください。

        //ここから処理を書いてください。
        count += Time.deltaTime;
        time = (int)count;
        Vector3 player = play.transform.position;
        if (player.x < 0)
        {
            player *= -1;
        }
        pos = transform.position.x - player.x;
        if (pos < distance)
        {

            if (time - ST > SponeTime)
            {
                if (player.x < transform.position.x && !skeletonAnimation.skeleton.FlipX)
                {
                    Appearance();
                    ST = time;
                }
            }
        }
        Slow();
        SlowCheck();
        
       
    }


    //キャラクターのhpが0になった時にDestroy(gameObject)以外の処理を書きたい場合はコメントを解除してください。
    /*
    protected override void DestroyCharacter()
    {
        Debug.Log("A");
        //base.DestroyCharacter();//Destroyしたくない場合は消去する。
    }
    */

    
    private void Appearance()
    {
        if (!slow)
        {
            appearance = spore_shot;

            GameObject newAppearance1 = Instantiate(appearance, spawnPoint.position, Quaternion.identity) as GameObject;
            newAppearance1.GetComponent<Rigidbody2D>();
        }
    }

    private void SlowCheck()
    {
        if (slow)
        {
            float T;
            T = Time.time - slowTime;
            if (SlowEffectTime<T)
            {
                slow = false;
                playerScript.moveSpeed *=2;
            }
        }
        
    }

    private void Slow()
    {
        if (!slow&&playerScript.moveSpeed<playerSpeed_old)
        {
            slow = true;
            slowTime = Time.time;
        }
        playerSpeed_old = playerScript.moveSpeed;
    }


}
