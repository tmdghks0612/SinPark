﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnControl : MonoBehaviour
{
    public GameControl gameControl;
    public CombatControl combatControl;

    public GameObject manaBar;

    // maxLanes according to CombatControl
    private int maxLanes;
    private int maxUnits;

    //number of type of creatures
    private int typeCreature;

    private DefaultCreature currentCreature;

    private int playerCreatureNum = 0;

    Vector2[] startCoord;
    Vector2[] endCoord;

    //mana related variables
    private int maxMana = 100;
    private int regenAmount = 5;
    private float regenTime = 0.5f;
    [SerializeField]
    private int baseMana;


    public GameObject[] prefabArray;

    // Start is called before the first frame update
    void Start()
    {
        maxLanes = gameControl.GetMaxLanes();
        maxUnits = gameControl.GetMaxUnits();
        typeCreature = gameControl.typeCreature;

        InitLaneCoords();
        InitPrefabs();

        //default creature
        combatControl.InitLanes();
        currentCreature = prefabArray[0].GetComponent<DefaultCreature>();
        SpawnCreatureLane( 0, GameControl.Sides.Friendly, 0) ;

        this.manaBar = GameObject.Find("/GameControl/ManaContainerPrefab/Bar");
        InvokeRepeating("GainMana", 0.5f, regenTime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnCreatureLane(int laneNum, GameControl.Sides side, int creatureType)
    {
        if (playerCreatureNum == maxUnits)
        {
            //when unit is full
            return;
        }
        playerCreatureNum++;

        //summon a certaine creature in lane
        SummonCreature(laneNum, side, creatureType);
    }

    void SummonCreature(int laneNum, GameControl.Sides side, int creatureType)
    {
        //spawn an actor through instantiate
        GameObject newObject;
        DefaultCreature newCreature;


        Debug.Log("creature type is " + creatureType.ToString());
        newObject = Instantiate<GameObject>(prefabArray[creatureType]);
        newCreature = newObject.GetComponent<DefaultCreature>();

        newCreature.SetGameControl(gameControl);
        newCreature.SetCombatControl(combatControl);

        if(side == GameControl.Sides.Friendly)
        {
            newCreature.SetCreature(startCoord[laneNum], endCoord[laneNum], creatureType, laneNum, side);
        }
        else
        {
            newCreature.SetCreature(endCoord[laneNum], startCoord[laneNum], creatureType, laneNum, side);
        }

        combatControl.PushCreature(laneNum, side, newCreature);
    }

    //mana related functions
    bool UseMana(int cost)
    {
        if (cost < baseMana)
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

    void ScaleManaBar()
    {
        manaBar.transform.localScale = new Vector3((float)baseMana / maxMana, 1.0f, 1.0f);
    }

    //initialization functions

    //initialize coordinates of start, end of lanes
    void InitLaneCoords()
    {
        startCoord = new Vector2[maxLanes];
        endCoord = new Vector2[maxLanes];

        for(int i=0; i < maxLanes; ++i)
        {
            startCoord[i] = new Vector2(-15.0f, i * 2.0f);
            endCoord[i] = new Vector2(15.0f, i * 2.0f);
        }
    }

    void InitPrefabs()
    {
        prefabArray = new GameObject[typeCreature];
        //find and load creature prefabs from folder 'creature#'
        for (int i = 0; i < typeCreature; ++i)
        {
            prefabArray[i] = Resources.Load("creature" + i.ToString() + "/creature" + i.ToString() + "Prefab") as GameObject;
        }
    }
}
