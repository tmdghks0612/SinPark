using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class GameControl : MonoBehaviour
{
    public CombatControl combatControl;
    public SpawnControl spawnControl;
    public ClientListener clientListener;

    //variables defined overall in game
    private int maxLanes = 3;
    private int maxUnits = 100;
    public int typeCreature = 6;
    public int typeUpgrade = 2;
    private Vector3 cameraSpeed;
    private float smoothSpeed;
    private GameObject mainCamera;
    private Vector3 targetPosition;
    private Vector3 smoothPosition;

    //>>> parameters for UI Summon Button may change later

    protected int[] creatureType;
    protected int[] upgradeType;

    public GameObject[] lanes;
    private int monsterType;
    public GameObject[] SummonButton;
    private bool buttonFlag = true;
    //<<<
    public enum Sides { Friendly, Hostile };

    //


    // Start is called before the first frame update
    void Start()
    {
        cameraSpeed = new Vector3(2.0f, 0, 0);
        mainCamera = GameObject.FindWithTag("MainCamera");
        targetPosition = mainCamera.transform.position;
        upgradeType = new int[typeCreature];
        creatureType = new int[typeCreature];
        //initializing creature and its upgrade type later changes will remove this 2 lines @@@@@@@@
        InitUpgradeType();
        InitCreatureType();
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

        //connect to tcp server
        clientListener.ConnectToServer();
           
    }

    void SummonProcedure(int laneNumber)
    {
        StartCoroutine(SendSpawnRequest(laneNumber, GameControl.Sides.Friendly, monsterType, upgradeType[monsterType]));
        Debug.Log("monsterType " + monsterType + "typeCreature" + typeCreature);
        spawnControl.SpawnCreatureLane(laneNumber, GameControl.Sides.Friendly, monsterType, upgradeType[monsterType]);
        spawnControl.SpawnCreatureLane(laneNumber, GameControl.Sides.Hostile, monsterType, upgradeType[monsterType]);

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


    IEnumerator SendSpawnRequest(int laneNumber, GameControl.Sides side, int creatureType, int upgradeType)
    {        
        SpawnRequestForm newRequestForm = new SpawnRequestForm();
        newRequestForm.laneNumber = laneNumber;
        newRequestForm.side = side;
        newRequestForm.creatureType = creatureType;
        newRequestForm.upgradeType = upgradeType;
        string newJson = JsonUtility.ToJson(newRequestForm);

        UnityWebRequest newRequest = new UnityWebRequest(SpawnRequestForm.getUrl(), "POST");
        byte[] bodyByte = Encoding.UTF8.GetBytes(newJson);
        newRequest.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyByte);
        newRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        newRequest.SetRequestHeader("Content-Type", "application/json");
        yield return newRequest.SendWebRequest();
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

    void InitUpgradeType() //임시적으로 해둔것, 나중에는 type을 받아와서 버튼에 저장할수 있도록 해줘야 한다.
    {
        for (int i = 0; i < typeCreature; i++)
        {
            upgradeType[i] = 0;
        }
    }

    private void InitCreatureType()
    {
        for(int i=0; i< typeCreature; ++i)
        {
            creatureType[i] = i;
        }
    }
}
