using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOver_BackGround : MonoBehaviour
{
    public float fadeSpeed = 0.04f;        //透明度が変わるスピードを管理
    [TooltipAttribute("背景の濃さ　最大１")]
    public float maxalfa = 0.7f;
    float red, green, blue, alfa;   //パネルの色、不透明度を管理

    public bool isFadeOut = false;  //フェードアウト処理の開始、完了を管理するフラグ
    public bool isFadeIn = false;   //フェードイン処理の開始、完了を管理するフラグ

    [TooltipAttribute("BackGroundを入れてください")]
    public GameObject fadeObject;  //透明度を変更するパネルのイメージ
    [TooltipAttribute("0: Logo 1:Restart 2:Title 3:Icon")]
    public GameObject[] UI = new GameObject[3];

    public GameObject player;

    Player player_Script;

    Image fadeImage;

    void Start()
    {
        //isFadeIn = true;
        if (maxalfa > 1) maxalfa = 1;
        fadeImage = fadeObject.GetComponent<Image>();
        red = fadeImage.color.r;
        green = fadeImage.color.g;
        blue = fadeImage.color.b;
        alfa = 0;
        player = GameObject.FindWithTag("Player");
        player_Script = player.GetComponent<Player>();
    }

    void Update()
    {
        if (player_Script.hp <= 0 && !isFadeOut && !fadeObject.activeSelf && !isFadeIn)
        {
            isFadeOut = true;
            fadeObject.SetActive(true);
            alfa = 0;
            SetAlpha();
            player.SetActive(false);
        }

        if (isFadeIn && !fadeObject.activeSelf)
        {
            fadeObject.SetActive(true);
            alfa = 1;
            SetAlpha();
        }

        if (Input.GetKeyDown("h") && !isFadeIn)
        {
            player_Script.hp = 0;
        }

        if (isFadeIn && !isFadeOut)
        {
            StartFadeIn();
        }

        if (isFadeOut && !isFadeIn)
        {
            StartFadeOut();
        }
    }

    void StartFadeIn()
    {
        alfa -= fadeSpeed;
        SetAlpha();
        if (alfa <= 0)
        {
            isFadeIn = false;
            fadeObject.SetActive(false);
        }
    }

    void StartFadeOut()
    {
        alfa += fadeSpeed;
        SetAlpha();
        if (alfa >= maxalfa)
        {
            isFadeOut = false;
            foreach (GameObject a in UI)
            {
                a.SetActive(true);
            }
        }
    }

    void SetAlpha()
    {
        fadeImage.color = new Color(red, green, blue, alfa);
    }
}
