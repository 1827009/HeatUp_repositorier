﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Sword : MonoBehaviour
{
    [Header("AttackOptions")]

    [SerializeField, TooltipAttribute("攻撃間隔")]
    public float interval = 0.5f;

    [SerializeField, TooltipAttribute("発動時間")]
    public float activateTime = 0.5f;

    [SerializeField, TooltipAttribute("キャラクターに与えるダメージ")]
    public int damageToPlayer = 2;

    [Header("TemperatureOptions")]
    [TooltipAttribute("初期温度")]
    public int defaultTemperature = 10;

    [TooltipAttribute("最大温度")]
    public int maxTemperature = 1000;

    [TooltipAttribute("温度上昇間隔")]
    public float increaseInterval = 1;

    [TooltipAttribute("温度上昇の初期値")]
    public float temperatureIncrease = 10;

    [TooltipAttribute("温度上昇の加速度")]
    public float temperatureAcceleration = 1;

    [TooltipAttribute("温度による色の種類")]
    public SwordColor[] swordColors;

    [Header("CollisionOptions")]
    [SerializeField, TooltipAttribute("攻撃範囲")]
    public Vector2 attackRange = Vector2.one;

    [SerializeField, TooltipAttribute("キャラクターの左右にある当たり判定の位置")]
    public Vector2 sideOffset = Vector2.zero;

    [SerializeField, TooltipAttribute("キャラクターの下にある当たり判定の位置")]
    public float underOffset = 0;

    [Header("CameraOptions")]
    [SerializeField, TooltipAttribute("カメラシェイクをオンにする温度")]
    public int cameraShakeTemperature = 50;

    [SerializeField, TooltipAttribute("カメラシェイクの最大震度")]
    public float maxCameraShake = 0.4f;

    [Header("Reference")]

    [SerializeField, TooltipAttribute("色を変更するスロット")]
    [Spine.Unity.SpineSlot]
    public string slot;

    [SerializeField, TooltipAttribute("剣のマテリアル")]
    public Material material;

    [HideInInspector]
    public int temperature;

    [SerializeField, TooltipAttribute("温度上昇を止める")]
    public bool stop = false;

    Player player;
    AudioSource[] sounds;
    CameraShake cameraShake;
    IEnumerator attackStart;

    [System.Serializable]
    public struct SwordColor
    {
        public int temperature;
        public Color color;
    }

    void Start()
    {
        player = GetComponent<Player>();
        sounds = GetComponents<AudioSource>();
        skeletonAnimation = player.GetComponent<Spine.Unity.SkeletonAnimation>();
        cameraShake = Camera.main.GetComponent<CameraShake>();
        temperature = defaultTemperature;
        UpdateColor();
    }

    void Update()
    {
        if(stop)
        {
            return;
        }
        if (barrels)
        {
            Vector3 playerPos = transform.position;
            playerPos.x = barrels.transform.position.x;
            transform.position = playerPos;
        }
        TemperatureIncrease();
    }

    #region Attack

    bool attackInput = false;
    bool attack = false;
    LayerMask layerMask;

    public void Attack(bool under, LayerMask layerMask)
    {
        attackInput = true;
        attackStart = AttackStart(under, layerMask);
        StartCoroutine(attackStart);
    }

    public void AttackFinish()
    {
        attackInput = false;
        attackStart = null;
        barrels = null;
    }

    private IEnumerator AttackStart(bool under, LayerMask layerMask)
    {
        yield return new WaitForSeconds(activateTime);
        attack = true;
        this.layerMask = layerMask;
        StartCoroutine(AttackLoop(under));
        yield break;
    }

    private IEnumerator AttackLoop(bool under)
    {
        while (attack)
        {
            if (under)
            {
                UnderHit(layerMask);
            }
            else
            {
                SideHit(player.right, layerMask);
            }
            if (!attackInput)
            {
                attack = false;
            }
            sounds[1].Play();
            yield return new WaitForSeconds(interval);
        }
        yield break;
    }

    void SideHit(bool right, LayerMask enemyMask)
    {
        Vector2 swordOffset = right ? sideOffset : new Vector2(-sideOffset.x, sideOffset.y);
        Vector2 center = new Vector2(transform.position.x, transform.position.y) + swordOffset;
        RaycastHit2D[] hits = Physics2D.BoxCastAll(center, attackRange, 0, Vector2.zero, 0, enemyMask);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit)
            {
                if (!Iron_BarrelsHit(hit))
                {
                    if (!BossHit(hit))
                    {
                        if (!Switch_AttackHit(hit))
                        {
                            if (!BeeAttack(hit))
                            {
                                Character chara = hit.collider.GetComponent<Character>();
                                if (chara)
                                {
                                    if (temperature > 0)
                                    {
                                        sounds[0].Play();
                                    }
                                    int maxHp = chara.maxHp;
                                    chara.ApplyDamage(temperature);
                                    UpdateTemperature(-maxHp);
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    Barrels barrels;

    void UnderHit(LayerMask enemyMask)
    {
        Vector2 center = new Vector2(transform.position.x, transform.position.y - underOffset);
        RaycastHit2D underHit = Physics2D.BoxCast(center, attackRange, 0, Vector2.zero, 0, enemyMask);
        if (underHit)
        {
            barrels = underHit.collider.GetComponent<Barrels>();
            if (barrels)
            {
                barrels.ApplyDamage((float)temperature / (maxTemperature / 4));
                attackInput = false;
            }
        }
    }

    bool Iron_BarrelsHit(RaycastHit2D hit)
    {
        Iron_Barrels iron_barrels = hit.collider.GetComponent<Iron_Barrels>();
        if (iron_barrels)
        {
            if (!sounds[0].isPlaying)
            {
                sounds[0].Play();
            }
            iron_barrels.ApplyDamage((hit.transform.position - transform.position).normalized, temperature);
            attackInput = false;
            sounds[0].Stop();
            UpdateTemperature(-10);
            return true;
        }
        return false;
    }

    bool BossHit(RaycastHit2D hit)
    {
        GolemHead bossHead = hit.collider.GetComponent<GolemHead>();
        if (bossHead)
        {
            bossHead.master.ApplyDamage(temperature);
            return true;
        }
        return false;
    }

    bool Switch_AttackHit(RaycastHit2D hit)
    {
        Switch_Attack switch_Attach = hit.collider.GetComponent<Switch_Attack>();
        if (switch_Attach)
        {
            switch_Attach.onSwitch(this);
            return true;
        }
        return false;
    }

    bool BeeAttack(RaycastHit2D hit)
    {
        Hati hati = hit.collider.GetComponent<Hati>();
        if (hati)
        {
            hati.ApplyDamage(temperature);
            return true;
        }
        return false;
    }

    #endregion

    #region Temperature

    float totalTime = 0;
    float currentTime = 0;
    Spine.Unity.SkeletonAnimation skeletonAnimation;

    void SetMaterial(Color color)
    {
        Spine.Slot s = skeletonAnimation.skeleton.FindSlot(slot);
        material.SetColor("_Color", color);
        skeletonAnimation.CustomSlotMaterials.Remove(s);
        skeletonAnimation.CustomSlotMaterials.Add(s, material);
    }

    void TemperatureIncrease()
    {
        totalTime += Time.deltaTime;
        currentTime += Time.deltaTime;
        if (currentTime > increaseInterval)
        {
            int temp = (int)(temperatureIncrease + temperatureAcceleration * totalTime);
            if (UpdateTemperature(temp))
            {
                totalTime = 0;
            }
            if (cameraShakeTemperature < temperature)
            {
                int max = maxTemperature - cameraShakeTemperature;
                int difference = temperature - cameraShakeTemperature;
                float alpha = (float)difference / max;
                float magnitude = Mathf.Lerp(0.0f, maxCameraShake, alpha);
                cameraShake.ShakeStop();
                cameraShake.Shake(magnitude);
            }
            else
            {
                cameraShake.ShakeStop();
            }
            currentTime = 0;
        }
    }

    public bool UpdateTemperature(int tempe)
    {
        bool release = false;
        if (temperature <= 0 || temperature >= maxTemperature)
        {
            release = true;
        }
        temperature += tempe;
        temperature = Mathf.Clamp(temperature, 0, maxTemperature);
        UpdateColor();
        if (player.maxTemperature < temperature)
        {
            player.ApplyDamage(damageToPlayer);
        }
        return release;
    }

    void UpdateColor()
    {
        int min = -1;
        int minIndex = 0;

        for (int i = 0; i < swordColors.Length; i++)
        {
            if (swordColors[i].temperature <= temperature)
            {
                int temp = temperature - swordColors[i].temperature;
                if (min == -1)
                {
                    min = temp;
                    minIndex = i;
                }
                else if (min > temp)
                {
                    min = temp;
                    minIndex = i;
                }
            }
        }
        SetMaterial(swordColors[minIndex].color);
    }

    #endregion

    void OnDrawGizmos()
    {
        Vector2 center = new Vector2(transform.position.x, transform.position.y) + sideOffset;
        Gizmos.color = new Color(1, 0, 0, 0.3f);
        Gizmos.DrawWireCube(center, attackRange);
        Gizmos.color = new Color(0, 1, 0, 0.3f);
        center = new Vector2(transform.position.x, transform.position.y) + new Vector2(-sideOffset.x, sideOffset.y);
        Gizmos.DrawWireCube(center, attackRange);
        Gizmos.color = new Color(0, 0, 1, 0.3f);
        center = new Vector2(transform.position.x, transform.position.y - underOffset);
        Gizmos.DrawWireCube(center, attackRange);
    }
}