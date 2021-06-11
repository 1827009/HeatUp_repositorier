using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneDebris : MonoBehaviour
{
    [SerializeField, TooltipAttribute("破片のエフェクト")]
    ParticleSystem stoneDebris;
    [SerializeField, TooltipAttribute("爆発範囲")]
    float range = 3;
    [SerializeField, TooltipAttribute("攻撃力")]
    int damage = 1000;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy" || collision.tag == "Ground") 
        {
            RaycastHit2D[] search = Physics2D.CircleCastAll(transform.position, range, Vector2.zero);
            foreach (RaycastHit2D hit in search)
            {
                Character character = hit.collider.GetComponent<Character>();
                if(character)
                {
                    character.ApplyDamage(damage);
                }
            }
            stoneDebris.transform.position = this.transform.position;
            Instantiate(stoneDebris);
            Destroy(this.gameObject);
        }
    }
}
