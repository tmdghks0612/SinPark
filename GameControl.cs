using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameControl : MonoBehaviour
{
    public CombatControl combatControl;
    public SpawnControl spawnControl;

    //variables defined overall in game
    private int maxLanes = 3;
    private int maxUnits = 100;
    public int typeCreature = 3;
    public int typeUpgrade = 2;
    private Vector3 cameraSpeed;
    private float smoothSpeed;
    private GameObject mainCamera;
    private Vector3 targetPosition;
    private Vector3 smoothPosition;

    //>>> parameters for UI Summon Button may change later
    public GameObject[] lanes;
    private int monsterType;
    private int[] upgradeType;
    public GameObject[] SummonButton;
    private bool buttonFlag = true;
    //<<<
    public enum Sides {Friendly, Hostile};

    //
    

    // Start is called before the first frame update
    void Start()
    {
        cameraSpeed = new Vector3(2.0f, 0, 0);
        mainCamera = GameObject.FindWithTag("MainCamera");
        targetPosition = mainCamera.transform.position;
        for(int i=0; i<SummonButton.Length; i++)
        {
            int temp = i;
            SummonButton[i].GetComponent<Button>().onClick.AddListener(delegate { ChooseCreature(temp); });
        }
        monsterType = 0;
        for(int i=0; i<lanes.Length; i++)
        {
            int temp = i;
            lanes[i].GetComponent<Button>().onClick.AddListener(delegate { SummonProcedure(temp); });
            lanes[i].SetActive(false);
        }

        upgradeType = new int[typeCreature];
        setUpgrade();
    }

    void SummonProcedure(int laneNumber)
    {
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

    void setUpgrade() //임시적으로 해둔것, 나중에는 type을 받아와서 버튼에 저장할수 있도록 해줘야 한다.
    {
        for(int i=0; i<typeCreature; i++)
        {
            upgradeType[i] = 0;
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

}
