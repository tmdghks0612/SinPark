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
        return manaAmount;
    }
    public static float GetManaRegenTime()
    {
        return manaRegenTime;
    }
    public static float GetCreatureSpawnTime()
    {
        return creatureSpawnTime;
    }
    public static void SetCreatureType(int[] _creatureType)
    {
        for(int i=0; i < 5; ++i)
        {
            _creatureType[i] = creatureType[i];
        }
    }

    public static void SetUpgradeType(int[] _upgradeType)
    {
        for (int i = 0; i < 5; ++i)
        {
            _upgradeType[i] = upgradeType[i];
        }
    }
}
