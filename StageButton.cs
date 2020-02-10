using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class StageButton : MonoBehaviour
{
    // parameter that handle difficulties of the game.
    // level of the stage
    [SerializeField]
    private int stageLevel;
    [SerializeField]
    private int manaAmount;
    [SerializeField]
    private float manaRegenTime;
    [SerializeField]
    private float creatureSpawnTime;
    // if this stage contains boss
    [SerializeField]
    private bool isBoss;

    [SerializeField]
    private GameObject bossPrefab;

    //Type of creatures used by hostile
    [SerializeField]
    private Vector2Int[] hostileType = new Vector2Int[PublicLevel.hostileTypeCreatureNum];
 
    
    //Save parameters stored in the button to the PublicLevel and load scene
    public void InitLevel()
    {
        PublicLevel.SetLevel(hostileType, manaAmount, manaRegenTime, creatureSpawnTime, stageLevel, isBoss, bossPrefab);
        SceneManager.LoadScene("DefaultIngame");
    }

    //Save parameters stored in the button to the PublicLevel and load scene
    public void InitLevelMultiplayer()
    {
        PublicLevel.SetLevel(hostileType, manaAmount, manaRegenTime, creatureSpawnTime, stageLevel, false, null);
        SceneManager.LoadScene("DefaultIngameCopy");
    }

    public int GetStageLevel()
    {
        return stageLevel;
    }
}

