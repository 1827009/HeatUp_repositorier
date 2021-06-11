using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;

public class startCamera : MonoBehaviour
{
    public bool play = false;
    public Transform defaultCameraPos;
    private Camera camera;
    private FollowCamera followCamera;
    private Vector3 p;
    private bool startF = false;
    private bool oneclick = false;
    private Vector3 def = Vector3.zero;
    [HideInInspector]
    public bool enablePause = false;

    void Start()
    {
        camera = GetComponent<Camera>();
        followCamera = camera.GetComponent<FollowCamera>();
        if (defaultCameraPos) 
        {
            followCamera.follow = false;
            def = camera.transform.position;
            followCamera.SetOffset(def);
            camera.transform.position = defaultCameraPos.position;
        }
        if(play)
        {
            startF = true;
            oneclick = true;
            followCamera.follow = true;
            enablePause = true;
        }
        if(!ShowStart.showStart)
        {
            followCamera.follow = true;
            enablePause = true;
            enabled = false;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S) && !oneclick|| Input.GetKeyDown("joystick button 7") && !oneclick) //キーを押した場合
        {
            startF = true;
            oneclick = true;
            enablePause = true;
        }
    }
}


