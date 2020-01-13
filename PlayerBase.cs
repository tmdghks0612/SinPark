using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBase : DefaultCreature
{
    public GameObject healthBar;

    
    private int maxHealth = 200;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public override void SetCreature( Vector2 st, Vector2 ed, int creatureType, int lane, GameControl.Sides sideCheck )
    {
        
        base.SetCreature(st, ed, creatureType, lane, sideCheck);
        this.hp = maxHealth;
        this.creatureAttackType = AttackType.Missile;
        this.creatureType = 0;
    }

    public override void DamageTaken(int damage)
    {
        hp -= damage;
        CalculateHealth();
    }

    void CalculateHealth()
    {
        if( this.hp < 0)
        {
            //player down!
            Debug.Log("player died!");
            this.Dead();
        }
        else
        {
            healthBar.transform.localScale = new Vector3( (float)this.hp / maxHealth, 1.0f, 1.0f );
        }

    }
}
