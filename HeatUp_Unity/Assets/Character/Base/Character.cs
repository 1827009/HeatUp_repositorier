using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Character : MonoBehaviour
{
    [Header("Character")]
    [TooltipAttribute("体力の初期値")]
    public int maxHp = 100;

    [TooltipAttribute("地面に着地しているかどうかを判断するためのオフセット")]
    public Vector2 groundCheckOffset = new Vector2(0, -1);

    [TooltipAttribute("HealthBarの位置")]
    public Vector2 healthBarOffset = new Vector2(0, 2);

    [TooltipAttribute("同じキャラクター同志の当たり判定とプレイヤーとの当たり判定を無視する")]
    public bool ignoreCollision = false;

    [TooltipAttribute("IgnoreColliderの大きさ")]
    public float ignoreSize = 5;

    [HideInInspector]
    public int hp;

    [HideInInspector]
    public Rigidbody2D rigidbody2D;

    private float changeColorTime = 0.1f;
    private LayerMask layerMask;
    private LayerMask playerMask;
    private MaterialPropertyBlock block;
    private MeshRenderer meshRenderer;
    private HealthBar healthBar;
    private float fadeOutSpeed = 1;
    private GameObject particlePrefab;
    private Collider2D[] collider2D;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        hp = maxHp;
        rigidbody2D = GetComponent<Rigidbody2D>();
        layerMask = 1 << gameObject.layer;
        playerMask = 1 << LayerMask.NameToLayer("Player");
        meshRenderer = GetComponent<MeshRenderer>();
        if(meshRenderer)
        {
            block = new MaterialPropertyBlock();
        }
        GameObject prefab = (GameObject)Resources.Load("Character/HealthBar");
        GameObject hpb = Instantiate(prefab, gameObject.transform);
        Vector3 pos = transform.position;
        pos += new Vector3(healthBarOffset.x, healthBarOffset.y, 0);
        hpb.transform.position = pos;
        healthBar = hpb.GetComponent<HealthBar>();
        particlePrefab = (GameObject)Resources.Load("Character/Particle");
        collider2D = GetComponents<Collider2D>();
        GameObject shadowPrefab = (GameObject)Resources.Load("Character/Shadow");
        shadow = Instantiate(shadowPrefab, transform).GetComponent<SpriteRenderer>();
        Vector2 size = shadow.transform.localScale;
        size.x = GetComponent<MeshFilter>().mesh.bounds.size.x * 0.8f;
        shadow.transform.localScale = size;
        defAlpha = shadow.color.a;
        shadow.sortingOrder = GetComponent<MeshRenderer>().sortingOrder - 1;
        Color color = shadow.color;
        if (!IsGrounded())
        {
            color.a = 0;
        }
        shadow.color = color;
        StartCoroutine(Fade(true));
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if(ignoreCollision)
        {
            IgnoreCollision();
        }
        if (IsGrounded() && !oldIsGrounded)
        {
            StartCoroutine(Shadow(true));
        }
        if(!IsGrounded() && oldIsGrounded)
        {
            StartCoroutine(Shadow(false));
        }
        oldIsGrounded = IsGrounded();
    }

    /// <summary>
    /// キャラクターが着地しているか
    /// </summary>
    public bool IsGrounded()
    {
        Vector2 pos = transform.position;
        pos += groundCheckOffset;
        RaycastHit2D hit = Physics2D.CircleCast(pos, 0.1f, Vector2.zero, 0, layerMask);
        if (hit)
        {
            return hit.collider.tag == "Ground";
        }
        return false;
    }

    /// <summary>
    /// キャラクターのhpが0になった時の処理。
    /// 子クラスのoverrideで処理を記述することにより、別の処理を組むことが可能。
    /// 書かない場合はDestroy(gameObject)が呼ばれる。
    /// </summary>
    protected virtual void OnDead()
    {
        GameObject particle = Instantiate(particlePrefab);
        particle.transform.position = transform.position;
        if (meshRenderer)
        {
            StartCoroutine(Fade(false));
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private SpriteRenderer shadow;
    private float shadowSpeed = 5;
    private float defAlpha;
    private bool oldIsGrounded;

    private IEnumerator Shadow(bool fadeIn)
    {
        float alpha = defAlpha;
        while (alpha > 0)
        {
            alpha -= shadowSpeed * Time.deltaTime;
            Color color = shadow.color;
            color.a= fadeIn ? defAlpha - alpha : alpha;
            shadow.color = color;
            yield return null;
        }
        yield break;
    }

    private IEnumerator Fade(bool fadeIn)
    {
        float alpha = 1;
        while (alpha > 0) 
        {
            alpha -= fadeOutSpeed * Time.deltaTime;
            Color color = Color.white;
            alpha = Mathf.Clamp(alpha, 0f, 1f);
            color.a = fadeIn ? 1f - alpha : alpha;
            block.SetColor("_Color", color);
            meshRenderer.SetPropertyBlock(block);
            yield return null;
        }
        if(!fadeIn)
        {
            Destroy(gameObject);
        }
        yield break;
    }

    /// <summary>
    /// キャラクターのhpにダメージを与える処理。
    /// <param name="damage">与えるダメージ</param>
    /// </summary>
    public void ApplyDamage(int damage)
    {
        if (hp <= 0)
        {
            return;
        }
        if(damage == 0)
        {
            StartCoroutine(SetColor(Color.gray));
            return;
        }
        hp -= damage;
        if (hp < 0)
        {
            hp = 0;
        }
        healthBar.UpdateBar();
        OnDamage();
        if (meshRenderer)
        {
            StartCoroutine(SetColor(Color.red));
        }
        if (hp <= 0) 
        {
            hp = 0;
            OnDead();
        }
    }

    /// <summary>
    /// ダメージが与えられた時の処理
    /// </summary>
    protected virtual void OnDamage()
    {
        
    }

    private IEnumerator SetColor(Color color)
    {
        block.SetColor("_Color", color);
        meshRenderer.SetPropertyBlock(block);
        yield return new WaitForSeconds(changeColorTime);
        block.SetColor("_Color", Color.white);
        meshRenderer.SetPropertyBlock(block);
        yield break;
    }

    private void IgnoreCollision()
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, ignoreSize, Vector2.zero, 0, layerMask);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit)
            {
                if (hit.collider.tag != "Ground")
                {
                    foreach(Collider2D c in collider2D)
                    {
                        if (hit.collider && c)
                        {
                            Physics2D.IgnoreCollision(hit.collider, c);
                        }
                    }
                }
            }
        }
        RaycastHit2D phit = Physics2D.CircleCast(transform.position, ignoreSize, Vector2.zero, 0, playerMask);
        if (phit)
        {
            foreach (Collider2D c in collider2D)
            {
                if (phit.collider && c)
                {
                    Physics2D.IgnoreCollision(phit.collider, c);
                }
            }
        }
    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 pos = transform.position;
        Vector3 cPos = groundCheckOffset;
        pos += cPos;
        Gizmos.DrawWireSphere(pos, 0.1f);
        Gizmos.color = Color.green;
        pos = transform.position;
        cPos = healthBarOffset;
        pos += cPos;
        Gizmos.DrawWireSphere(pos, 0.1f);
        if (ignoreCollision)
        {
            Gizmos.color = new Color(0, 0, 1, 0.3f);
            Gizmos.DrawWireSphere(transform.position, ignoreSize);
        }
    }

    public virtual void OnTriggerEnter2DOfAttacher(Collider2D collision)
    {

    }

    public virtual void OnTriggerStay2DOfAttacher(Collider2D collision)
    {

    }

    public virtual void OnTriggerExit2DOfAttacher(Collider2D collision)
    {

    }
}
