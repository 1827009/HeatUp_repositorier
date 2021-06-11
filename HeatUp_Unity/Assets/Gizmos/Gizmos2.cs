using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gizmos2
{
	public static void DrawArrow(Vector3 from, Vector3 to, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
	{
		Gizmos.DrawLine(from, to);
		var direction = to - from;
		var right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
		var left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
		Gizmos.DrawLine(to, to + right * arrowHeadLength);
		Gizmos.DrawLine(to, to + left * arrowHeadLength);
	}
}