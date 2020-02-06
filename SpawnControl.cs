using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnControl : MonoBehaviour
{
    // control instances
    public GameControl gameControl;
    public CombatControl combatControl;

    // mana related variables
    private readonly object lock_mana = new object();
    private bool manaFlag;

    // mana related variables
    private int maxMana = 100;
    private int regenAmount = 5;
    private float regenTime = 0.5f;

    [SerializeField]
    private int baseMana;

    private int[] friendlyCreatureManaCost;
    private int[] hostileCreatureManaCost;

    public GameObject manaBar;

    // maxLanes according to CombatControl
    private int maxLanes;
    private int maxUnits;

    // player selected creature
    private DefaultCreature currentCreature;

    //number of player creatures in the scene
    private int playerCreatureNum = 0;
    //offset of z position difference for display
    private float layerOffset = 0.01f;

    // creature start and end coordinates
    Vector3[] startCoord;
    Vector3[] endCoord;

    // creature prefab list for friendly and hostile player
    public GameObject[] friendlyCreatureList;
    public GameObject[] hostileCreatureList;

    // Start is called before the first frame update
    public void SpawnControlStart()
    {
        // variable initialization according to GameControl
        maxLanes = gameControl.GetMaxLanes();
        maxUnits = gameControl.GetMaxUnits();

        // data structure initializations
        InitLaneCoords();
        InitStage();
        ScaleManaBar();

        // initialize combat control
        combatControl.InitCombatControl();
        currentCreature = friendlyCreatureList[0].GetComponent<DefaultCreature>();

        // regenerate mana
        InvokeRepeating("GainMana", regenTime, regenTime);
        // summon friendly and hostile base
        SummonBase();
    }

    // spawn selected craeture in selected lane
    public void SpawnCreatureLane(int laneNum, GameControl.Sides side, int buttonNum)
    {
        if (playerCreatureNum == maxUnits)
        {
            // when player spawned too much creatures
            return;
        }
        else if(!UseMana(friendlyCreatureManaCost[buttonNum]))
        {
            // when insufficient mana to spawn such creature
            return;
        }
        playerCreatureNum++;
        SummonCreature(laneNum, side, buttonNum);
    }

    // summon a creature in certain lane, instantiate in the scene
    public void SummonCreature(int laneNum, GameControl.Sides side, int buttonNum)
    {
        //spawn an actor through instantiate
        GameObject newObject;
        DefaultCreature newCreature;

        if (side == GameControl.Sides.Friendly)
        {
            newObject = Instantiate<GameObject>(friendlyCreatureList[buttonNum]);
        }
        else
        {
            Debug.Log("current number : " + buttonNum);
            newObject = Instantiate<GameObject>(hostileCreatureList[buttonNum]);
        }

        // pass control instances to each of the creatures
        newCreature = newObject.GetComponent<DefaultCreature>();
        newCreature.SetGameControl(gameControl);
        newCreature.SetCombatControl(combatControl);

        // set start and end coordinates for spawning creature
        if(side == GameControl.Sides.Friendly)
        {
            startCoord[laneNum] -= new Vector3(0, 0, layerOffset);
            endCoord[laneNum] -= new Vector3(0, 0, layerOffset);
            newCreature.SetCreature(startCoord[laneNum], endCoord[laneNum], buttonNum, laneNum, side);
        }
        else
        {
            startCoord[laneNum] -= new Vector3(0, 0, layerOffset);
            endCoord[laneNum] -= new Vector3(0, 0, layerOffset);
            newCreature.SetCreature(endCoord[laneNum], startCoord[laneNum], buttonNum, laneNum, side);
        }

        // push creature into the list(side, lane) in combatControl
        combatControl.PushCreature(laneNum, side, newCreature);
    }

    #region mana related functions

    // use user mana
    bool UseMana(int cost)
    {
        lock (lock_mana)
        {
            if (cost < baseMana)
            {
                baseMana -= cost;
                manaFlag = true;
            }
            else
            {
                manaFlag = false;
            }
        }
        if(manaFlag == true)
        {
            ScaleManaBar();
        }
        return manaFlag;
    }

    void GainMana()
    {
        if (baseMana + regenAmount > maxMana)
        {
            baseMana = maxMana;
        }
        else
        {
            baseMana += 5;
        }
        ScaleManaBar();
    }

    // resize mana bar for UI
    void ScaleManaBar()
    {
        manaBar.transform.localScale = new Vector3((float)baseMana / maxMana, 1.0f, 1.0f);
    }

    #endregion

    //initialize coordinates of start, end of lanes
    void InitLaneCoords()
    {
        startCoord = new Vector3[maxLanes];
        endCoord = new Vector3[maxLanes];

        for(int i=0; i < maxLanes; ++i)
        {
            startCoord[i] = new Vector3(-15.0f, i * 2.5f -1, 0);
            endCoord[i] = new Vector3(15.0f, i * 2.5f -1, 0 );
        }
    }

    public void OnUnitDeath()
    {
        playerCreatureNum--;
    }

    // initialize prefab list and its mana costs according to PublicLevel
    void InitStage()
    {
        friendlyCreatureList = new GameObject[PublicLevel.friendlyTypeCreatureNum];
        hostileCreatureList = new GameObject[PublicLevel.friendlyTypeCreatureNum];

        friendlyCreatureManaCost = new int[PublicLevel.friendlyTypeCreatureNum];
        hostileCreatureManaCost = new int[PublicLevel.friendlyTypeCreatureNum];

        PublicLevel.PlayerStageSetting(friendlyCreatureList,hostileCreatureList);
        
        for(int i=0; i<PublicLevel.friendlyTypeCreatureNum;i++)
        {
            friendlyCreatureManaCost[i] = friendlyCreatureList[i].GetComponent<DefaultCreature>().GetManaCost();
        }
    }

    // summon friendly and hostile base
    public void SummonBase()
    {
        for(int i=0; i<3; i++)
        {
            //create three invisible creatures to take all lane's damage
            SummonCreature(i, GameControl.Sides.Friendly, 0);
            SummonCreature(i, GameControl.Sides.Hostile, 0);
        }
    }
}

