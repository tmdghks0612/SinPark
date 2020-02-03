using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBase : DefaultCreature
{
    private GameObject friendlyHealthBar;
    private GameObject hostileHealthBar;

    [SerializeField]
    private int friendlyMaxHealth;
    [SerializeField]
    private int hostileMaxHealth;

    [SerializeField]
    private static int friendlyCurrentHealth;
    [SerializeField]
    private static int hostileCurrentHealth;

    // Start is called before the first frame update
    void Start()
    {
        friendlyCurrentHealth = friendlyMaxHealth;
        hostileCurrentHealth = hostileMaxHealth;
        friendlyHealthBar = GameObject.Find("friendlyBaseHealthBar");
        hostileHealthBar = GameObject.Find("hostileBaseHealthBar");
        size = 99; // To prevent base from pushed
        gameControl = GameObject.Find("GameControl").GetComponent<GameControl>();
    }

    public override void SetCreature( Vector3 st, Vector3 ed, int buttonNum, int lane, GameControl.Sides sideCheck )
    {
        
        base.SetCreature(st, ed, buttonNum, lane, sideCheck);
        
        this.creatureAttackType = AttackType.Missile;
       
    }

    public override void DamageTaken(int damage, int size)
    {
        if (damage > 0)
        {
            if (side == GameControl.Sides.Friendly)
            {
                friendlyCurrentHealth -= damage;
                CalculateHealth();
            }
            else
            {
                hostileCurrentHealth -= damage;
                CalculateHealth();
            }
        }
    }

    void CalculateHealth()
    {
        if( friendlyCurrentHealth < 0)
        {
            //player down!
            Debug.Log("player died!");
            PlayerLose();
        }
        else if(hostileCurrentHealth < 0)
        {
            Debug.Log("player win!");
            PlayerWin();
        }
        else
        {
            if (side == GameControl.Sides.Friendly)
            {
                friendlyHealthBar.transform.localScale = new Vector3((float)friendlyCurrentHealth / friendlyMaxHealth, 1.0f, 1.0f);
            }
            else
            {
                hostileHealthBar.transform.localScale = new Vector3((float)hostileCurrentHealth / hostileMaxHealth, 1.0f, 1.0f);
            }
        }
    }

    private void PlayerLose()
    {
        friendlyHealthBar.transform.localScale = new Vector3(0,1,1);
        gameControl.GameOver(false);
        this.Dead();
    }
    private void PlayerWin()
    {
        hostileHealthBar.transform.localScale = new Vector3(0, 1, 1);
        gameControl.GameOver(true);
        this.Dead();
    }
}
