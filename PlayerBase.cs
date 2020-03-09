using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBase : DefaultCreature
{
    //HealthBar of friendly and hostile base
    protected GameObject friendlyHealthBar;
    public GameObject hostileHealthBar;

    //Both Health cannot exceed their MaxHealth
    [SerializeField]
    private int friendlyMaxHealth;
    [SerializeField]
    protected int hostileMaxHealth;

    //curernt health of the PlayerBase
    [SerializeField]
    private static int friendlyCurrentHealth;
    [SerializeField]
    protected static int hostileCurrentHealth;

    // Start is called before the first frame update
    void Start()
    {
        //start game with full health
        friendlyMaxHealth = (int)PublicLevel.GetPlayerMaxMana() * 8;
        friendlyCurrentHealth = friendlyMaxHealth;
        hostileCurrentHealth = hostileMaxHealth;
        
        //Find HealthBar GameObject in the scnce
        friendlyHealthBar = GameObject.Find("friendlyBaseHealthBar");
        hostileHealthBar = GameObject.Find("hostileBaseHealthBar");

        /*if(this.GetSide() == GameControl.Sides.Friendly)
        {
            this.tag = "FriendlyBase";
        }
        else
        {
            this.tag = "HostileBase";
        }*/
        
        size = 99; // To prevent base from pushed

        //Find Gamecontrol
        gameControl = GameObject.Find("GameControl").GetComponent<GameControl>();
    }

    //overrided function from DefaultCreature.
    public override void DamageTaken(int damage, int size)
    {
        //PlayerBase does not get heal
        if (damage > 0)
        {
            //Damage Taken depends on the side(Friendly/Hostile) of the base
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

    // Win/Lose Check and healthBar update
    protected void CalculateHealth()
    {
        if( friendlyCurrentHealth < 0)
        {
            PlayerLose();
        }
        else if(hostileCurrentHealth < 0)
        {
            PlayerWin();
        }
        else
        {
            //HealthBar is updated by changing localScale of the bar based on percentile.
            if (side == GameControl.Sides.Friendly)
            {
                friendlyHealthBar.transform.localScale = new Vector3((float)friendlyCurrentHealth / friendlyMaxHealth, friendlyHealthBar.transform.localScale.y, 1.0f);
            }
            else
            {
                hostileHealthBar.transform.localScale = new Vector3((float)hostileCurrentHealth / hostileMaxHealth, hostileHealthBar.transform.localScale.y, 1.0f);
            }
        }
    }

    //When Friendly side PlayerBase's health < 0, call GameOver Function in gameControl with parameter false.
    private void PlayerLose()
    {
        friendlyHealthBar.transform.localScale = new Vector3(0,1,1);
        gameControl.GameOver(false);
    }

    //When Hostile side PlayerBase's health < 0, call GameOver Function in gameControl with parameter true.
    private void PlayerWin()
    {
        hostileHealthBar.transform.localScale = new Vector3(0, 1, 1);
        gameControl.GameOver(true);
    }
}
