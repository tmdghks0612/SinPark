using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoomerCreature : DefaultCreature
{
    public GameObject explosion;

    protected override void Attack()
    {
        Debug.Log(attackFlag);
        if (attackFlag)
        {
            if (animControl != null)
                animControl.SetBool("onAttack", true);
            Dead();
        }
        else
        {
            if (animControl != null)
                animControl.SetBool("onAttack", false);
        }
    }

    public override void Dead()
    {
        combatControl.PopCreature(laneNum, side, this);
        CancelInvoke("Dead");
        CancelInvoke("DetectEnemy");
        combatControl.MeleeAttack(currentPosition, attackRange, attackDamage, size + 1, laneNum, side);
        Instantiate(explosion, currentPosition, transform.rotation = Quaternion.identity);
        Destroy(this.gameObject);
    }
}
