using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatControl : MonoBehaviour
{
    public GameControl gameControl;
    
    private int maxLanes;
    private int maxUnits;
    private int typeCreature;

    private readonly object lock_friendlyLanes = new object();
    private readonly object lock_hostileLanes = new object();

    private GameObject[] missileArray;

    //structure for sides containing creatureList(lanes)
    //creatureList contains creatures in the lane
    public struct SideLanes
    {
        public List<DefaultCreature>[] creatureList;
    }

    private SideLanes friendlyLanes;
    private SideLanes hostileLanes;

    // Start is called before the first frame update
    void Start()
    {
        maxLanes = gameControl.maxLanes;
        maxUnits = gameControl.maxUnits;
        typeCreature = gameControl.typeCreature;

        InitLanes();
        InitMissiles();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //functions to push and pop from creature lists

    //push creature called when a creature is spawned in a certain lane
    public void PushCreature( int laneNum, GameControl.Sides side, DefaultCreature newCreature )
    {
        if (side == GameControl.Sides.Friendly)
        {
            lock (lock_friendlyLanes)
            {
                //add the newCreature to friendly lanes in current laneNum
                friendlyLanes.creatureList[laneNum].Add(newCreature);
            }
        }
        else
        {
            lock (lock_hostileLanes)
            {
                //add the newCreature to hostile lanes in current laneNum
                hostileLanes.creatureList[laneNum].Add(newCreature);
            }
        }
    }

    //pop creature called when a creature died in a certain lane
    public void PopCreature( int laneNum, GameControl.Sides side, DefaultCreature deadCreature )
    {
        if (side == GameControl.Sides.Friendly)
        {
            //remove the deadCreature to friendly lanes in current laneNum
            lock (lock_friendlyLanes)
            {
                //remove the deadCreature to friendly lanes in current laneNum
                friendlyLanes.creatureList[laneNum].Remove(deadCreature);
            }
        }
        else
        {
            lock (lock_hostileLanes)
            {
                //remove the deadCreature to hostile lanes in current laneNum
                hostileLanes.creatureList[laneNum].Remove(deadCreature);
            }
        }
    }

    //functions to search and attack according to creature list

    public bool SearchCreature( Vector3 currentPosition, float attackRange, int laneNum, GameControl.Sides side )
    {
        //attack from friendly to hostile
        if (side == GameControl.Sides.Friendly)
        {
            lock (lock_hostileLanes)
            {
                //search creatures within attackrange of friendly lanes in current laneNum
                foreach (DefaultCreature currentCreature in hostileLanes.creatureList[laneNum])
                {

                    if (currentCreature.transform.position.x - currentPosition.x < attackRange)
                    {
                        return true;
                    }
                }
            }
        }
        //attack from hostile to friendly
        else
        {
            lock (lock_friendlyLanes)
            {
                //search creatures within attackrange of hostile lanes in current laneNum
                foreach (DefaultCreature currentCreature in friendlyLanes.creatureList[laneNum])
                {
                    if (currentPosition.x - currentCreature.transform.position.x < attackRange)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public void MeleeAttack(Vector3 currentPosition, float attackRange, int attackDamage, int laneNum, GameControl.Sides side)
    {
        //attack from friendly to hostile
        if (side == GameControl.Sides.Friendly)
        {
            lock(lock_hostileLanes)
            {
                List<DefaultCreature> currentList = hostileLanes.creatureList[laneNum];
                //search creatures within attackrange of friendly lanes in current laneNum
                for (int i = currentList.Count-1; i >= 0; i--)
                {
                    if (currentList[i].transform.position.x - currentPosition.x < attackRange)
                    {
                        currentList[i].DamageTaken(attackDamage);
                    }
                }
            }
        }
        //attack from hostile to friendly
        else
        {
            lock(lock_friendlyLanes)
            {
                List<DefaultCreature> currentList = friendlyLanes.creatureList[laneNum];
                //search creatures within attackrange of hostile lanes in current laneNum
                for (int i = currentList.Count-1; i >= 0; i--)
                {
                    if (currentList[i].transform.position.x - currentPosition.x < attackRange)
                    {
                        currentList[i].DamageTaken(attackDamage);
                    }
                }
            }
        }
    }

    public void MissileAttack( Vector3 currentPosition, int missileType, int attackDamage, int laneNum, GameControl.Sides side)
    {
        GameObject newObject = Instantiate<GameObject>(missileArray[missileType]);
        DefaultMissile newMissile = newObject.GetComponent<DefaultMissile>();
        newMissile.SetMissile( currentPosition, side );
    }

    //initializing functions

    //initialize struct of lists
    public void InitLanes()
    {
        //initialize creatureList in SideLanes
        //maxLanes contain the number of lanes available in the game
        friendlyLanes.creatureList = new List<DefaultCreature>[maxLanes];
        hostileLanes.creatureList = new List<DefaultCreature>[maxLanes];

        for ( int i = 0; i < maxLanes; ++i)
        {
            friendlyLanes.creatureList[i] = new List<DefaultCreature>();
            hostileLanes.creatureList[i] = new List<DefaultCreature>();
        }
    }

    public void InitMissiles()
    {
        missileArray = new GameObject[typeCreature];
        //find and load creature prefabs from folder 'creature#'
        for(int i = 0; i < typeCreature; i++)
        {
            missileArray[i] = Resources.Load("creature" + i.ToString() + "/creature" + i.ToString() + "Projectile") as GameObject;
        }

    }
}
