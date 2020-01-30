using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PublicLevel
{
    [SerializeField]
    public static int manaAmount = 30;
    [SerializeField]
    public static float manaRegenTime = 1.0f;
    [SerializeField]
    public static float creatureSpawnTime = 0.1f;
    [SerializeField]
    public static int[] creatureType = new int[5];
    [SerializeField]
    public static int[] upgradeType = new int[5];

    public static int GetManaAmount()
    {
        Debug.Log(manaAmount);
        return manaAmount;
    }
    public static float GetManaRegenTime()
    {
        Debug.Log(manaRegenTime);
        return manaRegenTime;
    }
    public static float GetCreatureSpawnTime()
    {
        Debug.Log(creatureSpawnTime);
        return creatureSpawnTime;
    }
    public static void GetCreatureType(int[] _creatureType)
    {
        for(int i=0; i < 5; ++i)
        {
            _creatureType[i] = creatureType[i];
        }
    }

    public static void GetUpgradeType(int[] _upgradeType)
    {
        for (int i = 0; i < 5; ++i)
        {
            _upgradeType[i] = upgradeType[i];
        }
    }

    public static void SetLevel(int[] _creatureType, int[] _upgradeType, int _manaAmount, float _manaRegenTime, float _creatureSpawnTime)
    {
        manaAmount = _manaAmount;
        manaRegenTime = _manaRegenTime;
        creatureSpawnTime = _creatureSpawnTime;
        for(int i = 0; i < 5; ++i)
        {
            creatureType[i] = _creatureType[i];
            upgradeType[i] = _upgradeType[i];
        }
        creatureType = _creatureType;
    }
}
