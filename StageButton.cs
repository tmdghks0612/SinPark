using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class StageButton : MonoBehaviour
{
    //parameter that handle difficulties of the game.
    //level of the stage
    [SerializeField]
    private int stageLevel;
    [SerializeField]
    private int manaAmount;
    [SerializeField]
    private float manaRegenTime;
    [SerializeField]
    private float creatureSpawnTime;

    //Type of creatures used by hostile
    [SerializeField]
    private Vector2Int[] hostileType = new Vector2Int[PublicLevel.hostileTypeCreatureNum];
 
    
    //Save parameters stored in the button to the PublicLevel and load scene
    public void InitLevel()
    {
        PublicLevel.SetLevel(hostileType, manaAmount, manaRegenTime, creatureSpawnTime, stageLevel);
        SceneManager.LoadScene("DefaultIngame");
    }

    //Save parameters stored in the button to the PublicLevel and load scene
    public void InitLevelMultiplayer()
    {
        PublicLevel.SetLevel(hostileType, manaAmount, manaRegenTime, creatureSpawnTime, stageLevel);
        SceneManager.LoadScene("DefaultIngameCopy");
    }

    public int GetStageLevel()
    {
        return stageLevel;
    }
}

