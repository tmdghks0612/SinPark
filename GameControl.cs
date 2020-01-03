using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControl : MonoBehaviour
{
    public CombatControl combatControl;
    public SpawnControl spawnControl;

    //variables defined overall in game
    public int maxLanes = 3;
    public int maxUnits = 3;

    public enum Sides { Friendly, Hostile };

    //

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
