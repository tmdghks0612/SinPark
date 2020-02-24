using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    [SerializeField]
    private bool creatureSentFlag = false;
    [SerializeField]
    private bool creatureReceivedFlag = false;
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
        InitLevel();
        StartCoroutine(WaitForSceneLoad());
    }
    
    private IEnumerator WaitForSceneLoad()
    {
        yield return new WaitForSeconds(10.0f);
        if (creatureReceivedFlag)
        {
            NetworkWaitPanelInactive();
            LoadingSceneManager.LoadScene("DefaultInGameMultiplayer");
        }
        else
        {
            NetworkErrorPanelactive();
            serverControl.CloseSocket();
        }
        
    }

    //Save parameters stored in the button to the PublicLevel and load scene
    public override void InitLevel()
    {
        creatureSentFlag = false;
        creatureReceivedFlag = false;
        StartCoroutine(serverControl.OpenStream(6.0f));
        PublicLevel.SetLevel(hostileType, manaAmount, manaRegenTime, creatureSpawnTime, stageLevel, false, null);
        
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
    public void SetCreatureSentFlag(bool _creatureSentFlag)
    {
        creatureSentFlag = _creatureSentFlag;
    }

    public void SetCreatureReceivedFlag(bool _creatureReceivedFlag)
    {
        creatureReceivedFlag = _creatureReceivedFlag;
    }

    public static void SetSocketFlag(bool _socketFlag)
    {
        socketFlag = _socketFlag;
    }
    #endregion
}
