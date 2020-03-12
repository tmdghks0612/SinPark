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
    public AIplayer aiplayer;


    //variables defined overall in game
    //Number of maximum lane
    private int maxLanes = 3;
    //Maximum number of Units that can exist during the game
    private int maxUnits = 100;
    
    //Paramters for camera to move smoothly to intended direction
    private Vector3 cameraSpeed;
    private float smoothSpeed;
    private Vector3 targetPosition;
    private Vector3 smoothPosition;
    protected static GameObject mainCamera;

    protected int[] creatureType;
    protected int[] upgradeType;

    private GameObject[] lanes = new GameObject[PublicLevel.usingLaneNum];
    private GameObject[] SummonButton = new GameObject[PublicLevel.usingCreatureNum];
    protected int selectedCreatureType;

    private bool buttonFlag = true;
    private float coolDownTime = 0.15f;
    protected bool spawnCooldownFlag = true;  //true when able to spawn. False when not able to spawn

    private Text[] costText;
    private bool gameOverFlag = false;

    //UI Panel hidden. One of the panel become active when game ends
    public GameObject losePanel;
    public GameObject winPanel;
    public GameObject surrenderPanel;

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
        creatureType = new int[PublicLevel.friendlyTypeCreatureNum];
        //initializing creature and its upgrade type later changes will remove this 2 lines
        InitCreatureType();
        

        //Make Buttons to call ChooseCreature.
        for (int i = 0; i < SummonButton.Length; i++)
        {
            SummonButton[i] = GameObject.Find("SummonButton" + (i + 1));
            int temp = creatureType[i+1];
            SummonButton[i].GetComponent<Button>().onClick.AddListener(delegate { ChooseCreature(temp); });
        }
        selectedCreatureType = 0;
        
        InitButtonImage();

        //Make Buttons to call SummonProcedure. Make it disabled until activated
        for (int i = 0; i < lanes.Length; i++)
        {
            lanes[i] = GameObject.Find("LaneButton" + (i + 1));
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
        minSwipeDist = Mathf.Max(ScreenSize.x, ScreenSize.y) / 15f;
    }
    
    protected virtual void SummonProcedure(int laneNumber)
    {
        if (spawnCooldownFlag)
        {
            spawnCooldownFlag = false;
            StartCoroutine("CoolDownCount");
            spawnControl.SpawnCreatureLane(laneNumber, GameControl.Sides.Friendly, selectedCreatureType);
        }
    }

    IEnumerator CoolDownCount()
    {
        yield return new WaitForSeconds(coolDownTime);
        spawnCooldownFlag = true;
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
        // if player destroyed hostile base
        if (isWin && gameOverFlag == false)
        {
            // if incoming boss remains
            if (PublicLevel.GetIsBoss())
            {
                //spawn boss
                spawnControl.SummonBoss();
                PublicLevel.SetIsBoss(false);
            }
            else
            {
                PublicLevel.SetPlayerLevel(PublicLevel.GetStageLevel() + 1);
                spawnControl.DeadAllHostileCreature();
                aiplayer.AIplayerStop();
                winPanel.SetActive(true);
                winPanel.transform.GetChild(0).GetComponent<Text>().text = "Your level is " + PublicLevel.GetPlayerLevel().ToString();
            }
            
            for(int i = 0; i < PublicLevel.usingLaneNum; ++i)
            {
                GameObject[] laneObjects = GameObject.FindGameObjectsWithTag("Lane" + i.ToString());
                foreach(GameObject laneObject in laneObjects)
                {
                    if(laneObject.name == "creature0_0Prefab(Clone)" && laneObject.GetComponent<DefaultCreature>().GetSide() == GameControl.Sides.Hostile)
                    {
                        laneObject.GetComponent<DefaultCreature>().Dead();
                    }
                }
            }
            return;
        }
        // if player's base was destroyed
        else if (!isWin && gameOverFlag)
        {
            losePanel.SetActive(true);
        }
        gameOverFlag = true;
        aiplayer.AIplayerStop();
    }

    public void ActivateSurrenderPanel()
    {
        surrenderPanel.SetActive(true);
    }

    public void DeactivateSurrenderPanel()
    {
        surrenderPanel.SetActive(false);
    }

    public virtual void LoadGameScene()
    {
        SceneManager.LoadScene("StageSelect");
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
                    if(targetPosition.x <= 15)
                        targetPosition = mainCamera.transform.position + cameraSpeed * Mathf.Sqrt((touchDownPos.x - currentPos.x) / 50) ; //100 is constant number to adjust camera move speed
                }
                else if(currentPos.x - touchDownPos.x > minSwipeDist)
                {
                    if (targetPosition.x >= -15)
                        targetPosition = mainCamera.transform.position - cameraSpeed * Mathf.Sqrt((currentPos.x - touchDownPos.x) / 50); //100 is constant number to adjust camera move speed
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
        for(int i=0; i< PublicLevel.friendlyTypeCreatureNum; ++i)
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
            if(PublicLevel.friendlyType[i+1].x == 0)
            {
                Color alphaChecker = SummonButton[i].GetComponent<Image>().color;
                alphaChecker.a = 0;
                SummonButton[i].GetComponent<Image>().color = alphaChecker;
                SummonButton[i].GetComponent<Button>().interactable = false;
            }
            else
            {
                SummonButton[i].GetComponent<Image>().sprite = PublicLevel.friendlyImageList[i + 1];
                costText[i] = SummonButton[i].transform.GetChild(0).gameObject.GetComponent<Text>();
                costText[i].text = PublicLevel.friendlyCreatureList[i + 1].GetComponent<DefaultCreature>().GetManaCost().ToString();
            }
            
        }
    }

    #region Get functions

    public static Transform GetCameraTransform()
    {
        return mainCamera.transform;
    }

    #endregion
}
