using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageButtonMultiplayer : StageButton
{
    // control instances
    [SerializeField]
    private ServerControl serverControl;

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
        creatureSentFlag = false;
        creatureReceivedFlag = false;
        StartCoroutine(serverControl.OpenStream(6.0f));
        PublicLevel.SetLevel(hostileType, manaAmount, manaRegenTime, creatureSpawnTime, stageLevel, false, null);
        //StartCoroutine(WaitForServer(6.0f));
    }

    /*public IEnumerator WaitForServer(float _waitTime)
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
        
        Debug.Log("receive check started");
        for (int i = 0; i < maxAttempt; ++i)
        {
            
            if (creatureReceivedFlag == true)
            {
                Debug.Log("receive flag detected");

                NetworkWaitPanelInactive();
                // load after creaturelist receive is complete
                LoadingSceneManager.LoadScene("DefaultIngameMultiplayer");
                Debug.Log("loaded?");
                break;
            }
        }
        NetworkErrorPanelactive();
        Debug.Log("receive check ended");

    }*/

    /*public IEnumerator WaitForServerReceive(float _waitTime)
    {
        Debug.Log("wait for receive");
        yield return new WaitForSeconds(_waitTime);
        Debug.Log("receive check started");
        for (int i = 0; i < maxAttempt; ++i)
        {
            
            if (creatureReceivedFlag == true)
            {
                Debug.Log("receive flag detected");

                NetworkWaitPanelInactive();
                // load after creaturelist receive is complete
                LoadingSceneManager.LoadScene("DefaultIngameMultiplayer");
                break;
            }
        }
        NetworkErrorPanelactive();
        Debug.Log("receive check ended");

    }*/

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
