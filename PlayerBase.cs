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
        size = 99; // To prevent base from pushed
    }

    public override void SetCreature( Vector3 st, Vector3 ed, int creatureType, int upgradeType, int lane, GameControl.Sides sideCheck )
    {
        
        base.SetCreature(st, ed, creatureType, upgradeType, lane, sideCheck);
        this.hp = maxHealth;
        this.creatureAttackType = AttackType.Missile;
        this.creatureType = 0;
        this.upgradeType = 0;
    }

    public override void DamageTaken(int damage, int size)
    {
        if (damage > 0)
        {
            hp -= damage;
            CalculateHealth();
        }
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
