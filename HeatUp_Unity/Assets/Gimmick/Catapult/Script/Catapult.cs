using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Catapult : Gimmick
{
    [SerializeField, TooltipAttribute("作動したあとの角度")]
    float shotAngle = 90;

    [SerializeField, TooltipAttribute("作動してから投げるまでの時間")]
    float rotaSpeed = 0.5f;

    [SerializeField, TooltipAttribute("投擲速度")]
    float power = 1000;

    GameObject spinPoint;
    GameObject rope;
    CatapultStand stand;

    bool switchOn;

    // Start is called before the first frame update
    void Start()
    {
        spinPoint = transform.Find("SpinPoint").gameObject;
        rope = transform.Find("Rope").gameObject;
        stand = spinPoint.transform.Find("Stand").gameObject.GetComponent<CatapultStand>();
    }

    // Update is called once per frame
    void Update()
    {
        if (switchOn)
        {
            if (spinPoint.transform.localRotation.z <= shotAngle / 180)
            {
                spinPoint.transform.Rotate(new Vector3(0, 0, (Time.deltaTime * shotAngle) / rotaSpeed));
            }
            else
            {
                switchOn = false;
                GameObject stone = spinPoint.transform.Find("Stand").gameObject.transform.Find("Stone").gameObject;
                stone.layer = this.gameObject.layer;
                stone.transform.parent = null;
                stone.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
                float rag = (spinPoint.transform.rotation.z * 180 + 90) * Mathf.Deg2Rad;
                stone.GetComponent<Rigidbody2D>().AddForce(new Vector2(Mathf.Cos(rag), Mathf.Sin(rag)) * power);
            }
        }
    }

    public override void OnJustSwitch()
    {
        switchOn = true;
    }
}
