using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;

public class start : MonoBehaviour
{
    [SerializeField] GameObject testpage;
    private Camera camera;
    private Quaternion rotation;
    private float zoom;
    private float view;
    private float GetTime = 0;
    private Vector3 p;
    private Vector3 r;
    private Vector3 rot;
    private bool startF = false;
    private float FPS = 120;

    void Start()
    {
        camera = GetComponent<Camera>();
        view = camera.fieldOfView;
        rotation = camera.transform.rotation;
        p.x = (float)43.23 / FPS;
        p.y = (float)34.97/ FPS;
        p.z = -5/ FPS;
        r.x= (float)3.7 / FPS;
        r.y= (float)0.5/ FPS;
        zoom=-66/ FPS;


    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown("space")) //マウス左クリック、スペースキーを押した場合
        {
            startF = true;
            testpage.SetActive(false);
            GetTime = Time.time;
        }
    
        if (startF)
        {
            if (Time.time - GetTime>0.5f)
            {
                if (camera.transform.position.z < -21.0f)
                {
                    if (view > 60)
                    {
                        view += zoom;
                    }
                    else
                    {
                        SceneManager.LoadScene("Abe4_Stage");//some_senseiシーンをロードする
                    }
                    camera.fieldOfView = view;
                    camera.transform.position -= p;
                    rot.x -= r.x;
                    rot.y -= r.y;
                    camera.transform.rotation = Quaternion.Euler(rot.x, rot.y, 0.0f); ;
                }
            }
        }
        



    }
}


