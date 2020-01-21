using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoomerCreature : DefaultCreature
{
    public GameObject explosion;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    protected override void Attack()
    {
        if (attackFlag)
        {
            if (animControl != null)
                animControl.SetBool("onAttack", true);
            this.Dead();
        }
        else
        {
            if (animControl != null)
                animControl.SetBool("onAttack", false);
        }
    }

    protected override void Dead()
    {
        combatControl.PopCreature(laneNum, side, this);
        CancelInvoke("Dead");
        CancelInvoke("DetectEnemy");
        combatControl.MeleeAttack(currentPosition, attackRange, attackDamage, laneNum, side);
        Instantiate(explosion, currentPosition, transform.rotation = Quaternion.identity);
        Destroy(this.gameObject);
    }
}
