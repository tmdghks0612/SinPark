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
    [SerializeField]
    private GameObject networkWaitPanel;

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

    public void NetworkWaitStart()
    {
        NetworkWaitPanelactive();
        Invoke("InitLevelMultiplayer", 0.5f);
    }

    //Save parameters stored in the button to the PublicLevel and load scene
    public void InitLevelMultiplayer()
    {
        Debug.Log("activating");
        // activate waiting panel
        networkWaitPanel.SetActive(true);
        Debug.Log("activated");
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
        
        for(int i = 0; i < 50000000; ++i)
        {
            if (i % 1000000 == 0)
                Debug.Log("waiting for server to send list....");
        }
        NetworkWaitPanelInactive();
        return;
    }

    #region network error UI control functions

    private void NetworkWaitPanelInactive()
    {
        networkWaitPanel.SetActive(false);
    }

    private void NetworkWaitPanelactive()
    {
        networkWaitPanel.SetActive(true);
    }

    private void NetworkErrorPanelInactive()
    {
        networkErrorPanel.SetActive(false);
    }

    private void NetworkErrorPanelactive()
    {
        NetworkWaitPanelInactive();
        networkErrorPanel.SetActive(true);
    }
    #endregion

    public int GetStageLevel()
    {
        return stageLevel;
    }
}

