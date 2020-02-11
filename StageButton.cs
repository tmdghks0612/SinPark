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
    // control instances
    [SerializeField]
    private GameObject networkErrorPanel;

    // parameter that handle difficulties of the game.
    // level of the stage
    [SerializeField]
    private int stageLevel;
    [SerializeField]
    private int manaAmount;
    [SerializeField]
    private float manaRegenTime;
    [SerializeField]
    private float creatureSpawnTime;
    // if this stage contains boss
    [SerializeField]
    private bool isBoss;
    

    [SerializeField]
    private GameObject bossPrefab;

    //Type of creatures used by hostile
    [SerializeField]
    private Vector2Int[] hostileType = new Vector2Int[PublicLevel.hostileTypeCreatureNum];
 
    
    //Save parameters stored in the button to the PublicLevel and load scene
    public void InitLevel()
    {
        PublicLevel.SetLevel(hostileType, manaAmount, manaRegenTime, creatureSpawnTime, stageLevel, isBoss, bossPrefab);
        LoadingSceneManager.LoadScene("DefaultIngame");
    }

    //Save parameters stored in the button to the PublicLevel and load scene
    public void InitLevelMultiplayer()
    {
        try
        {
            // when connection was success
            if (ServerControl.OpenStream())
            {
                PublicLevel.SetLevel(hostileType, manaAmount, manaRegenTime, creatureSpawnTime, stageLevel, false, null);
                if (ServerControl.SendCreatureList())
                {
                    StartCoroutine(ServerControl.ListenForHostileCreatureList(3.0f));
                }
                WaitForCreatureList();
            }
            // when connection was a failure
            else
            {
                // pop up network connection error panel
                NetworkErrorPanelactive();
            }
        }
        catch(System.Exception exception)
        {
            // pop up network connection error panel
            NetworkErrorPanelactive();
            Debug.Log("Connection Exception : " + exception);
        }
    }

    public void WaitForCreatureList()
    {
        for(int i = 0; i < int.MaxValue; ++i)
        {
            if (i % 100000 == 0)
                Debug.Log("waiting for server to send list....");
        }
        return;
    }

    #region network error UI control functions
    public void NetworkErrorPanelInactive()
    {
        networkErrorPanel.SetActive(false);
    }

    public void NetworkErrorPanelactive()
    {
        networkErrorPanel.SetActive(true);
    }
    #endregion

    public int GetStageLevel()
    {
        return stageLevel;
    }
}

