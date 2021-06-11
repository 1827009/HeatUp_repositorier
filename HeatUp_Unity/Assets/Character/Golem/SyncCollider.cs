using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncCollider : MonoBehaviour
{
    [TooltipAttribute("同期させるコライダー")]
    public Collider2D collider;

    [TooltipAttribute("同期させるトランスフォーム")]
    public Transform target;

    private Character master;

    // Start is called before the first frame update
    void Start()
    {
        master = GetComponent<Character>();
    }

    // Update is called once per frame
    void Update()
    {
        collider.offset = target.position - transform.position;
    }
}
