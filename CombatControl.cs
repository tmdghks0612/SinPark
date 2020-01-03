using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatControl : MonoBehaviour
{
    GameControl gameControl;
    
    private int maxLanes;
    private int maxUnits;

    //structure for sides containing creatureList(lanes)
    //creatureList contains creatures in the lane
    protected struct SideLanes
    {
        public List<DefaultCreature>[] creatureList;
    }

    protected SideLanes friendlyLanes;
    protected SideLanes hostileLanes;

    // Start is called before the first frame update
    void Start()
    {
        InitLanes();

        maxLanes = gameControl.maxLanes;
        maxUnits = gameControl.maxUnits;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //push creature called when a creature is spawned in a certain lane
    public void PushCreature(int laneNum, GameControl.Sides side, DefaultCreature newCreature)
    {
        if(side == GameControl.Sides.Friendly)
        {
            //add the newCreature to friendly lanes in current laneNum
            friendlyLanes.creatureList[laneNum].Add(newCreature);
        }
        else
        {
            //add the newCreature to hostile lanes in current laneNum
            hostileLanes.creatureList[laneNum].Add(newCreature);
        }
    }

    //pop creature called when a creature died in a certain lane
    public void PopCreature(int laneNum, GameControl.Sides Side, DefaultCreature deadCreature)
    {
        if (Side == GameControl.Sides.Friendly)
        {
            //remove the deadCreature to friendly lanes in current laneNum
            friendlyLanes.creatureList[laneNum].Remove(deadCreature);
        }
        else
        {
            //remove the deadCreature to hostile lanes in current laneNum
            hostileLanes.creatureList[laneNum].Remove(deadCreature);
        }
    }

    //initializing functions

    //initialize struct of lists
    public void InitLanes()
    {
        //initialize struct SideLanes
        friendlyLanes = new SideLanes();
        hostileLanes = new SideLanes();

        //initialize creatureList in SideLanes
        //maxLanes contain the number of lanes available in the game
        friendlyLanes.creatureList = new List<DefaultCreature>[maxLanes];
        hostileLanes.creatureList = new List<DefaultCreature>[maxLanes];
    }
}
