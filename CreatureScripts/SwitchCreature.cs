using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchCreature : DefaultCreature
{
	[SerializeField]
    protected float meleeRange;
	[SerializeField]
    protected float missileRange;


    protected override void DetectEnemy()
    {
		bool detectCheck;
		currentPosition = transform.position;
		detectCheck = combatControl.SearchCreature(currentPosition, meleeRange, laneNum, side);

		if (detectCheck)
		{
			moveFlag = false;
			attackFlag = true;
			creatureAttackType = AttackType.Melee;
		}
		else
		{
			detectCheck = combatControl.SearchCreature(currentPosition, missileRange, laneNum, side);
			if(detectCheck)
			{
				moveFlag = false;
				attackFlag = true;
				creatureAttackType = AttackType.Missile;
			}
			else
			{
				attackFlag = false;
				moveFlag = true;
			}
		}
	}
}
