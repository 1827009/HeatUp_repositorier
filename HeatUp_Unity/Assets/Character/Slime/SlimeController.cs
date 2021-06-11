using UnityEngine;

public class SlimeController : Character
{
    public int mininterval=3;
    public int maxinterval=10;
    public int StopTime = 2;
    public float minJumpForce = 1f;
    public float maxJumpForce = 5f;
    public float minSpeed = 0.5f;
    public float maxSpeed = 3f;
    public float rangeAffectedByWaterVapor = 5;
    public float ignoreCollisionSize = 2;

    GameObject player;
    Player playerScript;
    private float jumpForce;
    private float speed;
    public int interval;
    private LayerMask platformsLayerMask;
    private CircleCollider2D collider;
    private float moveInput;
    private float countup = 0.0f;
    private int setSeve = 0;
    private int ST = 0;
    private int S;
    private bool a = false;
    private bool moveF = true;
    private bool jump = true;
    private bool start = true;
    private bool attack = false;
    private Animator animator;
    private LayerMask playerMask;
    private CircleCollider2D collider2D;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();//消さないでください。

        //ここから処理を書いてください。
        player = GameObject.FindWithTag("Player");
        playerMask = 1 << player.layer;
        playerScript = player.GetComponent<Player>();
        collider = GetComponent<CircleCollider2D>();
        interval= Random.Range(mininterval, maxinterval);
        moveInput = Random.Range(-1, 1);
        if (moveInput < 0)
        {
            moveInput = -1;
        }
        else
        {
            moveInput = 1;
        }
        string str = LayerMask.LayerToName(gameObject.layer);
        platformsLayerMask = LayerMask.GetMask(str);
        speed = Random.Range(minSpeed, maxSpeed);
        jumpForce= Random.Range(minJumpForce, maxJumpForce);
        Move();
        start = false;
        animator = GetComponent<Animator>();
        collider2D = GetComponent<CircleCollider2D>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();//消さないでください。

        //ここから処理を書いてください。
       
        countup += Time.deltaTime;
        S = (int)countup;
        if (IsGrounded() && jump == false) {
            ST = S;
            moveF = false;
            attack = false;
        }
        if (S - ST == StopTime) {
            moveF = true;
        }

        if (moveF)
        {
            Move();
        }
        jump = IsGrounded();
        Attack();
    }

    //キャラクターのhpが0になった時にDestroy(gameObject)以外の処理を書きたい場合はコメントを解除してください。
    
    protected override void OnDead()
    {
        if(animator)
        {
            animator.SetBool("isMelt", true);
        }
        base.OnDead();
    }

    protected override void OnDamage()
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, rangeAffectedByWaterVapor, Vector2.zero, 0, platformsLayerMask);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit)
            {
                Plant Plant = hit.collider.GetComponent<Plant>();
                if (Plant)
                {
                    Plant.ApplyGrowth(1);
                }
            }
        }
    }

    private void Move()
    {
        int T = (int)countup;
        if (a && T - setSeve >= interval && IsGrounded()||start)
        {
            moveInput *= -1;
            setSeve = T;
            a = false;
        }
        else if (T - setSeve >= interval && !a && IsGrounded()||start)
        {
            moveInput *= -1;
            setSeve = T;
            a = true;
        }



        Vector3 pos = transform.position;
        pos.x += moveInput * speed * Time.deltaTime;
        transform.position = pos;
        transform.Rotate(new Vector3(0, 0, 0));

        Jump();
    }

    private void Jump()
    {
        if (IsGrounded()||start)
        {
            rigidbody2D.velocity = Vector2.up * jumpForce;
        }
    }

    private bool oldHit = false;
    private void Attack()
    {
        if (IsGrounded()) 
        {
            return;
        }
        RaycastHit2D hit = Physics2D.CircleCast(collider2D.bounds.center, collider2D.bounds.size.x, Vector2.zero, 0, playerMask);
        if (hit && !oldHit) 
        {
            oldHit = true;
            playerScript.ApplyDamage(1);
        }
        if(!hit)
        {
            oldHit = false;
        }

    }

    protected override void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 1, 0, 0.3f);
        Gizmos.DrawWireSphere(transform.position, rangeAffectedByWaterVapor);
        base.OnDrawGizmos();
    }

    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Player"
            && playerScript.CompareLayerAndColliderMask(gameObject.layer) &&IsGrounded()&&!attack)
        {
            playerScript.ApplyDamage(1);
            attack = true;
        }
    }
}
