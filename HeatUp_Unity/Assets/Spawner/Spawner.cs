using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public LayerMask playerMask;
    List<SpawnArea> spawnArea;
    // Start is called before the first frame update
    void Start()
    {
        spawnArea = new List<SpawnArea>();
        int lenght = transform.childCount;
        for (int i = 0; i < lenght; i++) 
        {
            SpawnArea s = transform.GetChild(i).GetComponent<SpawnArea>();
            spawnArea.Add(s);
        }
    }

    private bool spawn = false;
    private void Update()
    {
        if(spawn)
        {
            return;
        }
        RaycastHit2D hit = Physics2D.BoxCast(transform.position,
            transform.localScale, 0, Vector2.zero, 0, playerMask);
        if (hit) 
        {
            Debug.Log("Spawn");
            LayerMask collisionMask = hit.collider.GetComponent<Player>().controller.collisionMask;
            string str = LayerMask.LayerToName(gameObject.layer);
            LayerMask myLayer = LayerMask.GetMask(str);
            if (collisionMask == myLayer)
            {
                foreach(SpawnArea s in spawnArea)
                {
                    s.Spawn();
                }
                spawn = true;
            }
        }
    }

    void OnDrawGizmos()
    {
        Color c = Gizmos.color;
        c = Color.cyan;
        c.a = 0.3f;
        Gizmos.color = c;
        Gizmos.DrawCube(transform.position, new Vector2(transform.localScale.x, transform.localScale.y));
        spawnArea = new List<SpawnArea>();
        int lenght = transform.childCount;
        for (int i = 0; i < lenght; i++)
        {
            SpawnArea s = transform.GetChild(i).GetComponent<SpawnArea>();
            spawnArea.Add(s);
        }
        Gizmos.color = Color.red;
        for (int i = 0; i < lenght; i++)
        {
            Gizmos.DrawLine(transform.position, spawnArea[i].transform.position);
        }
    }
}
