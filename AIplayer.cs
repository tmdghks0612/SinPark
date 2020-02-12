using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class AIplayer : MonoBehaviour
{
    public SpawnControl spawnControl;

    // mana related variables
    [SerializeField]
    private int currentMana;
    [SerializeField]
    private int manaAmount;
    [SerializeField]
    private float manaRegenTime;

    // array of total creature types of hostile side
    [SerializeField]
    private DefaultCreature[] creatureArray;
    // array of spawnable number of creature
    [SerializeField]
    private int[] creatureFlag = new int[PublicLevel.hostileTypeCreatureNum];
    // array of ratio of creature
    [SerializeField]
    private float[] creatureRatio = new float[PublicLevel.hostileTypeCreatureNum];
    // array of spawnable creature types
    [SerializeField]
    private GameObject[] hostileCreatureList;

    // time interval between creature spawns
    [SerializeField]
    private float creatureSpawnTime;
    // minimum cost of spawnable minions
    private int minimumCost;

    // Overall Initialization of AIPlayer in the scene, called in GameControl
    public void AIplayerStart()
    {
        creatureArray = new DefaultCreature[PublicLevel.hostileTypeCreatureNum];
        InitAI();
        InvokeRepeating("GainMana", manaRegenTime, manaRegenTime);
        InvokeRepeating("ChooseCreature", creatureSpawnTime, creatureSpawnTime);
    }

    // Stop spawn and mana gain of aiplayer
    public void AIplayerStop()
    {
        CancelInvoke("GainMana");
        CancelInvoke("ChooseCreature");
    }

    // Initialization of variables and data structures in AIPlayer
    void InitAI()
    {
        currentMana = 0;
        // initialize mana variables according to PublicLevel
        this.manaAmount = PublicLevel.GetManaAmount();
        this.manaRegenTime = PublicLevel.GetManaRegenTime();
        this.creatureSpawnTime = PublicLevel.GetCreatureSpawnTime();
        hostileCreatureList = new GameObject[PublicLevel.hostileTypeCreatureNum];
        PublicLevel.GetHostileCreatureList(hostileCreatureList);

        // deciding how much creatures will be spawned, and its composition
        SetCreatureArray();
        minimumCost = GetMinimumCost();
        SetCreatureRatio();
        SetCreatureFlag();
    }

    // choosing next creature to spawn, regarding flags and random generator
    void ChooseCreature()
    {
        int currentIndex;
        int currentLane;
        if (minimumCost > currentMana)
        {
            //when none of the unit is available
            return;
        }

        while (true)
        {
            currentIndex = UnityEngine.Random.Range(1, 5);
            if (creatureFlag[currentIndex]  == 0)
            {
                // when chosen unit was spawned too much
                continue;
            }
            else
            {
                // choose random lane and spawn the creature
                currentLane = UnityEngine.Random.Range(0, 3);
                UseMana(creatureArray[currentIndex].GetManaCost());
                creatureFlag[currentIndex]--;
                spawnControl.SummonCreature(currentLane, GameControl.Sides.Hostile, currentIndex);
                break;
            }
        }
    }

    // decide how frequently a creature is going to be spawned
    void SetCreatureRatio()
    {
        float totalRatio = 0.0f;
        for (int i = 0; i < 5; ++i)
        {
            // maximum number of creatures spawn possible, regarding managain rate
            creatureRatio[i] = (float)Math.Round( (manaAmount / manaRegenTime) / (creatureArray[i].GetManaCost() / creatureSpawnTime), 2 );
            totalRatio += creatureRatio[i];
        }
        for (int i = 0; i < 5; ++i)
        {
            // ratio of creature spawn, relatively
            creatureRatio[i] = (float)Math.Round( creatureRatio[i] / totalRatio, 2);
        }
    }

    // decide the number of creatures spawn possible in certain period
    void SetCreatureFlag()
    {
        for (int i = 0; i < 5; ++i)
        {
            // multiply ratio of creature to get maximum number of creatures spawned in period
            creatureFlag[i] = Mathf.RoundToInt( creatureRatio[i] * manaRegenTime / creatureSpawnTime ) ;
            if(creatureFlag[i] == 0)
            {
                // expensive units can still be spawned
                creatureFlag[i] = 1;
            }
        }
    }

    // increase currentMana
    void GainMana()
    {
        currentMana += manaAmount;
        SetCreatureFlag();
    }

    // decrease currentMana
    void UseMana(int currentManaCost)
    {
        currentMana -= currentManaCost;
    }

    // creature array will use only a part of total type of creatures
    void SetCreatureArray()
    {
        for (int i = 0; i < PublicLevel.hostileTypeCreatureNum; ++i)
        {
            creatureArray[i] = hostileCreatureList[i].GetComponent<DefaultCreature>();
        }
    }

    // get lowest cost of spawnable creatures
    int GetMinimumCost()
    {
        minimumCost = int.MaxValue;
        int currentCost = 0;
        for(int i = 0; i < 5; ++i)
        {
            currentCost = creatureArray[i].GetManaCost();
            if (minimumCost > currentCost)
            {
                //when new minimum cost is found
                minimumCost = currentCost;
            }
        }
        return minimumCost;
    }
}
