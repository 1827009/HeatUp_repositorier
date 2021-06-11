using UnityEngine;
using System.Collections;

public class FollowCamera : MonoBehaviour
{
	[Header("Reference")]
	[SerializeField, TooltipAttribute("追尾するオブジェクト")]
	public Transform target;

	[Header("Options")]
	[SerializeField, TooltipAttribute("スムーズスピード(値を大きくすると、よりスムーズに動きます)")]
	public float smoothSpeed = 0.5f;

	[HideInInspector]
	public Vector3 offset;
	[HideInInspector]
	public bool follow = true;

	Vector3 velocity = Vector3.zero;

	void Start()
	{
		SetOffset(transform.position);
	}

	void Update()
	{
		if (follow)
		{
			Vector3 desiredPosition = target.position + offset;
			Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothSpeed);
			transform.position = smoothedPosition;
		}
	}

	public void SetOffset(Vector3 pos)
	{
		offset = pos - target.position;
	}
}