using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnControl : MonoBehaviour
{
    public GameControl gameControl;
    public CombatControl combatControl;

    // maxLanes according to CombatControl
    private int maxLanes;
    private int maxUnits;

    //number of type of creatures
    private int typeCreature = 2;

    private GameObject currentCreature;

    private int playerCreatureNum = 0;

    Vector3[] startCoord;
    Vector3[] endCoord;

    public GameObject[] prefabArray;

    // Start is called before the first frame update
    void Start()
    {
        maxLanes = gameControl.maxLanes;
        maxUnits = gameControl.maxUnits;

        InitLaneCoords();
        InitPrefabs();

        //default creature
        currentCreature = prefabArray[0];
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
    void SummonCreature(int laneNum, GameControl.Sides side, int creatureType)
    {
        //spawn an actor through instantiate
        GameObject newCreature;

        newCreature = Instantiate<GameObject>(prefabArray[creatureType]);
        
    }


    //initialization functions

    //initialize coordinates of start, end of lanes
    void InitLaneCoords()
    {
        startCoord = new Vector3[maxLanes];
        endCoord = new Vector3[maxLanes];

        for(int i=0; i < maxLanes; ++i)
        {
            startCoord[i] = new Vector3(-200.0f, i * 100.0f, 0.0f);
            endCoord[i] = new Vector3(200.0f, i * 100.0f, 0.0f);
        }
    }

    void InitPrefabs()
    {
        prefabArray = new GameObject[typeCreature];

        for(int i=0; i < typeCreature; ++i)
        {
            prefabArray[i] = Resources.Load("creature" + i.ToString() + "/creature" + i.ToString() + "Prefab") as GameObject;
        }
    }
}
