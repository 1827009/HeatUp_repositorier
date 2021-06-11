using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GimmickSwitch : MonoBehaviour
{
    [SerializeField, TooltipAttribute("このスイッチによって作動するGimmick")] protected Gimmick[] gimmicks;
}
