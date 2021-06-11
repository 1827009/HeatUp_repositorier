using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CameraController : MonoBehaviour
{
	public Player player;

	FollowCamera followCamera;

	void Start()
	{
		followCamera = GetComponent<FollowCamera>();
	}

	void Update()
	{
		CompulsionMoveInUpdate();
	}


	#region CompulsionMove

	CameraTrigger cameraTrigger;
    float cameraAlpha = 0;
	bool jump = false;
	Vector3 cameraStartPos;
	Vector3 cameraStopPos;

	public void SetCameraTrigger(CameraTrigger cameraTrigger)
	{
        this.cameraTrigger = cameraTrigger;
    }

	void CompulsionMoveInUpdate()
	{
        if (!cameraTrigger)
        {
            return;
        }
        if (cameraAlpha == 0)
        {
            cameraStartPos = transform.position;
            cameraStopPos = cameraTrigger.connectionTrigger.transform.position + followCamera.offset;
            followCamera.follow = false;
            player.inputStop = true;
            player.compulsionInput = cameraTrigger.go.vector;
        }
        if (cameraAlpha < 1)
        {
            cameraAlpha += cameraTrigger.cameraSpeed * Time.deltaTime;
            transform.position = Vector3.Lerp(cameraStartPos, cameraStopPos, cameraAlpha);
            if (cameraAlpha > cameraTrigger.jumpAlpha && !jump)
            {
                player.transform.position = cameraTrigger.connectionTrigger.come.transform.position;
                jump = true;
            }
            if (jump)
            {
                player.compulsionInput = cameraTrigger.connectionTrigger.come.vector;
                string s = LayerMask.LayerToName(cameraTrigger.connectionTrigger.gameObject.layer);
                LayerMask l = LayerMask.GetMask(s);
                player.SetCollisionMask(l);
            }
        }
        else
        {
            transform.position = cameraStopPos;
            followCamera.follow = true;
            player.inputStop = false;
            jump = false;
            cameraAlpha = 0;
            cameraTrigger = null;
        }
    }

    #endregion
}
