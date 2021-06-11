using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatapultStand : MonoBehaviour
{
    [HideInInspector]
    public List<GameObject> objects;

    BoxCollider2D hitBox;

    Catapult catapult;
    Transform throwPoint;

    // Start is called before the first frame update
    void Start()
    {
        objects = new List<GameObject>();

        hitBox = GetComponent<BoxCollider2D>();
        throwPoint = transform.Find("ThrowPoint").transform;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!objects.Contains(collision.gameObject))
            objects.Add(collision.gameObject);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (objects.Contains(collision.gameObject))
            objects.Remove(collision.gameObject);
    }

    public void shotCarry()
    {
        foreach (GameObject ob in objects)
        {
            ob.transform.position = throwPoint.position;
        }
    }
}