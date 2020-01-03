using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnControl : MonoBehaviour
{
    GameControl gameControl;
    CombatControl combatControl;

    // maxLanes according to CombatControl
    private int maxLanes;
    private int maxUnits;

    private int playerCreatureNum = 0;

    Vector3[] startCoord;
    Vector3[] endCoord;

    // Start is called before the first frame update
    void Start()
    {
        maxLanes = gameControl.maxLanes;
        maxUnits = gameControl.maxUnits;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnCreatureLane(int laneNum, GameControl.Sides side, int creatureType)
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

    void SummonCreature(int laneNum, GameControl.Sides side, int creatueType)
    {
        //spawn an actor through instantiate

        //start and end coordinates given through startCoord and endCoord
    }

    //initialization functions

    //initialize coordinates of start, end of lanes
    void InitLaneCoords()
    {
        startCoord = new Vector3[maxLanes];
        endCoord = new Vector3[maxLanes];

        for(int i=0; i<maxLanes; ++i)
        {
            startCoord[i] = new Vector3(-200.0f, 0.0f, i * 100.0f);
            endCoord[i] = new Vector3(200.0f, 0.0f, i * 100.0f);
        }
    }
}
