using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour
{
    [SerializeField]
    private GameObject gameDataControl;

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

    // Update is called once per frame

    private void Start()
    {
        PublicLevel.usingCreatureNum = 5;

        for (int i = 0; i < PublicLevel.friendlyTypeCreatureNum; i++)
        {
            int temp = i;
            upgradeButton[i].GetComponent<Button>().onClick.AddListener(delegate { TargetCreature(upgradeOption[temp]); });

        }

        for (int i = 0; i < PublicLevel.usingCreatureNum; i++)
        {
            int temp = i;
            locationButton[temp].onClick.AddListener(delegate { ChangeCreature(temp); });
        }
        upgradeShop.SetActive(false);
        gameDataControl = GameObject.Find("GameDataControl");

        //if no gameDataControl exists, instantiate one new gameDataControl
        if (gameDataControl == null)
        {
            gameDataControl = Instantiate(new GameObject(), transform.position, Quaternion.identity);
            gameDataControl.name = "GameDataControl";
            gameData = gameDataControl.AddComponent<GameData>();

            loadedData = gameData.LoadGameData();

            //load when game was levelselect scene was first loaded
            if (loadedData == null)
            {

                PublicLevel.SetPlayerLevel(1);
                PublicLevel.SetPlayerWin(0);
                for (int i = 0; i < PublicLevel.friendlyTypeCreatureNum; i++)
                {
                    PublicLevel.friendlyType[i] = new Vector2Int(i, 0);
                }
            }
            else
            {
                PublicLevel.SetPlayerLevel(loadedData.GetPlayerLevel());
                PublicLevel.SetPlayerWin(loadedData.GetPlayerWin());
                for (int i = 0; i < PublicLevel.friendlyTypeCreatureNum; i++)
                {
                    PublicLevel.friendlyType[i] = new Vector2Int(loadedData.GetFriendlyType()[i].x, loadedData.GetFriendlyType()[i].y);
                }
            }

            for (int i = 0; i < PublicLevel.friendlyTypeCreatureNum; i++)
            {
                PublicLevel.friendlyCreatureList[i] = PublicLevel.friendlyPrefab[PublicLevel.friendlyType[i].x, PublicLevel.friendlyType[i].y];

                PublicLevel.friendlyImageList[i] = PublicLevel.friendlyImage[PublicLevel.friendlyType[i].x, PublicLevel.friendlyType[i].y];
                Debug.Log(PublicLevel.friendlyImageList[i]);
            }
        }

        gameData.SaveGameData();
    }
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

    public void Escape()
    {
        Application.Quit();
    }
    public void LoadUpgrade()
    {
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
        Debug.Log("saved exitting upgradeshop");
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(scene.name == "LevelSelect")
        {
            gameDataControl = GameObject.Find("GameDataControl");
            if (gameDataControl != null)
            {
                gameData = gameDataControl.GetComponent<GameData>();
            }
        }
        if(scene.name == "DefaultIngame")
        {
        }
    }
}
