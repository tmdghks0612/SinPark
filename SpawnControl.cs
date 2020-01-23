using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnControl : MonoBehaviour
{
    private readonly object lock_mana = new object();
    private bool manaFlag;

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
    private float layerOffset = 0.01f;

    Vector3[] startCoord;
    Vector3[] endCoord;

    public GameObject[,] prefabArray;

    [SerializeField]
    private int[,] creatureManaCost;

    //mana related variables
    private int maxMana = 100;
    private int regenAmount = 5;
    private float regenTime = 0.5f;
    [SerializeField]
    private int baseMana;


    // Start is called before the first frame update
    public void SpawnControlStart()
    {
        maxLanes = gameControl.GetMaxLanes();
        maxUnits = gameControl.GetMaxUnits();
        typeCreature = gameControl.typeCreature;
        typeUpgrade = gameControl.typeUpgrade;
        InitLaneCoords();
        InitPrefabs();
        ScaleManaBar();
        //default creature
        combatControl.InitLanes();
        currentCreature = prefabArray[0,0].GetComponent<DefaultCreature>();

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
        else if(!UseMana(creatureManaCost[creatureType, upgradeType]))
        {
            return;
        }
        playerCreatureNum++;
        //UseMana(creatureManaCost[creatureType, upgradeType]);
        //summon a certaine creature in lane
        SummonCreature(laneNum, side, creatureType, upgradeType);
    }

    public void SummonCreature(int laneNum, GameControl.Sides side, int creatureType, int upgradeType)
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
            startCoord[laneNum] -= new Vector3(0, 0, layerOffset);
            endCoord[laneNum] -= new Vector3(0, 0, layerOffset);
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

    void ScaleManaBar()
    {
        manaBar.transform.localScale = new Vector3((float)baseMana / maxMana, 1.0f, 1.0f);
    }

    //initialization functions

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
    void InitPrefabs()
    {
        prefabArray = new GameObject[typeCreature,typeUpgrade];
        creatureManaCost = new int[typeCreature, typeUpgrade];
        //find and load creature prefabs from folder 'creature#'
        for (int i = 0; i < typeCreature; ++i)
        {
            for(int k = 0; k < typeUpgrade; ++k)
            {
                prefabArray[i, k] = Resources.Load("creature" + i.ToString() + "/creature" + i.ToString() + "_" + k.ToString() + "/creature" + i.ToString() + "_" + k.ToString() + "Prefab") as GameObject;
                //GameObject temporary = Instantiate<GameObject>(prefabArray[i, k]);
                creatureManaCost[i, k] = prefabArray[i, k].GetComponent<DefaultCreature>().GetManaCost();

            }
        }
        Debug.Log("prefab ready!");
    }
}
