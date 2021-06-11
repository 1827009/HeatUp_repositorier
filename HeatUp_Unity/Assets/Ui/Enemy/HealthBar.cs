using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [TooltipAttribute("ヘルスバーの長さの基準となる値（キャラクターのmaxHp）")]
    public int basis = 200;

    [TooltipAttribute("ヘルスバーの長さの制限値（キャラクターのmaxHp）")]
    public int horizontalMax = 1000;

    [TooltipAttribute("ヘルスバーの縦の長さの最大値（キャラクターのmaxHp）")]
    public int verticalMax = 10000;

    [TooltipAttribute("縦のサイズが更新される間隔")]
    public int verticalInterval = 2000;

    [TooltipAttribute("hpが最小値の時の色")]
    public Color minColor;

    [TooltipAttribute("hpが最大値の時の色")]
    public Color maxColor;

    Character master;
    Transform scaler;
    SpriteRenderer sprite;

    // Start is called before the first frame update
    void Start()
    {
        scaler = transform.GetChild(1);
        sprite = scaler.GetChild(0).GetComponent<SpriteRenderer>();
        FindMaster();
        SetScale();
        SetColor();
        UpdateBar();
    }

    private void FindMaster()
    {
        Transform parent = transform.parent;
        master = parent.GetComponent<Character>();
        for (int i = 0; i < parent.childCount; i++)
        {
            if (master)
            {
                break;
            }
            master = parent.GetChild(i).GetComponent<Character>();
        }
    }

    private void SetScale()
    {
        Vector3 scale = transform.localScale;
        if (master.maxHp >= horizontalMax)
        {
            scale.x = 1000f / (float)basis;
            if (master.maxHp > verticalMax) 
            {
                scale.y = 10000f / (float)basis;
            }
            else
            {
                int multiplier = (master.maxHp - horizontalMax) / verticalInterval;
                scale.y = multiplier + 1;
            }
        }
        else
        {
            scale.x = (float)master.maxHp / (float)basis;
        }
        transform.localScale = scale;
    }

    private void SetColor()
    {
        if (horizontalMax > master.hp) 
        {
            sprite.color = minColor;
            return;
        }
        float alpha = (float)(master.hp - horizontalMax) / (verticalMax - horizontalMax);
        sprite.color = Color.Lerp(minColor, maxColor, alpha);
    }

    public void UpdateBar()
    {
        Vector3 scale = scaler.transform.localScale;
        scale.x = (float)master.hp / (float)master.maxHp;
        scaler.transform.localScale = scale;
    }
}
