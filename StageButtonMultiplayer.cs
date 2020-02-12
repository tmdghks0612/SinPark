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
    private static bool creatureSentFlag = false;
    private static bool creatureReceivedFlag = false;
    // flag to check if server is ready
    private static bool socketFlag = false;

    private int maxAttempt = 50000000;

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
        InitLevel();
        //StartCoroutine("InitLevel");
        //Invoke("InitLevel", 0.5f);
    }

    //Save parameters stored in the button to the PublicLevel and load scene
    public override void InitLevel()
    {
        Debug.Log("start");
        StartCoroutine(ServerControl.OpenStream(3.0f));
        Debug.Log("level set start");
        PublicLevel.SetLevel(hostileType, manaAmount, manaRegenTime, creatureSpawnTime, stageLevel, false, null);
        Debug.Log("level set complete");
        StartCoroutine(WaitForServer(3.0f));
    }

    public IEnumerator WaitForServer(float _waitTime)
    {
        yield return new WaitForSeconds(_waitTime);
        for(int i = 0; i < maxAttempt; ++i)
        {
            if (creatureSentFlag)
            {
                StartCoroutine(ServerControl.ListenForHostileCreatureList(1.0f));
                break;
            }
        }
        for (int i = 0; i < maxAttempt; ++i)
        {
            
            if (creatureReceivedFlag == true)
            {
                creatureReceivedFlag = false;
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
    public static void SetCreatureSentFlag(bool _creatureSentFlag)
    {
        creatureSentFlag = _creatureSentFlag;
    }

    public static void SetCreatureReceivedFlag(bool _creatureReceivedFlag)
    {
        creatureReceivedFlag = _creatureReceivedFlag;
    }

    public static void SetSocketFlag(bool _socketFlag)
    {
        socketFlag = _socketFlag;
    }
    #endregion
}
