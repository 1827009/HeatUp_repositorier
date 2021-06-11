using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fadeOn : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject stageStartImage;
    public AudioSource bgm;
    private Fade fade;
    private bool oneclick = false;
    private Sword sword;

    void Start()
    {
        fade = GetComponent<Fade>();
        sword = GameObject.FindWithTag("Player").GetComponent<Sword>();
        oneclick = false;
        if (!ShowStart.showStart) 
        {
            PlayBgm();
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S) && !oneclick || Input.GetKeyDown("joystick button 7") && !oneclick) //キーを押した場合
        {
            oneclick = true;
            fade.Play();
            Invoke("PlayBgm", 1);
        }
    }

    private void PlayBgm()
    {
        bgm.Play();
        stageStartImage.SetActive(true);
        sword.stop = false;
    }
}