using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowingPlayer : MonoBehaviour
{
    /// <summary>
    /// 追従する
    /// </summary>
    Player player;

    [SerializeField, TooltipAttribute("キャラからどの位置に配置するか")]
    Vector3 localPosition;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player.CompareLayerAndColliderMask(gameObject.layer))
        {
            transform.position = player.transform.position + localPosition;
        }
    }
}
