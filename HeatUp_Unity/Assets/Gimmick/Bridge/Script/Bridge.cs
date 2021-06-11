using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bridge : MonoBehaviour
{
    public Vector2 size = Vector2.one;
    public float offset;
    public float breakTime = 0.5f;
    public LayerMask layerMask;

    Spine.Unity.SkeletonAnimation skeletonAnimation;
    PolygonCollider2D collider;
    Vector2 center;
    bool isBreak = false;
    float currentTime = 0;
    // Start is called before the first frame update
    void Start()
    {
        center = new Vector2(transform.position.x, transform.position.y + offset);
        skeletonAnimation = GetComponent<Spine.Unity.SkeletonAnimation>();
        collider = GetComponent<PolygonCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit2D hit = Physics2D.BoxCast(center, size, 0, Vector2.zero, 0, layerMask);
        if(hit)
        {
            Player player = hit.collider.GetComponent<Player>();
            if(!player.CompareLayerAndColliderMask(gameObject.layer))
            {
                return;
            }
            skeletonAnimation.timeScale = 1;
            isBreak = true;
        }
        if(isBreak)
        {
            currentTime += Time.deltaTime;
            if (currentTime >= breakTime) 
            {
                collider.enabled = false;
            }
        }
    }

    private void OnDrawGizmos()
    {
        center = new Vector2(transform.position.x, transform.position.y + offset);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(center, size);
    }
}
