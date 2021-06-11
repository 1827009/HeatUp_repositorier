using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Iron_Barrels : MonoBehaviour
{
    Vector2 Push;
    public float Force = 50;
    public float T = 5;
    public float Span = 0.5f;
    public Rigidbody2D rigidbody2D;

    AudioSource audioSource;
    bool damage = false;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2D = this.gameObject.GetComponent<Rigidbody2D>();
        rigidbody2D.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ApplyDamage(Vector2 dir, int Temperature)
    {
        rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionY;
        Push = Vector2.zero;
        Push.x = Force * dir.x;
        rigidbody2D.AddForce(Push, ForceMode2D.Impulse);
        audioSource.Play();
        damage = true;
       
        GameObject.Destroy(this.gameObject, T + (Span * Temperature));
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == gameObject.layer && damage) 
        {
            Destroy(gameObject);
        }
    }
}