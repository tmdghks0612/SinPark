using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Threading;
using System.Net;
using System.Net.Sockets;

public class StageButton : MonoBehaviour
{
    

    // parameter that handle difficulties of the game.
    // level of the stage
    [SerializeField]
    protected int stageLevel;
    [SerializeField]
    protected int manaAmount;
    [SerializeField]
    protected float manaRegenTime;
    [SerializeField]
    protected float creatureSpawnTime;
    // if this stage contains boss
    [SerializeField]
    protected bool isBoss;

    [SerializeField]
    private GameObject bossPrefab;

    //Type of creatures used by hostile
    [SerializeField]
    protected Vector2Int[] hostileType = new Vector2Int[PublicLevel.hostileTypeCreatureNum];

    //Save parameters stored in the button to the PublicLevel and load scene
    public virtual void InitLevel()
    {
        PublicLevel.SetLevel(hostileType, manaAmount, manaRegenTime, creatureSpawnTime, stageLevel, isBoss, bossPrefab);
        if (PublicLevel.GetTurorial())
        {
            PublicLevel.SetTutorial(false);
            SceneManager.LoadScene("Tutorial");
        }
        else
        {
            LoadingSceneManager.LoadScene("DefaultIngame");
        }
    }

    

    #region Get functions
    public int GetStageLevel()
    {
        return stageLevel;
    }
    #endregion

    #region Set functions

    

    #endregion
}

