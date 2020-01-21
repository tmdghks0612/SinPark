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
        base.InitCreature();
        base.laneNum = lane;
        base.side = sideCheck;
        detectRange = attackRange;
        attackDamage = healAmount * -1; //
        this.creatureType = creatureType;
        this.upgradeType = upgradeType;
        if (side == GameControl.Sides.Hostile)
        {
            speed.x *= -1;
            base.Enemy = true;
            transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
        }

        base.MoveTo(st, ed);
    }
}
