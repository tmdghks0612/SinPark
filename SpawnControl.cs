using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class SpawnControl : MonoBehaviour
{
    // control instances
    public GameControl gameControl;
    public CombatControl combatControl;

    // mana related variables
    private readonly object lock_mana = new object();
    private bool manaFlag;

    // mana related variables
    private float maxMana = 100;
    private float regenAmount = 10f;

    [SerializeField]
    private float baseMana;

    private int[] friendlyCreatureManaCost;
    private int[] hostileCreatureManaCost;

    public Image manaBar;

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


    private void Update()
    {
        if (baseMana + regenAmount * Time.deltaTime > maxMana)
        {
            baseMana = maxMana;
        }
        else
        {
            baseMana += regenAmount * Time.deltaTime;
        }
        manaBar.fillAmount = baseMana / maxMana;
    }

    // Start is called before the first frame update
    public void SpawnControlStart()
    {
        // variable initialization according to GameControl
        maxLanes = gameControl.GetMaxLanes();
        maxUnits = gameControl.GetMaxUnits();

        // data structure initializations
        InitLaneCoords();
        InitStage();

        // initialize combat control
        combatControl.InitCombatControl();
        currentCreature = friendlyCreatureList[0].GetComponent<DefaultCreature>();

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
        Debug.Log("Time : " + System.DateTime.Now);
        if (side == GameControl.Sides.Friendly)
        {
            newObject = Instantiate<GameObject>(friendlyCreatureList[buttonNum]);
        }
        else
        {
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
        return manaFlag;
    }

    #endregion

    //initialize coordinates of start, end of lanes
    void InitLaneCoords()
    {
        startCoord = new Vector3[maxLanes];
        endCoord = new Vector3[maxLanes];

        float cameraHeight = Camera.main.orthographicSize * 2;
        Vector2 cameraSize = new Vector2(Camera.main.aspect * cameraHeight, cameraHeight);

        float yPosition = cameraSize.y / 90;
        for (int i=0; i < maxLanes; ++i)
        {
            startCoord[i] = new Vector3(-15.0f, -yPosition * 18 + yPosition * i * 14, 0);
            endCoord[i] = new Vector3(15.0f, -yPosition * 18 + yPosition * i * 14, 0 );
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

    // summon friendly and hostile base
    public void SummonBoss()
    {
        /*for (int i = 0; i < 3; i++)
        {
            //create three invisible creatures to take all lane's damage
            SummonCreature(i, GameControl.Sides.Hostile, 0);
        }*/

        for (int i = 0; i < PublicLevel.usingLaneNum; ++i)
        {
            //spawn an actor through instantiate
            GameObject newObject;
            DefaultCreature newCreature; ;
            newObject = Instantiate<GameObject>(PublicLevel.GetBossPrefab());

            // pass control instances to each of the creatures
            newCreature = newObject.GetComponent<DefaultCreature>();
            newCreature.SetGameControl(gameControl);
            newCreature.SetCombatControl(combatControl);

            // set start and end coordinates for spawning creature
            startCoord[i] -= new Vector3(0, 0, layerOffset);
            endCoord[i] -= new Vector3(0, 0, layerOffset);
            newCreature.SetCreature(endCoord[i], startCoord[i], 0, i, GameControl.Sides.Hostile);

            newObject.tag = "Boss";

            // push creature into the list(side, lane) in combatControl
            combatControl.PushCreature(i, GameControl.Sides.Hostile, newCreature);
        }
    }

    // summon friendly and hostile base
    public void DeadAllHostileCreature()
    {
        for (int i = PublicLevel.usingCreatureNum - 1; i >= 0; i--)
        {
            // find objects depending on lane
            GameObject[] currentCreatureLane = GameObject.FindGameObjectsWithTag("Lane" + i.ToString());
            
            DefaultCreature deletingCreature;
            for (int j = currentCreatureLane.Length - 1; j >= 0; --j)
            {
                deletingCreature = currentCreatureLane[j].GetComponent<DefaultCreature>();
                // when it is not a valid creature
                if (deletingCreature == null)
                {
                    continue;
                }
                // when it is a hostile creature
                else if (deletingCreature.GetSide() == GameControl.Sides.Hostile)
                {
                    // creature dies (deleted frop list and gameObject destroyed)
                    deletingCreature.Dead();
                }
            }
        }
    }

}

