using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class AIplayer : MonoBehaviour
{
    public SpawnControl spawnControl;

    [SerializeField]
    private int currentMana;

    [SerializeField]
    private int manaAmount;
    [SerializeField]
    private float manaRegenTime;
    [SerializeField]
    private float creatureSpawnTime;
    [SerializeField]
    private DefaultCreature[] creatureArray = new DefaultCreature[5];
    [SerializeField]
    private int[] creatureType = new int[5];
    [SerializeField]
    private int[] upgradeType = new int[5];
    [SerializeField]
    private int[] creatureFlag = new int[5];
    [SerializeField]
    private float[] creatureRatio = new float[5];

    private int minimumCost;

    // Start is called before the first frame update
    public void AIplayerStart()
    {
        InitAI();
        InvokeRepeating("GainMana", manaRegenTime, manaRegenTime);
        InvokeRepeating("ChooseCreature", creatureSpawnTime, creatureSpawnTime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void InitAI()
    {
        currentMana = 0;
        this.manaAmount = PublicLevel.GetManaAmount();
        this.manaRegenTime = PublicLevel.GetManaRegenTime();
        this.creatureSpawnTime = PublicLevel.GetCreatureSpawnTime();
        PublicLevel.SetCreatureType(creatureType);
        PublicLevel.SetUpgradeType(upgradeType);

        SetCreatureArray();
        minimumCost = GetMinimumCost();
        SetCreatureRatio();
        SetCreatureFlag();
    }

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
            currentIndex = UnityEngine.Random.Range(0, 5);
            if (creatureFlag[currentIndex]  == 0)
            {
                continue;
            }
            else
            {
                currentLane = UnityEngine.Random.Range(0, 3);
                UseMana(creatureArray[currentIndex].GetManaCost());
                creatureFlag[currentIndex]--;
                spawnControl.SpawnCreatureLane(currentLane, GameControl.Sides.Hostile, creatureType[currentIndex], upgradeType[currentIndex]);
                break;
            }
        }
    }

    void SetCreatureRatio()
    {
        for (int i = 0; i < 5; ++i)
        {
            creatureRatio[i] = (float)Math.Round( (manaAmount / manaRegenTime) / (creatureArray[i].GetManaCost() / creatureSpawnTime), 2 );
        }
    }

    void SetCreatureFlag()
    {
        for (int i = 0; i < 5; ++i)
        {
            creatureFlag[i] = Mathf.RoundToInt( creatureRatio[i] * manaRegenTime / creatureSpawnTime );
        }
    }

    void GainMana()
    {
        currentMana += manaAmount;
        SetCreatureFlag();
    }

    void UseMana(int currentManaCost)
    {
        currentMana -= currentManaCost;
    }

    void SetCreatureArray()
    {
        Debug.Log("prefab using!");
        for (int i = 0; i < 5; ++i)
        {
            creatureArray[i] = spawnControl.prefabArray[creatureType[i], upgradeType[i]].GetComponent<DefaultCreature>();
        }
    }

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
