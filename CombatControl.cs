using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatControl : MonoBehaviour
{
    // control instances
    public GameControl gameControl;
    public SpawnControl spawnControl;
    
    // lane and creature initialization variables
    private int maxLanes;
    private int maxUnits;
    private int typeCreature;
    private int typeUpgrade;

    // locks used for synchronizing spawn
    private readonly object lock_friendlyLanes = new object();
    private readonly object lock_hostileLanes = new object();

    // array of missile prefabs
    private GameObject[,] missileArray;

    // structure for sides containing creatureList(lanes)
    // creatureList contains creatures in the lane
    public struct SideLanes
    {
        public List<DefaultCreature>[] creatureList;
    }

    // data structure for lane of creatures in each sides
    private SideLanes friendlyLanes;
    private SideLanes hostileLanes;

    // Start is called before the first frame update
    public void InitCombatControl()
    {
        // variable initialization according to gameControl
        maxLanes = gameControl.GetMaxLanes();
        maxUnits = gameControl.GetMaxUnits();
        typeCreature = gameControl.typeCreature;
        typeUpgrade = gameControl.typeUpgrade;

        InitLanes();
    }

    #region push pop functions

    // push creature called when a creature is spawned in a certain lane
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
        spawnControl.OnUnitDeath();
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
    #endregion

    #region search and attack functions

    // search creatures in melee attack range
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

    // attacks performed to each creature
    public void MeleeAttack(Vector3 currentPosition, float attackRange, int attackDamage, int size, int laneNum, GameControl.Sides side)
    {
        // attack from friendly to hostile
        if (side == GameControl.Sides.Friendly)
        {
            // used lock to stop creature unwanted destroys
            lock(lock_hostileLanes)
            {
                List<DefaultCreature> currentList = hostileLanes.creatureList[laneNum];
                // search creatures within attackrange of friendly lanes in current laneNum
                for (int i = currentList.Count-1; i >= 0; i--)
                {
                    if (currentList[i].transform.position.x - currentPosition.x < attackRange)
                    {
                        currentList[i].DamageTaken(attackDamage, size);
                    }
                }
            }
        }
        // attack from hostile to friendly
        else
        {
            lock(lock_friendlyLanes)
            {
                List<DefaultCreature> currentList = friendlyLanes.creatureList[laneNum];
                //search creatures within attackrange of hostile lanes in current laneNum
                for (int i = currentList.Count-1; i >= 0; i--)
                {
                    if (currentPosition.x - currentList[i].transform.position.x < attackRange)
                    {
                        currentList[i].DamageTaken(attackDamage, size);
                    }
                }
            }
        }
    }

    // heal performed to nearby allies on confronting hostile creature
    public void Heal(Vector3 currentPosition, float attackRange, int attackDamage, int laneNum, GameControl.Sides side)
    {
        //heal friendly creatures
        if (side == GameControl.Sides.Friendly)
        {
            lock (lock_friendlyLanes)
            {
                List<DefaultCreature> currentList = friendlyLanes.creatureList[laneNum];
                
                for (int i = currentList.Count - 1; i >= 0; i--)
                {
                    if (Mathf.Abs(currentList[i].transform.position.x - currentPosition.x) < attackRange)
                    {
                        currentList[i].DamageTaken(attackDamage, 0);
                    }
                }
            }
        }
        //heal hostile creatures
        else
        {
            lock (lock_hostileLanes)
            {
                List<DefaultCreature> currentList = hostileLanes.creatureList[laneNum];
                
                for (int i = currentList.Count - 1; i >= 0; i--)
                {
                    if (Mathf.Abs(currentList[i].transform.position.x - currentPosition.x) < attackRange)
                    {
                        currentList[i].DamageTaken(attackDamage, 0);
                    }
                }
            }
        }
    }

    #endregion

    // spawn a missile that damage on hit
    public void MissileAttack( Vector3 currentPosition, GameObject projectile, int attackDamage, int laneNum, GameControl.Sides side)
    {
        GameObject newObject = Instantiate<GameObject>(projectile); 
        DefaultMissile newMissile = newObject.GetComponent<DefaultMissile>();
        newMissile.SetMissile( currentPosition, side, laneNum );
    }

    // initializing functions

    // initialize struct of lists
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

   
}
