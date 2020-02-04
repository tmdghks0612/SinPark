using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PublicLevel
{
    public readonly static int friendlyTypeCreatureNum = 7;
    public readonly static int friendlyTypeUpgradeNum = 1;

    public readonly static int hostileTypeCreatureNum = 7;
    public readonly static int hostileTypeUpgradeNum = 1;

    [SerializeField]
    public static int manaAmount = 30;
    [SerializeField]
    public static float manaRegenTime = 1.0f;
    [SerializeField]
    public static float creatureSpawnTime = 0.1f;

    public static GameObject[,] friendlyPrefab;
    public static GameObject[,] hostilePrefab;
    public static Sprite[,] friendlyImage;

    public static Vector2Int[] friendlyType;
    public static Vector2Int[] hostileType;

    static GameObject[] hostileCreatureList;
    public static GameObject[] friendlyCreatureList;
    public static Sprite[] friendlyImageList;

    private static readonly int playerMaxLevel = 10;
    private static int playerLevel;
    private static int playerWin;
    private static int stageLevel;

    public static int usingCreatureNum;

    private static GameData gameData;

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

    public static void getHostileCreatureList(GameObject[] _hostileCreatureList)
    {
        for(int i = 0; i < hostileTypeCreatureNum; ++i)
        {
            _hostileCreatureList[i] = hostileCreatureList[i];
        }
    }

    public static void SetLevel(Vector2Int[] _hostileType, int _manaAmount, float _manaRegenTime, float _creatureSpawnTime, int _stageLevel)
    {
        manaAmount = _manaAmount;
        manaRegenTime = _manaRegenTime;
        creatureSpawnTime = _creatureSpawnTime;
        for(int i = 0; i < hostileTypeCreatureNum; ++i)
        {
            hostileType[i] = _hostileType[i];

            hostileCreatureList[i] = hostilePrefab[(int)hostileType[i].x, (int)hostileType[i].y];
        }
        hostileType = _hostileType;
        stageLevel = _stageLevel;
    }

    public static void InitReady()
    {
        /*
        friendlyPrefab = new GameObject[friendlyTypeCreatureNum, friendlyTypeUpgradeNum];
        friendlyCreatureList = new GameObject[friendlyTypeCreatureNum];
        friendlyImage = new Sprite[friendlyTypeCreatureNum, friendlyTypeUpgradeNum];
        friendlyImageList = new Sprite[friendlyTypeCreatureNum];
        friendlyType = new Vector2Int[friendlyTypeCreatureNum];

        hostilePrefab = new GameObject[hostileTypeCreatureNum, hostileTypeUpgradeNum];
        hostileCreatureList = new GameObject[hostileTypeCreatureNum];
        hostileType = new Vector2Int[hostileTypeCreatureNum];
         */
    }

    public static void InitSetting()
    {
        friendlyPrefab = new GameObject[friendlyTypeCreatureNum, friendlyTypeUpgradeNum];
        friendlyCreatureList = new GameObject[friendlyTypeCreatureNum];
        friendlyImage = new Sprite[friendlyTypeCreatureNum, friendlyTypeUpgradeNum];
        friendlyImageList = new Sprite[friendlyTypeCreatureNum];
        friendlyType = new Vector2Int[friendlyTypeCreatureNum];

        hostilePrefab = new GameObject[hostileTypeCreatureNum, hostileTypeUpgradeNum];
        hostileCreatureList = new GameObject[hostileTypeCreatureNum];
        hostileType = new Vector2Int[hostileTypeCreatureNum];


        //find and load creature prefabs from folder 'creature#'
        for (int i = 0; i < friendlyTypeCreatureNum; ++i)
        {
            for (int k = 0; k < friendlyTypeUpgradeNum; ++k)
            {
                friendlyPrefab[i, k] = Resources.Load("creature" + i.ToString() + "/creature" + i.ToString() + "_" + k.ToString() + "/creature" + i.ToString() + "_" + k.ToString() + "Prefab") as GameObject;
                friendlyImage[i,k] = Resources.Load<Sprite>("creature" + i.ToString() + "/creature" + i.ToString() + "_" + k.ToString() + "/creature" + i.ToString() + "_" + k.ToString() + "Image") as Sprite;
            }
        }

        for (int i = 0; i < hostileTypeCreatureNum; ++i)
        {
            for (int k = 0; k < hostileTypeUpgradeNum; ++k)
            {
                hostilePrefab[i, k] = Resources.Load("creature" + i.ToString() + "/creature" + i.ToString() + "_" + k.ToString() + "/creature" + i.ToString() + "_" + k.ToString() + "Prefab") as GameObject;
            }
        }

    }

    public static void PlayerStageSetting(GameObject[] friendlyArray, GameObject[] hostileArray)
    {
        for(int i=0; i<friendlyTypeCreatureNum; i++)
        {
            friendlyArray[i] = friendlyCreatureList[i];
        }
        for (int i = 0; i < hostileTypeCreatureNum; i++)
        {
            hostileArray[i] = hostileCreatureList[i];
        }
    }

    public static void SetPlayerLevel(int newLevel)
    {
        if(newLevel > playerMaxLevel)
        {
            playerLevel = playerMaxLevel;
        }
        else if(newLevel > playerLevel)
        {
            playerLevel = newLevel;

            Debug.Log("playerlevel set to " + newLevel.ToString());
        }
    }

    public static void SetPlayerWin(int newWin)
    {
        if (newWin < int.MaxValue)
        {
            playerLevel = newWin;
        }
    }

    public static int GetPlayerLevel()
    {
        return playerLevel;
    }
    public static int GetPlayerWin()
    {
        return playerWin;
    }
    public static int GetStageLevel()
    {
        return stageLevel;
    }
}
