using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class creature1_0 : DefaultCreature
{
    public override void FindAttackSound()
    {
        attackSound = Resources.Load<AudioClip>("Sound/MeleeHit1");
        //audioSource.clip = attackSound;
        Debug.Log("found attack sound!");
    }

    public override void FindDeathSound()
    {
        deathSound = Resources.Load<AudioClip>("Sound/Death1");
    }

}
