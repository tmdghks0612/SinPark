using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealCreature : DefaultCreature
{
    [SerializeField]
    protected float safeRange;
    [SerializeField]
    protected int healAmount;


    public override void SetCreature(Vector2 st, Vector2 ed, int creatureType, int upgradeType, int lane, GameControl.Sides sideCheck)
    { 
        base.SetCreature(st, ed, creatureType, upgradeType, lane, sideCheck);
        attackDamage = healAmount * -1; //
    }
}
