using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;

public static class PublicLevel
{
    //Total number of creatures and upgrades : friendly side
    public readonly static int friendlyTypeCreatureNum = 8;
    public readonly static int friendlyTypeUpgradeNum = 1;

    //Total number of creatures and upgrades : hostile side
    public readonly static int hostileTypeCreatureNum = 8;
    public readonly static int hostileTypeUpgradeNum = 1;

    // number of actually using creatures' number
    public static int usingCreatureNum = 5;
    public static int usingLaneNum = 3;

    // Adjust difficulties of the stage by changing mana regenration amount/period and spawn cooldown
    [SerializeField]
    private static int manaAmount;
    [SerializeField]
    private static float manaRegenTime;
    [SerializeField]
    private static float creatureSpawnTime;
    [SerializeField]
    private static bool isBoss;

    private static int cornNum;


    [SerializeField]
    private static GameObject bossPrefab;

    // List of every creature prefabs loaded from resources folder
    public static GameObject[,] friendlyPrefab;
    public static GameObject[,] hostilePrefab;

    public static bool[,] unlockType;

    // List of images loaded from resources folder
    public static Sprite[,] friendlyImage;

    // List of images which will be used in actual game.
    public static Sprite[] friendlyImageList;

    // Save type of creature/upgrade which will be used in actual game.
    public static Vector2Int[] friendlyType;
    public static Vector2Int[] hostileType;

    // Save list of GameObjects which will be used in actual game.
    public static GameObject[] hostileCreatureList;
    public static GameObject[] friendlyCreatureList;
    
    // Player Profile. These parameters will be saved and loaded by GameDataControl.
    private static readonly int playerMaxLevel = 10;
    private static int playerLevel;
    private static int playerWin;

    // stage level which will be set before starting actual game.
    private static int stageLevel;

    private static GameData gameData;

    private static NetworkStream serverStream;

    // Used by AIController to know which creature to spawn.
    public static void GetHostileCreatureList(GameObject[] _hostileCreatureList)
    {
        Debug.Log(_hostileCreatureList.Length + " " + hostileCreatureList.Length + " " + hostileTypeCreatureNum);
        for(int i = 0; i < hostileTypeCreatureNum; ++i)
        {
            _hostileCreatureList[i] = hostileCreatureList[i];
        }
    }

    public static Vector2Int[] GetFriendlyType()
    {
        return friendlyType;
    }

    // Called when stage enter button is clicked. Save settings of the stage which is stored in the button to PublicLevel
    public static void SetLevel(Vector2Int[] _hostileType, int _manaAmount, float _manaRegenTime, float _creatureSpawnTime, int _stageLevel, bool _isBoss, GameObject _bossPrefab)
    {
        //Difficulty adjustments
        manaAmount = _manaAmount;
        manaRegenTime = _manaRegenTime;
        creatureSpawnTime = _creatureSpawnTime;
        isBoss = _isBoss;

        bossPrefab = _bossPrefab;
        
        // Set hostile creature list used by AIController based on hostileType[]
        for(int i = 0; i < hostileTypeCreatureNum; ++i)
        { 
            hostileType[i] = _hostileType[i];

            hostileCreatureList[i] = hostilePrefab[(int)hostileType[i].x, (int)hostileType[i].y];
        }
        hostileType = _hostileType;
        stageLevel = _stageLevel;
    }

    // Basic Setting of the game before going into StageSelect Scene.
    // Initialize lists using in PublicLevel
    // Load prefabs and image of creatures and save it at list
    public static void InitSetting()
    {
        // Initialize every friendly-related lists. Every Prefab, Using Prefab, Every Image, Using Image, Using Type.
        friendlyPrefab = new GameObject[friendlyTypeCreatureNum, friendlyTypeUpgradeNum];
        friendlyCreatureList = new GameObject[friendlyTypeCreatureNum];
        friendlyImage = new Sprite[friendlyTypeCreatureNum, friendlyTypeUpgradeNum];
        friendlyImageList = new Sprite[friendlyTypeCreatureNum];
        friendlyType = new Vector2Int[friendlyTypeCreatureNum];

        // Initialize every hostile-related lists. Every Prefab, Using Prefab, Using Type.
        hostilePrefab = new GameObject[hostileTypeCreatureNum, hostileTypeUpgradeNum];
        hostileCreatureList = new GameObject[hostileTypeCreatureNum];
        hostileType = new Vector2Int[hostileTypeCreatureNum];

        unlockType = new bool[friendlyTypeCreatureNum, friendlyTypeUpgradeNum];


        // find and load friendly creature prefabs and images from folder 'creature#_#'
        for (int i = 0; i < friendlyTypeCreatureNum; ++i)
        {
            for (int k = 0; k < friendlyTypeUpgradeNum; ++k)
            {
                friendlyPrefab[i, k] = Resources.Load("creature" + i.ToString() + "/creature" + i.ToString() + "_" + k.ToString() + "/creature" + i.ToString() + "_" + k.ToString() + "Prefab") as GameObject;
                friendlyImage[i,k] = Resources.Load<Sprite>("creature" + i.ToString() + "/creature" + i.ToString() + "_" + k.ToString() + "/creature" + i.ToString() + "_" + k.ToString() + "Image") as Sprite;
                unlockType[i, k] = false;
            }
        }

        // find and load friendly creature prefabs from folder 'creature#_#'
        for (int i = 0; i < hostileTypeCreatureNum; ++i)
        {
            for (int k = 0; k < hostileTypeUpgradeNum; ++k)
            {
                hostilePrefab[i, k] = Resources.Load("creature" + i.ToString() + "/creature" + i.ToString() + "_" + k.ToString() + "/creature" + i.ToString() + "_" + k.ToString() + "Prefab") as GameObject;
            }
        }

    }

    // Called by Spawn Control. Set friendlyArray and hostile Array based on friendlyCreature List and hostileCreatureList
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

    public static void UpdateFriendlyList(int _location, Vector2Int _changingInfo)
    {
        PublicLevel.friendlyCreatureList[_location] = PublicLevel.friendlyPrefab[_changingInfo.x, _changingInfo.y];
        PublicLevel.friendlyImageList[_location] = PublicLevel.friendlyImage[_changingInfo.x, _changingInfo.y];
        PublicLevel.friendlyType[_location] = _changingInfo;
    }

    #region Set functions
    // Called when game ends, or attempts to load. Set Player's level. Player's level only maintains or increases, not decreases
    public static void SetPlayerLevel(int newLevel)
    {
        if(newLevel > playerMaxLevel)
        {
            playerLevel = playerMaxLevel;
            SetCorn(GetCorn() + 15);
        }
        else if(newLevel > playerLevel)
        {
            playerLevel = newLevel;
            SetCorn(GetCorn() + 30);
        }
        else
            SetCorn(GetCorn() + 15);
    }

    // Set Player's win number. Works only at multi mode
    public static void SetPlayerWin(int newWin)
    {
        if (newWin < int.MaxValue)
        {
            playerWin = newWin;
        }
    }

    // Set server stream
    public static void SetServerStream(NetworkStream _serverStream)
    {
        serverStream = _serverStream;
    }

    // Set PublicLevel variable according to stagebutton information
    public static void SetIsBoss(bool _isBoss)
    {
        isBoss = _isBoss;
    }

    #endregion

    public static void SetCorn(int currentCorn)
    {
        cornNum = currentCorn;
    }


    #region Get functions

    //Used to get player's current level
    public static int GetPlayerLevel()
    {
        return playerLevel;
    }

    //Used to get player's current corn
    public static int GetCorn()
    {
        return cornNum;
    }

    //Used to get player's curren win number
    public static int GetPlayerWin()
    {
        return playerWin;
    }

    //Used to get current stage level
    public static int GetStageLevel()
    {
        return stageLevel;
    }

    //return manaAmount. Used by AIController to know amount of mana gain
    public static int GetManaAmount()
    {
        return manaAmount;
    }

    //return manaRegenTime. Used by AIController to know period of mana gain
    public static float GetManaRegenTime()
    {
        return manaRegenTime;
    }

    //return manaAmount. Used by AIController to know cooldown of creature spawn
    public static float GetCreatureSpawnTime()
    {
        return creatureSpawnTime;
    }

    // return if this stage contains boss
    public static bool GetIsBoss()
    {
        return isBoss;
    }

    public static GameObject GetBossPrefab()
    {
        return bossPrefab;
    }

    // Get server stream
    public static NetworkStream GetServerStream()
    {
        return serverStream;
    }
    #endregion
}
