using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTrigger : MonoBehaviour
{
    public LayerMask playerMask;
    public float cameraSpeed = 1;
    [Range(0, 1)]
    public float jumpAlpha = 0.5f;
    public CameraTrigger connectionTrigger;

    [HideInInspector]
    public MoveDirection go;
    [HideInInspector]
    public MoveDirection come;

    BoxCollider2D collider;
    LayerMask layerMask;

    private void Start()
    {
        go = transform.GetChild(0).GetComponent<MoveDirection>();
        come = transform.GetChild(1).GetComponent<MoveDirection>();
        collider = GetComponent<BoxCollider2D>();
        string str = LayerMask.LayerToName(gameObject.layer);
        layerMask = LayerMask.GetMask(str);
    }

    void OnDrawGizmos()
    {
        go = transform.GetChild(0).GetComponent<MoveDirection>();
        come = transform.GetChild(1).GetComponent<MoveDirection>();
        collider = GetComponent<BoxCollider2D>();
        Gizmos.color = new Color(0, 0, 1, 0.3f);
        Gizmos.DrawCube(transform.position, collider.bounds.size);
        if (connectionTrigger && connectionTrigger.go && connectionTrigger.come) 
        {
            Gizmos.DrawLine(transform.position, connectionTrigger.come.transform.position);
        }
    }
}
