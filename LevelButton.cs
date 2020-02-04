using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class LevelButton : MonoBehaviour
{
    [SerializeField]
    private int stageLevel;
    [SerializeField]
    private int manaAmount;
    [SerializeField]
    private float manaRegenTime;
    [SerializeField]
    private float creatureSpawnTime;
    [SerializeField]
    private Vector2Int[] hostileType = new Vector2Int[7];
    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.GetComponent<Button>().interactable = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitLevel()
    {
        PublicLevel.SetLevel(hostileType, manaAmount, manaRegenTime, creatureSpawnTime, stageLevel);
        SceneManager.LoadScene("DefaultIngame");
    }
}
