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
    private DefaultCreature[] creatureArray = new DefaultCreature[7];
    [SerializeField]
    private int[] creatureFlag = new int[7];
    [SerializeField]
    private float[] creatureRatio = new float[7];
    [SerializeField]
    private GameObject[] hostileCreatureList = new GameObject[7];

    private int minimumCost;

    // Start is called before the first frame update
    public void AIplayerStart()
    {
        creatureArray = new DefaultCreature[PublicLevel.friendlyTypeCreatureNum];
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
        PublicLevel.getHostileCreatureList(hostileCreatureList);

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
            currentIndex = UnityEngine.Random.Range(1, 5);
            if (creatureFlag[currentIndex]  == 0)
            {
                continue;
            }
            else
            {
                currentLane = UnityEngine.Random.Range(0, 3);
                UseMana(creatureArray[currentIndex].GetManaCost());
                creatureFlag[currentIndex]--;
                spawnControl.SummonCreature(currentLane, GameControl.Sides.Hostile, currentIndex);
                break;
            }
        }
    }

    void SetCreatureRatio()
    {
        float totalRatio = 0.0f;
        for (int i = 0; i < 5; ++i)
        {
            creatureRatio[i] = (float)Math.Round( (manaAmount / manaRegenTime) / (creatureArray[i].GetManaCost() / creatureSpawnTime), 2 );
            totalRatio += creatureRatio[i];
        }
        for (int i = 0; i < 5; ++i)
        {
            creatureRatio[i] = (float)Math.Round( creatureRatio[i] / totalRatio, 2);
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
            Debug.Log(hostileCreatureList[i]);
            creatureArray[i] = hostileCreatureList[i].GetComponent<DefaultCreature>();
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
