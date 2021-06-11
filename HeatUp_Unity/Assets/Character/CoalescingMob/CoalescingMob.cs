using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoalescingMob : MonoBehaviour
{
    [SerializeField, TooltipAttribute("合体する対象の範囲")]
    float searchRange = 3;
    [SerializeField, TooltipAttribute("合体後のオブジェクト")]
    GameObject perfectBody;
    [SerializeField, TooltipAttribute("合体に必要な個体数")]
    int CoalescingCount = 5;

    // [HideInInspector]
    public bool leaderFlag;
    List<CoalescingMob> subordinateObjcts;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit2D[] search = Physics2D.CircleCastAll(transform.position, searchRange, Vector2.zero);
        LeaderDecision(search);
        Coalescing(search);
    }

    void LeaderDecision(RaycastHit2D[] search)
    {
        // 範囲内に入ったCoalescingSlimeControllerを探す
        bool notReaderFlag = false;

        foreach (RaycastHit2D hit in search)
        {
            CoalescingMob coalescingSlime = hit.collider.GetComponent<CoalescingMob>();

            // 群れにリーダーがいるか否か。いたら記録
            if (coalescingSlime && coalescingSlime.leaderFlag)
            {
                if (coalescingSlime.gameObject != gameObject)
                    notReaderFlag = true;
            }
        }
        // リーダーがいなければリーダー化
        if (!notReaderFlag)
        {
            leaderFlag = true;
            subordinateObjcts = new List<CoalescingMob>(5);
        }
        else
        {
            leaderFlag = false;
        }
    }

    /// <summary>
    /// 5体そろってから合体するまでの演出時間
    /// </summary>
    float coalescingTime = 1;

    void Coalescing(RaycastHit2D[] search)
    {
        if (leaderFlag)
        {
            foreach (RaycastHit2D hit in search)
            {
                // 周囲の融合対象を数える
                CoalescingMob coalescingMob = hit.collider.GetComponent<CoalescingMob>();
                if (coalescingMob && subordinateObjcts.Count < CoalescingCount)
                {
                    subordinateObjcts.Add(coalescingMob);
                }
            }
            // 規定数以上いるなら
            if (subordinateObjcts.Count == CoalescingCount)
            {
                // 引き寄せ演出
                if (coalescingTime > 0)
                {
                    coalescingTime -= Time.deltaTime;
                    foreach (CoalescingMob slime in subordinateObjcts)
                    {
                        slime.GetComponent<Rigidbody2D>().velocity = (transform.position - slime.gameObject.transform.position);
                    }
                }
                // 合体・融合
                else
                {
                    // 合体後のプレハブを召喚、合体前のプレハブを消去
                    GameObject ps = Instantiate(perfectBody);
                    MeshRenderer mr = ps.GetComponent<MeshRenderer>();
                    mr.sortingOrder = 3;
                    ps.transform.position = transform.position;
                    ps.layer = gameObject.layer;
                    foreach (CoalescingMob coalescongMobs in subordinateObjcts)
                    {
                        Destroy(coalescongMobs.gameObject);
                    }
                }
            }
            else
                subordinateObjcts.Clear();
        }
    }
}
