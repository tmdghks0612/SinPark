using System.Collections;
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
    private int typeUpgrade;

    private DefaultCreature currentCreature;

    private int playerCreatureNum = 0;

    Vector2[] startCoord;
    Vector2[] endCoord;

    public GameObject[,] prefabArray;

    //mana related variables
    private int maxMana = 100;
    private int regenAmount = 5;
    private float regenTime = 0.5f;
    [SerializeField]
    private int baseMana;


    // Start is called before the first frame update
    void Start()
    {
        maxLanes = gameControl.GetMaxLanes();
        maxUnits = gameControl.GetMaxUnits();
        typeCreature = gameControl.typeCreature;
        typeUpgrade = gameControl.typeUpgrade;
        InitLaneCoords();
        InitPrefabs();

        //default creature
        combatControl.InitLanes();
        currentCreature = prefabArray[0,0].GetComponent<DefaultCreature>();

        this.manaBar = GameObject.Find("/GameControl/ManaContainerPrefab/Bar");
        InvokeRepeating("GainMana", 0.5f, regenTime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnCreatureLane(int laneNum, GameControl.Sides side, int creatureType, int upgradeType)
    {
        if (playerCreatureNum == maxUnits)
        {
            //when unit is full
            return;
        }
        playerCreatureNum++;

        //summon a certaine creature in lane
        SummonCreature(laneNum, side, creatureType, upgradeType);
    }

    void SummonCreature(int laneNum, GameControl.Sides side, int creatureType, int upgradeType)
    {
        //spawn an actor through instantiate
        GameObject newObject;
        DefaultCreature newCreature;


        Debug.Log("creature type is " + creatureType.ToString());
        newObject = Instantiate<GameObject>(prefabArray[creatureType,upgradeType]); // 일단 0으로 설정 후에 변수 upgradeType으로 바꿔줘야한다. 함수 자체를 바꿔야 하기에 일단은 0 넣음
        newCreature = newObject.GetComponent<DefaultCreature>();

        newCreature.SetGameControl(gameControl);
        newCreature.SetCombatControl(combatControl);

        if(side == GameControl.Sides.Friendly)
        {
            newCreature.SetCreature(startCoord[laneNum], endCoord[laneNum], creatureType, upgradeType, laneNum, side);
        }
        else
        {
            newCreature.SetCreature(endCoord[laneNum], startCoord[laneNum], creatureType, upgradeType, laneNum, side);
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
        prefabArray = new GameObject[typeCreature,typeUpgrade];
        //find and load creature prefabs from folder 'creature#'
        for (int i = 0; i < typeCreature; ++i)
        {
            for(int k = 0; k < typeUpgrade; ++k)
            {
                prefabArray[i, k] = Resources.Load("creature" + i.ToString() + "/creature" + i.ToString() + "_" + k.ToString() + "/creature" + i.ToString() + "_" + k.ToString() + "Prefab") as GameObject;
            }
        }
    }
}
