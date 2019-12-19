using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PAttack : MonoBehaviour
{
    public MoveType AttackType { get; set; }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //print(this.gameObject.GetComponent<CircleCollider2D>().)
    }

    private bool hasHit = false;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!hasHit)
        {
            BossAttack incomingAttack = collision.gameObject.GetComponent<BossAttack>();

            if (incomingAttack != null && incomingAttack.RequiredMove == this.AttackType)
            {
                Destroy(incomingAttack.gameObject);
                hasHit = true;
            }

        }
    }


}
