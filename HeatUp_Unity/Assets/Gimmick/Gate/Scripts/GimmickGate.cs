using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GimmickGate : Gimmick
{
    [SerializeField, TooltipAttribute("ギミックが起動した際の移動先")] public Vector3 localWaypoints;

    float movePercentage = 0;
    bool gimmick_on;

    // Start is called before the first frame update
    void Start()
    {
        localWaypoints += this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (gimmick_on && movePercentage < 1)
        {
            movePercentage += Time.deltaTime;
            this.transform.position = Vector3.Lerp(this.transform.position, localWaypoints, movePercentage);
        }
    }

    public override void OnJustSwitch()
    {
        if (!gimmick_on)
        {
            gimmick_on = true;
        }

    }
}
