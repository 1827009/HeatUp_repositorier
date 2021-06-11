using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSwitch : GimmickSwitch
{
    bool swicht_on = false;
    float alpha = 0;
    float speed = 3;
    float def = 0;

    // Start is called before the first frame update
    void Start()
    {
        def = transform.eulerAngles.z;
    }

    // Update is called once per frame
    void Update()
    {
        if(swicht_on&&alpha<1)
        {
            alpha += Time.deltaTime * speed;
            Vector3 rot = transform.eulerAngles;
            rot.z = Mathf.Lerp(def + 30, def - 30, alpha);
            transform.eulerAngles = rot;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!swicht_on)
        {
            if (gameObject.layer == collision.gameObject.layer)
            {
                Iron_Barrels iron_Barrels = collision.gameObject.GetComponent<Iron_Barrels>();
                if (iron_Barrels)
                    swicht_on = true;
                if (swicht_on)
                {
                    foreach (Gimmick gimmick in gimmicks)
                        gimmick.OnJustSwitch();
                }
            }
        }
    }
}
