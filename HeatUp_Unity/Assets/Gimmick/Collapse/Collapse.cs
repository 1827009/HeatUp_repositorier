using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collapse : MonoBehaviour
{
    public LayerMask layerMask;
    public float offset;
    public Vector2 size = Vector2.one;
    public float timeLimit = 1;
    public float fallSpeed = 0.2f;
    public float destroyTime = 1;
    float now;
    Vector2 center;
    bool fall;

    // Start is called before the first frame update
    void Start()
    {
        fall = false;
        center = new Vector2(transform.position.x, transform.position.y + offset);
        now = 0;
    }

    //// Update is called once per frame
    void Update()
    {
        RaycastHit2D hit = Physics2D.BoxCast(center, size, 0, Vector2.zero, 0, layerMask);
        if (hit)
        {
            Player player = hit.collider.GetComponent<Player>();
            if(!player.CompareLayerAndColliderMask(gameObject.layer))
            {
                return;
            }
            now += Time.deltaTime;
            if (now > timeLimit)
            {
                fall = true;
                Destroy(gameObject, destroyTime);
            }
        }
        else if (!hit) { now = 0; }
        if (fall)
        {
            Vector3 pos = this.transform.position;
            pos.y -= fallSpeed;
            this.transform.position = pos;
        }
    }

    private void OnDrawGizmos()
    {
        center = new Vector2(transform.position.x, transform.position.y + offset);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(center, size);
    }
}
