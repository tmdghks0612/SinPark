using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealCreature : DefaultCreature
{
    [SerializeField]
    protected float safeRange;
    [SerializeField]
    protected int healAmount;


    public override void SetCreature(Vector3 st, Vector3 ed, int buttonNum, int lane, GameControl.Sides sideCheck)
    { 
        base.SetCreature(st, ed, buttonNum, lane, sideCheck);
        attackDamage = healAmount * -1; //
    }
}
