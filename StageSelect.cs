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
    [SerializeField]
    private GameObject unlockPopup;

    //show number of corns player have
    [SerializeField]
    private Text cornText;

    //Buttons that are child of upgradeShop gameObject.
    private Button[] upgradeButton = new Button[PublicLevel.friendlyTypeCreatureNum * PublicLevel.friendlyTypeUpgradeNum];
    private Button[] locationButton = new Button[PublicLevel.usingCreatureNum];
    private GameObject[] stageButton;

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
            upgradeButton[i] = GameObject.Find("UpgradeButton" + (i + 1)).GetComponent<Button>();
            int temp = i; //used temp since just using i makes every buttons to send last i value as a parameter
            upgradeButton[i].GetComponent<Button>().onClick.AddListener(delegate { TargetCreature(upgradeOption[temp]); });
        }

        //make buttons toi call ChangeCreature fucntion
        for (int i = 0; i < PublicLevel.usingCreatureNum; i++)
        {
            locationButton[i] = GameObject.Find("Location" + (i + 1)).GetComponent<Button>();
            int temp = i;
            locationButton[temp].onClick.AddListener(delegate { ChangeCreature(temp); });
        }

        unlockPopup.SetActive(false);
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
                PublicLevel.SetCorn(0);
                for(int i=1; i<6; i++)
                {
                    PublicLevel.unlockType[i, 0] = true;
                }
                for (int i = 0; i < PublicLevel.friendlyTypeCreatureNum; i++)
                {
                    PublicLevel.friendlyType[i] = new Vector2Int(i, 0);
                }
            }
            else
            {
                PublicLevel.SetPlayerLevel(loadedData.GetPlayerLevel());
                PublicLevel.SetPlayerWin(loadedData.GetPlayerWin());
                PublicLevel.SetCorn(loadedData.GetCorn());

                for (int i = 0; i < PublicLevel.usingCreatureNum; i++)
                {
                    PublicLevel.friendlyType[i] = new Vector2Int(loadedData.GetFriendlyType()[i].x, loadedData.GetFriendlyType()[i].y);
                }

                for(int i=0; i< PublicLevel.friendlyTypeCreatureNum; i++)
                {
                    for(int k=0; k<PublicLevel.friendlyTypeUpgradeNum; k++)
                    {
                        PublicLevel.unlockType[i, k] = loadedData.GetUnlockType()[i,k];
                    }
                }
            }

            //fill frienldyCreatureList based on loaded/default friendlyType
            for (int i = 0; i < PublicLevel.friendlyTypeCreatureNum; i++)
            {
                PublicLevel.friendlyCreatureList[i] = PublicLevel.friendlyPrefab[PublicLevel.friendlyType[i].x, PublicLevel.friendlyType[i].y];

                PublicLevel.friendlyImageList[i] = PublicLevel.friendlyImage[PublicLevel.friendlyType[i].x, PublicLevel.friendlyType[i].y];
            }
        }

        cornText.text = PublicLevel.GetCorn().ToString();

        stageButton = GameObject.FindGameObjectsWithTag("StageButton");
        foreach (GameObject stageBtn in stageButton)
        {
            StageButton stageCheck = stageBtn.GetComponent<StageButton>();
            if (stageCheck.GetStageLevel() > PublicLevel.GetPlayerLevel())
            {
                stageBtn.GetComponent<Image>().color = Color.red;
                stageBtn.GetComponent<Button>().interactable = false;
            }
            else if(stageCheck. GetStageLevel() == PublicLevel.GetPlayerLevel())
            {
                stageBtn.GetComponent<Image>().color = Color.blue;
            }
        }

        //save data
        gameData.SaveGameData();
    }

    //When button on upgrade shop is clicked, save the type of the creatures stored in the button
    //make location button interactable(clickable) for player to put decided creature to the position they want
    void TargetCreature(Vector2Int type)
    {
        changingInfo = type;
        if (PublicLevel.unlockType[type.x, type.y] == true)
        {
            for (int i = 0; i < PublicLevel.usingCreatureNum; i++)
            {
                locationButton[i].interactable = true;
            }
        }
        else
        {
            for (int i = 0; i < PublicLevel.usingCreatureNum; i++)
            {
                locationButton[i].interactable = false;
            }
            unlockPopup.SetActive(true);
            int price = PublicLevel.friendlyPrefab[type.x, type.y].GetComponent<DefaultCreature>().GetUnlockCost();
            if (PublicLevel.GetCorn() < price)
            {
                unlockPopup.transform.GetChild(1).GetComponent<Button>().interactable = false;
                unlockPopup.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = "Unlock\n (Not enough corns)";
            }
            else
            {
                unlockPopup.transform.GetChild(1).GetComponent<Button>().interactable = true;
                unlockPopup.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = "Unlock\n (" + price + " corn )";
            }
        }
    }

    //Change friendlyCreatureList and image of the button to the saved creature/upgrade type at TargetCreature function. 
    void ChangeCreature(int location)
    {
        PublicLevel.UpdateFriendlyList(location, changingInfo);
        
    
        upgradeShop.transform.GetChild(2 + location).GetChild(0).GetComponent<Image>().sprite = PublicLevel.friendlyImageList[location];

        //player use 10 corns per changing upgrade mode will be fixed soon.
        cornText.text = PublicLevel.GetCorn().ToString();

        //make location button no longer interactable
        for (int i = 0; i < PublicLevel.usingCreatureNum; i++)
        {
            locationButton[i].interactable = false;
        }

    }
    
    //exit the game
    public void Escape()
    {
        Application.Quit();
    }

    //called when upgradeShop open button is clicked. activate upgradeShop UI and deactivate stageMap UI. Image setup for button
    public void LoadUpgrade()
    {
        upgradeShop.SetActive(true);
        cornText.text = PublicLevel.GetCorn().ToString();
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
        cornText.text = PublicLevel.GetCorn().ToString();
        gameData.SaveGameData();
        stageMap.SetActive(true);
        upgradeShop.SetActive(false);
    }

    //make OnSceneLoaded called when scene is loaded
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void UnlockCancel()
    {
        unlockPopup.SetActive(false);
    }

    public void UnlockCreature()
    {
        int cost = PublicLevel.friendlyPrefab[changingInfo.x, changingInfo.y].GetComponent<DefaultCreature>().GetUnlockCost();
        PublicLevel.SetCorn(PublicLevel.GetCorn() - cost);
        PublicLevel.unlockType[changingInfo.x, changingInfo.y] = true;
        unlockPopup.SetActive(false);
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
            Debug.Log("changing music to stageselect!");
            GameObject.Find("BGMControl").GetComponent<BGMControl>().ChangeBGMToMainMenu();
        }
        if(scene.name == "DefaultIngame")
        {
            Debug.Log("changing music to Ingame!");
            GameObject.Find("BGMControl").GetComponent<BGMControl>().ChangeBGMToIngame();
        }
    }

}
