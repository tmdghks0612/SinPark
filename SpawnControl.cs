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
    private int typeCreature;

    private DefaultCreature currentCreature;

    private int playerCreatureNum = 0;

    Vector2[] startCoord;
    Vector2[] endCoord;

    public GameObject[] prefabArray;

    // Start is called before the first frame update
    void Start()
    {
        maxLanes = gameControl.maxLanes;
        maxUnits = gameControl.maxUnits;
        typeCreature = gameControl.typeCreature;

        InitLaneCoords();
        InitPrefabs();

        //default creature
        combatControl.InitLanes();
        currentCreature = prefabArray[0].GetComponent<DefaultCreature>();
        
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

        newCreature.gameControl = gameControl;
        newCreature.combatControl = combatControl;

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
        for(int i=0; i < typeCreature; ++i)
        {
            prefabArray[i] = Resources.Load("creature" + i.ToString() + "/creature" + i.ToString() + "Prefab") as GameObject;
        }
    }
}
