using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameControl : MonoBehaviour
{
    //Basic Controller during game. put manually by Unity Editor
    public CombatControl combatControl;
    public SpawnControl spawnControl;
    public ServerControl serverControl;
    public AIplayer aiplayer;


    //variables defined overall in game
    //Number of maximum lane
    private int maxLanes = 3;
    //Maximum number of Units that can exist during the game
    private int maxUnits = 100;
    
    
    public int typeCreature = PublicLevel.friendlyTypeCreatureNum;
    public int typeUpgrade = PublicLevel.friendlyTypeUpgradeNum;
    
    //Paramters for camera to move smoothly to intended direction
    private Vector3 cameraSpeed;
    private float smoothSpeed;
    private GameObject mainCamera;
    private Vector3 targetPosition;
    private Vector3 smoothPosition;

    protected int[] creatureType;
    protected int[] upgradeType;

    public GameObject[] lanes;
    protected int selectedCreatureType;
    public GameObject[] SummonButton;
    private bool buttonFlag = true;
    
    private Text[] costText;
    private bool gameOverFlag = false;

    //UI Panel hidden. One of the panel become active when game ends
    public GameObject losePanel;
    public GameObject winPanel;

    //Parameters For Android Touch/Swipe Function
    Vector2 ScreenSize;
    float minSwipeDist;
    Vector2 swipeDirection;
    Vector2 touchDownPos;
    Vector2 currentPos;

    //Side of the Creatures summoned in the game
    public enum Sides { Friendly, Hostile };

    // Start is called before the first frame update
    protected virtual void Start()
    {
        cameraSpeed = new Vector3(2.0f, 0, 0);
        mainCamera = GameObject.FindWithTag("MainCamera");
        targetPosition = mainCamera.transform.position;
        creatureType = new int[typeCreature];
        //initializing creature and its upgrade type later changes will remove this 2 lines
        InitCreatureType();
        InitButtonImage();

        //Make Buttons to call ChooseCreature.
        for (int i = 0; i < SummonButton.Length; i++)
        {
            int temp = creatureType[i];
            SummonButton[i].GetComponent<Button>().onClick.AddListener(delegate { ChooseCreature(temp); });
        }
        selectedCreatureType = 0;

        //Make Buttons to call SummonProcedure. Make it disabled until activated
        for (int i = 0; i < lanes.Length; i++)
        {
            int temp = creatureType[i];
            lanes[i].GetComponent<Button>().onClick.AddListener(delegate { SummonProcedure(temp); });
            lanes[i].SetActive(false);
        }

        spawnControl.SpawnControlStart();

        if(aiplayer != null)
        {
            aiplayer.AIplayerStart();
        }

        //Values for android touch. Get Screen Size and set minimum swipe distant as 1/14 of it.
        ScreenSize = new Vector2(Screen.width, Screen.height);
        minSwipeDist = Mathf.Max(ScreenSize.x, ScreenSize.y) / 14f;
    }
    

    protected virtual void SummonProcedure(int laneNumber)
    {
        spawnControl.SpawnCreatureLane(laneNumber, GameControl.Sides.Friendly, selectedCreatureType);
    }

    //Choose the creature to summon to the lane. Decided data is saved at selectedCreatureType
    void ChooseCreature(int type)
    {
        if (type != selectedCreatureType)
        {
            foreach (GameObject buttons in lanes)
            {
                buttons.SetActive(true);
            }
            selectedCreatureType = type;
        }
        else if (buttonFlag)
        {
            foreach (GameObject buttons in lanes)
            {
                buttons.SetActive(true);
            }
            buttonFlag = false;
        }
        else
        {
            foreach (GameObject buttons in lanes)
            {
                buttons.SetActive(false);
            }
            buttonFlag = true;
        }
    }

    //called from PlayerBase when one of the player died
    public virtual void GameOver(bool isWin)
    {
        if (isWin && gameOverFlag == false)
        {
            PublicLevel.SetPlayerLevel(PublicLevel.GetStageLevel());
            winPanel.SetActive(true);
            winPanel.transform.GetChild(0).GetComponent<Text>().text = "Your level is " + PublicLevel.GetPlayerLevel().ToString();
        }
        else if (!isWin && gameOverFlag)
        {
            losePanel.SetActive(true);
        }
        gameOverFlag = true;
        aiplayer.AIplayerStop();
    }
    public void LoadGameScene()
    {
        SceneManager.LoadScene("StageSelect");
        //SceneManager.LoadScene("StageSelect");
    }

    //Input Depends on the mod
    private void LateUpdate()
    {
        //UNITY_Anroid
#if UNITY_ANDROID
        //save the position where first touched, calculate the disance between first and current. If it's distance is larger than setted value, make camera move to the position
        if(Input.touches.Length>0)
        {
            Touch t = Input.GetTouch(0);
            if(t.phase == TouchPhase.Began)
            {
                touchDownPos = new Vector2(t.position.x, t.position.y);
                currentPos = new Vector2(t.position.x, t.position.y);
            }
            else if(t.phase == TouchPhase.Moved || t.phase == TouchPhase.Stationary)
            {
                currentPos = new Vector2(t.position.x, t.position.y);
                if(touchDownPos.x - currentPos.x > minSwipeDist)
                {
                    targetPosition = mainCamera.transform.position - cameraSpeed;
                }
                else if(currentPos.x - touchDownPos.x > minSwipeDist)
                {
                    targetPosition = mainCamera.transform.position + cameraSpeed;
                }
            }
            else if (t.phase == TouchPhase.Ended)
            {

            }
        }

#else
        if (Input.GetKey("right"))
        {
            targetPosition = mainCamera.transform.position + cameraSpeed;
        }
        else if (Input.GetKey("left"))
        {
            targetPosition = mainCamera.transform.position - cameraSpeed;
        }
#endif

        smoothPosition = Vector3.Lerp(mainCamera.transform.position, targetPosition, 0.05f);
        mainCamera.transform.position = smoothPosition;
    }

    //reutrn maxLanes
    public int GetMaxLanes()
    {
        return maxLanes;
    }

    //return maxUnits
    public int GetMaxUnits()
    {
        return maxUnits;
    }

    private void InitCreatureType()
    {
        for(int i=0; i< typeCreature; ++i)
        {
            creatureType[i] = i;
        }
    }

    //Change image and cost of summon creature to the right value
    private void InitButtonImage()
    {
        costText = new Text[PublicLevel.usingCreatureNum];
        for (int i = 0; i < PublicLevel.usingCreatureNum; i++)
        {
            SummonButton[i].GetComponent<Image>().sprite = PublicLevel.friendlyImageList[i];
            costText[i] = SummonButton[i].transform.GetChild(0).gameObject.GetComponent<Text>();
            costText[i].text = PublicLevel.friendlyCreatureList[i].GetComponent<DefaultCreature>().GetManaCost().ToString();
        }
    }
}
