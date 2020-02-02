using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour
{
    [SerializeField]
    private GameObject upgradeShop;
    [SerializeField]
    private GameObject stageMap;
    [SerializeField]
    private Button[] upgradeButton = new Button[PublicLevel.friendlyTypeCreatureNum * PublicLevel.friendlyTypeUpgradeNum];
    [SerializeField]
    private Button[] locationButton = new Button[5];
    [SerializeField]
    private Vector2Int[] upgradeOption = new Vector2Int[PublicLevel.friendlyTypeCreatureNum * PublicLevel.friendlyTypeUpgradeNum];
    private GameObject listObject;
    private Vector2Int changingInfo;
    private Sprite Image;
    private GameData gameData;
    private GameDataForm loadedData;

    void Start()
    {
        PublicLevel.usingCreatureNum = 5;

        Debug.Log("Creature Num " + PublicLevel.friendlyTypeCreatureNum);

        for(int i = 0; i<PublicLevel.friendlyTypeCreatureNum; i++)
        {
            int temp = i;
            upgradeButton[i].GetComponent<Button>().onClick.AddListener(delegate { TargetCreature(upgradeOption[temp]); });

        }

        for(int i = 0; i<PublicLevel.usingCreatureNum; i++)
        {
            int temp = i;
            locationButton[temp].onClick.AddListener(delegate { ChangeCreature(temp); });
        }
        upgradeShop.SetActive(false);
        gameData = GameObject.Find("GameDataControl").GetComponent<GameData>();

        PublicLevel.InitReady();

        loadedData = gameData.LoadGameData();
                
        if(loadedData == null)
        {
            PublicLevel.playerLevel = 1;
            PublicLevel.playerWin = 0;
            for(int i=0; i<PublicLevel.friendlyTypeCreatureNum; i++)
            {
                PublicLevel.friendlyType[i] = new Vector2Int(i, 0);
            }
        }
        else
        {
            PublicLevel.playerLevel = loadedData.GetPlayerLevel();
            PublicLevel.playerWin = loadedData.GetPlayerWin();
            for(int i=0; i<PublicLevel.friendlyTypeCreatureNum;i++)
            {
                PublicLevel.friendlyType[i] = new Vector2Int(loadedData.GetFriendlyType()[i].x, loadedData.GetFriendlyType()[i].y);
            }
        }

        PublicLevel.InitSetting();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void ChangeCreature(int location)
    {
        Debug.Log(location);
        PublicLevel.friendlyCreatureList[location] = PublicLevel.friendlyPrefab[changingInfo.x, changingInfo.y];
        PublicLevel.friendlyImageList[location] = PublicLevel.friendlyImage[changingInfo.x, changingInfo.y];
        PublicLevel.friendlyType[location] = changingInfo;
    
        upgradeShop.transform.GetChild(2 + location).GetChild(0).GetComponent<Image>().sprite = PublicLevel.friendlyImageList[location];
        for (int i = 0; i < 5; i++)
        {
            locationButton[i].interactable = false;
        }

    }
    void TargetCreature(Vector2Int type)
    {
        Debug.Log("in");
        changingInfo = type;
        
        for(int i=0; i<PublicLevel.usingCreatureNum; i++)
        {
            locationButton[i].interactable = true;
        }
        
    }
    public void LoadUpgrade()
    {
        Debug.Log("Level " + PublicLevel.playerLevel);
        Debug.Log("Win " + PublicLevel.playerWin);
        upgradeShop.SetActive(true);

        for(int i=0; i<PublicLevel.friendlyTypeCreatureNum;i++)
        {
            for (int k = 0; k < PublicLevel.friendlyTypeUpgradeNum; k++)
            {
                listObject = upgradeShop.transform.GetChild(7 + i).gameObject;
                listObject.GetComponent<Image>().sprite = PublicLevel.friendlyImage[i, k];
            }
        }
        for (int i = 0; i < PublicLevel.usingCreatureNum; i++)
        {
            listObject = upgradeShop.transform.GetChild(2 + i).GetChild(0).gameObject;
            listObject.GetComponent<Image>().sprite = PublicLevel.friendlyImageList[i];
            locationButton[i].interactable = false;
        }
        
        stageMap.SetActive(false);
    }
    public void LoadMap()
    {
        gameData.SaveGameData();
        stageMap.SetActive(true);
        upgradeShop.SetActive(false);
    }
}
