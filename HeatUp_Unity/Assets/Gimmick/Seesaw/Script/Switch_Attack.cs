using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 攻撃時作動し、消滅するSwitch
/// </summary>
public class Switch_Attack : GimmickSwitch
{
    [SerializeField, TooltipAttribute("作動する剣の温度")] int possibleTemperature;
    public void onSwitch(Sword sword)
    {
        if (possibleTemperature <= sword.temperature)
        {
            foreach (Gimmick gim in gimmicks)
            {
                gim.OnJustSwitch();
            }
            Destroy(this.gameObject);
        }
    }
}
