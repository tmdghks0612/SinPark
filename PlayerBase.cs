using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBase : MonoBehaviour
{
    public GameControl gameControl;
    public GameObject manaBar;
    public GameObject healthBar;

    [SerializeField]
    private int baseHealth;
    [SerializeField]
    private int baseMana;

    private int maxMana;
    private int maxHealth;
    private int regenAmount;
    private float regenTime;

    // Start is called before the first frame update
    void Start()
    {
        maxMana = 100;
        maxHealth = 200;
        baseHealth = 100;
        regenTime = 0.5f;
        regenAmount = 5;
        InvokeRepeating("GainMana", 0.5f, regenTime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GainMana()
    {
        if ( baseMana + regenAmount > maxMana )
        {
            baseMana = maxMana;
        }
        else
        {
            baseMana += 5;
        }
        ScaleManaBar();
    }

    void ScaleManaBar()
    {
        manaBar.transform.localScale = new Vector3( (float)baseMana / maxMana, 1.0f, 1.0f );
    }
    

    bool UseMana(int cost)
    {
        if( cost < baseMana )
        {
            baseMana -= cost;
            ScaleManaBar();
            return true;
        }
        else
        {
            return false;
        }
    }

    void CalculateHealth()
    {
        if( baseHealth < 0)
        {
            //player down!
            Debug.Log("player died!");
            Destroy( this.gameObject );
        }
        else
        {
            healthBar.transform.localScale = new Vector3( (float)baseHealth / maxHealth, 1.0f, 1.0f );
        }

    }
}
