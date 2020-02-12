using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageButtonMultiplayer : StageButton
{
    // UI game objects
    [SerializeField]
    private static GameObject networkErrorPanel;
    [SerializeField]
    private static GameObject networkWaitPanel;

    // flag to check if server is ready
    private static bool creatureFlag = false;
    // flag to check if server is ready
    private static bool socketFlag = false;

    private void Start()
    {
        networkErrorPanel = GameObject.Find("Canvas/NetworkError");
        networkWaitPanel = GameObject.Find("Canvas/NetworkWait");
        NetworkErrorPanelInactive();
        NetworkWaitPanelInactive();
    }

    public void NetworkWaitStart()
    {
        NetworkWaitPanelactive();
        //InitLevel();
        StartCoroutine("InitLevel");
        //Invoke("InitLevel", 0.5f);
    }

    //Save parameters stored in the button to the PublicLevel and load scene
    public override void InitLevel()
    {
        StartCoroutine(ServerControl.OpenStream(3.0f));
        PublicLevel.SetLevel(hostileType, manaAmount, manaRegenTime, creatureSpawnTime, stageLevel, false, null);
        
        WaitForServer(3.0f);
    }

    public IEnumerator WaitForServer(float _waitTime)
    {
        yield return new WaitForSeconds(_waitTime);
        while(true)
        {
            if (socketFlag == true)
            {
                socketFlag = false;
                if (ServerControl.SendCreatureList())
                {
                    StartCoroutine(ServerControl.ListenForHostileCreatureList(3.0f));
                }
                else
                {
                    NetworkErrorPanelactive();
                }
                
                break;
            }
        }
        while(true)
        {
            
            if (creatureFlag == true)
            {
                creatureFlag = false;
                break;
            }
        }
        
    }

    #region network error UI control functions

    public static void NetworkWaitPanelInactive()
    {
        networkWaitPanel.SetActive(false);
    }

    public static void NetworkWaitPanelactive()
    {
        networkWaitPanel.SetActive(true);
    }

    public static void NetworkErrorPanelInactive()
    {
        networkErrorPanel.SetActive(false);
    }

    public static void NetworkErrorPanelactive()
    {
        NetworkWaitPanelInactive();
        networkErrorPanel.SetActive(true);
    }
    #endregion

    #region Set functions
    public static void SetCreatureFlag(bool _creatureFlag)
    {
        creatureFlag = _creatureFlag;
    }

    public static void SetSocketFlag(bool _socketFlag)
    {
        socketFlag = _socketFlag;
    }
    #endregion
}
