using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TemperatureGage : MonoBehaviour
{
    [TooltipAttribute("hpバーを光らせる間隔")]
    public float glowSpeed = 1;

    Player player;
    Sword sword;
    //[SerializeField, TooltipAttribute("熱ダメージを受けているときのParticle")]
    //GameObject particle;
    bool particleOn;
    List<RectTransform> tempes;

    Image hpImage;
    Outline outline;

    // Start is called before the first frame update
    void Start()
    {
        ;
        hpImage = transform.GetChild(2).gameObject.GetComponent<Image>();
        outline = transform.GetChild(2).gameObject.GetComponent<Outline>();

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        sword = GameObject.FindGameObjectWithTag("Player").GetComponent<Sword>();

        tempes = new List<RectTransform>();
        FindTemperatureMasters();
        UpdateTemperature();
    }

    // Update is called once per frame
    void Update()
    {
        float tempGage = (float)sword.temperature / (float)sword.maxTemperature;
        float hpGage = (float)player.hp / (float)player.maxHp;
        UpdateTemperature();
        hpImage.fillAmount = hpGage;

        if (!particleOn && tempGage > hpGage)// 熱ダメージをうけていたら
        {
            particleOn = true;
            //Instantiate<GameObject>(particle, this.transform);
        }
        else if (particleOn && tempGage <= hpGage)
        {
            particleOn = false;
            //Destroy(particle);
        }
        HpGlow();
    }

    private void FindTemperatureMasters()
    {
        foreach (Transform child in transform.GetChild(0))
        {
            tempes.Add(child.GetComponent<RectTransform>());
        }
    }

    private void UpdateTemperature()
    {
        int count = tempes.Count;
        float tempGage = (float)sword.temperature / (float)sword.maxTemperature;
        float maxWidth = 1126f;
        float oneWidth = maxWidth / count;
        float width = maxWidth * tempGage;
        for (int i = 0; i < count; i++)
        {
            Vector2 size = tempes[i].sizeDelta;
            if (width > oneWidth * (i + 1))
            {
                size.x = oneWidth * (i + 1);
            }
            else
            {
                size.x = (width - oneWidth * i) + oneWidth * i;
            }
            tempes[i].sizeDelta = size;
        }
    }

    private void Reset()
    {
        Transform blade = transform.GetChild(0);
        for (int i = blade.childCount - 1; i >= 0; --i)
        {
            GameObject.DestroyImmediate(blade.GetChild(i).gameObject);
        }

        sword = GameObject.FindGameObjectWithTag("Player").GetComponent<Sword>();
        int amount = sword.swordColors.Length;

        GameObject prefab = (GameObject)Resources.Load("UI/TemperatureMaster");

        float width = prefab.GetComponent<RectTransform>().sizeDelta.x;

        for (int i = 0; i < amount; i++)
        {
#if UNITY_EDITOR
            GameObject g = UnityEditor.PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            g.transform.SetParent(transform.GetChild(0));
            g.transform.localPosition = new Vector3(374, 0, 0);
            g.transform.localScale = Vector3.one;

            Image image = g.transform.GetChild(0).GetComponent<Image>();
            float alpha = 1f / amount;
            image.fillAmount = 1f - alpha * i;

            image.color = sword.swordColors[i].color;

            RectTransform rect = g.GetComponent<RectTransform>();
            Vector2 size = rect.sizeDelta;
            size.x = width * alpha * (i + 1);
            rect.sizeDelta = size;
#endif
        }
    }

    private float alpha;
    [HideInInspector]
    public bool glowing = false;

    private void HpGlow()
    {
        if (!glowing)
        {
            outline.effectDistance = Vector2.zero;
            return;
        }
        alpha = (Mathf.Sin(2 * Mathf.PI * glowSpeed * Time.time) / 2 + 0.5f) * 20;
        outline.effectDistance = new Vector2(alpha, alpha);
    }
}
