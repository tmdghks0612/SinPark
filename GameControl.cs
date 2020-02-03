using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameControl : MonoBehaviour
{
    public CombatControl combatControl;
    public SpawnControl spawnControl;
    public ServerControl serverControl;

    public AIplayer aiplayer;

    //variables defined overall in game
    private int maxLanes = 3;
    private int maxUnits = 100;
    public int typeCreature = 7;
    public int typeUpgrade = 1;
    private Vector3 cameraSpeed;
    private float smoothSpeed;
    private GameObject mainCamera;
    private Vector3 targetPosition;
    private Vector3 smoothPosition;

    protected int[] creatureType;
    protected int[] upgradeType;

    public GameObject[] lanes;
    protected int monsterType;
    public GameObject[] SummonButton;
    private bool buttonFlag = true;
    private Text[] costText;

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
        for (int i = 0; i < SummonButton.Length; i++)
        {
            int temp = creatureType[i];
            SummonButton[i].GetComponent<Button>().onClick.AddListener(delegate { ChooseCreature(temp); });
        }
        monsterType = 0;
        for (int i = 0; i < lanes.Length; i++)
        {
            int temp = creatureType[i];
            lanes[i].GetComponent<Button>().onClick.AddListener(delegate { SummonProcedure(temp); });
            lanes[i].SetActive(false);
        }

        spawnControl.SpawnControlStart();

        if(aiplayer != null)
        {
            Debug.Log("aiplayer not null!");
            aiplayer.AIplayerStart();
        }
        spawnControl.SummonBase();
    }
    

    protected virtual void SummonProcedure(int laneNumber)
    {
        Debug.Log("monsterType " + monsterType + "typeCreature" + typeCreature);
        spawnControl.SpawnCreatureLane(laneNumber, GameControl.Sides.Friendly, monsterType);

    }

    void ChooseCreature(int type)
    {
        if (type != monsterType)
        {
            foreach (GameObject buttons in lanes)
            {
                buttons.SetActive(true);
            }
            monsterType = type;
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

    // Update is called once per frame
    void Update()
    {

    }

    private void LateUpdate()
    {
        if (Input.GetKey("right"))
        {
            targetPosition = mainCamera.transform.position + cameraSpeed;
        }
        else if (Input.GetKey("left"))
        {
            targetPosition = mainCamera.transform.position - cameraSpeed;
        }

        smoothPosition = Vector3.Lerp(mainCamera.transform.position, targetPosition, 0.05f);
        mainCamera.transform.position = smoothPosition;
    }

    public int GetMaxLanes()
    {
        return maxLanes;
    }
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
