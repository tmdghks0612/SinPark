using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class StageSelect : MonoBehaviour
{
    //GameDataControl is undestructable gameObject, save and load data
    [SerializeField]
    private GameObject gameDataControl;
    private GameData gameData;
    private GameDataForm loadedData;

    //GameObect in Canvas. Upgradeshop and stageMap objects become active and inactive like a switch, change UI shown to the player 
    [SerializeField]
    private GameObject upgradeShop;
    [SerializeField]
    private GameObject stageMap;

    //Buttons that are child of upgradeShop gameObject. They are put mannually in editor by SerializeField
    [SerializeField]
    private Button[] upgradeButton = new Button[PublicLevel.friendlyTypeCreatureNum * PublicLevel.friendlyTypeUpgradeNum];
    [SerializeField]
    private Button[] locationButton = new Button[5];

    //connects the type of creatures to select and button. datas put mannually in editor
    [SerializeField]
    private Vector2Int[] upgradeOption = new Vector2Int[PublicLevel.friendlyTypeCreatureNum * PublicLevel.friendlyTypeUpgradeNum];
    
    //Temporary GameObject used to change images of button at upgradeShop
    private GameObject listObject;

    //Save the type of creature to put on the button until player decides which number of button to change.
    private Vector2Int changingInfo;

    private void Start()
    {
        //make buttons to call TargetCreature function which sends upgradeoption[temp] as a parameter.
        for (int i = 0; i < PublicLevel.friendlyTypeCreatureNum; i++)
        {
            int temp = i; //used temp since just using i makes every buttons to send last i value as a parameter
            upgradeButton[i].GetComponent<Button>().onClick.AddListener(delegate { TargetCreature(upgradeOption[temp]); });

        }

        //make buttons toi call ChangeCreature fucntion
        for (int i = 0; i < PublicLevel.usingCreatureNum; i++)
        {
            int temp = i;
            locationButton[temp].onClick.AddListener(delegate { ChangeCreature(temp); });
        }

        //UI upgradShop is not shown until upgradeShop is active
        upgradeShop.SetActive(false);


        //Try to find GameDataControl
        gameDataControl = GameObject.Find("GameDataControl");

        //if no gameDataControl exists, instantiate one new gameDataControl
        if (gameDataControl == null)
        {
            gameDataControl = Instantiate(new GameObject(), transform.position, Quaternion.identity);
            gameDataControl.name = "GameDataControl";
            gameData = gameDataControl.AddComponent<GameData>();

            loadedData = gameData.LoadGameData();

            //load when StageSelect scene was first loaded. If there's no loadedData, make a default setting for player - level, win, creature type using.
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
                for (int i = 0; i < PublicLevel.usingCreatureNum; i++)
                {
                    PublicLevel.friendlyType[i] = new Vector2Int(loadedData.GetFriendlyType()[i].x, loadedData.GetFriendlyType()[i].y);
                }
            }

            //fill frienldyCreatureList based on loaded/default friendlyType
            for (int i = 0; i < PublicLevel.friendlyTypeCreatureNum; i++)
            {
                PublicLevel.friendlyCreatureList[i] = PublicLevel.friendlyPrefab[PublicLevel.friendlyType[i].x, PublicLevel.friendlyType[i].y];

                PublicLevel.friendlyImageList[i] = PublicLevel.friendlyImage[PublicLevel.friendlyType[i].x, PublicLevel.friendlyType[i].y];
                Debug.Log(PublicLevel.friendlyImageList[i]);
            }
        }

        //save data
        gameData.SaveGameData();
    }

    //When button on upgrade shop is clicked, save the type of the creatures stored in the button
    //make location button interactable(clickable) for player to put decided creature to the position they want
    void TargetCreature(Vector2Int type)
    {
        Debug.Log("in");
        changingInfo = type;

        for (int i = 0; i < PublicLevel.usingCreatureNum; i++)
        {
            locationButton[i].interactable = true;
        }
    }

    //Change friendlyCreatureList and image of the button to the saved creature/upgrade type at TargetCreature function. 
    void ChangeCreature(int location)
    {
        PublicLevel.friendlyCreatureList[location] = PublicLevel.friendlyPrefab[changingInfo.x, changingInfo.y];
        PublicLevel.friendlyImageList[location] = PublicLevel.friendlyImage[changingInfo.x, changingInfo.y];
        PublicLevel.friendlyType[location] = changingInfo;
    
        upgradeShop.transform.GetChild(2 + location).GetChild(0).GetComponent<Image>().sprite = PublicLevel.friendlyImageList[location];

        //make location button no longer interactable
        for (int i = 0; i < PublicLevel.usingCreatureNum; i++)
        {
            locationButton[i].interactable = false;
        }

    }
    
    //eixt the game
    public void Escape()
    {
        Application.Quit();
    }

    //called when upgradeShop open button is clicked. activate upgradeShop UI and deactivate stageMap UI. Image setup for button
    public void LoadUpgrade()
    {
        upgradeShop.SetActive(true);

        //Changes images of button to the UpgradeOption buttons based on friendlyImage[,]
        for (int i=0; i<PublicLevel.friendlyTypeCreatureNum;i++)
        {
            for (int k = 0; k < PublicLevel.friendlyTypeUpgradeNum; k++)
            {
                listObject = upgradeShop.transform.GetChild(7 + i).gameObject;
                listObject.GetComponent<Image>().sprite = PublicLevel.friendlyImage[i, k];
            }
        }

        //Changes images of location button based on friendlyImageList[]
        for (int i = 0; i < PublicLevel.usingCreatureNum; i++)
        {
            listObject = upgradeShop.transform.GetChild(2 + i).GetChild(0).gameObject;
            listObject.GetComponent<Image>().sprite = PublicLevel.friendlyImageList[i];
            locationButton[i].interactable = false;
        }
        
        stageMap.SetActive(false);
    }

    //opposite of LoadUpgrade. Activate stageMap and Deactivate upgradeShop. Save data that have been changed during upgradeShop
    public void LoadMap()
    {
        gameData.SaveGameData();
        stageMap.SetActive(true);
        upgradeShop.SetActive(false);
    }

    //make OnSceneLoaded called when scene is loaded
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //When StageSelect Scene is loaded, find GameDataControl and gameData
        if(scene.name == "StageSelect")
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