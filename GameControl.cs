using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameControl : MonoBehaviour
{
    public CombatControl combatControl;
    public SpawnControl spawnControl;

    //variables defined overall in game
    public int maxLanes = 3;
    public int maxUnits = 100;
    public int typeCreature = 2;

    //>>> parameters for UI Summon Button may change later
    public GameObject[] lanes;
    private int monsterType;
    public GameObject[] SummonButton;
    private bool buttonFlag = true;
    //<<<
    public enum Sides {Friendly, Hostile};

    //

    // Start is called before the first frame update
    void Start()
    {
        for(int i=0; i<SummonButton.Length; i++)
        {
            int temp = i;
            SummonButton[i].GetComponent<Button>().onClick.AddListener(delegate { ChooseLane(temp); });
        }
        monsterType = 0;
        for(int i=0; i<lanes.Length; i++)
        {
            int temp = i;
            lanes[i].GetComponent<Button>().onClick.AddListener(delegate { SummonProcedure(temp); });
            lanes[i].SetActive(false);
        }
    }

    void SummonProcedure(int laneNumber)
    {
        Debug.Log("Lane " + laneNumber);
        spawnControl.SpawnCreatureLane(laneNumber, GameControl.Sides.Friendly, monsterType);
        
    }

    void ChooseLane(int type)
    {
        monsterType = type;
        Debug.Log("Type " + monsterType);
        if (buttonFlag)
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
}
