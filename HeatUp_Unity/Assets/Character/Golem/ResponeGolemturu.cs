using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResponeGolemturu : MonoBehaviour
{
    public float time = 2;
    public GameObject prefab;
    public bool Respaen = false;
    public GameObject origin;
    bool spawn = false;

    [HideInInspector]
    public bool canSpawn = true;
    Vector3 pos;
    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = transform.parent.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!animator.GetBool("isMoving")) 
        {
            return;
        }
        if (!origin && !spawn&&Respaen)
        {
            StartCoroutine(Spawn());
            pos= transform.position;
        }

    }

    IEnumerator Spawn()
    {
        spawn = true;
        Respaen = false;
        yield return new WaitForSeconds(time);
        GameObject g = Instantiate(prefab);
        g.transform.position = transform.position;
        g.layer = gameObject.layer;
        origin = g;
        spawn = false;
        StopCoroutine(Spawn());
    }
}
