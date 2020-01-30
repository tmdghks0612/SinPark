using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class LevelButton : MonoBehaviour
{
    [SerializeField]
    private int manaAmount;
    [SerializeField]
    private float manaRegenTime;
    [SerializeField]
    private float creatureSpawnTime;
    [SerializeField]
    private int[] creatureType = new int[5];
    [SerializeField]
    private int[] upgradeType = new int[5];
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
        PublicLevel.SetLevel(creatureType, upgradeType, manaAmount, manaRegenTime, creatureSpawnTime);
        SceneManager.LoadScene("DefaultIngame");
    }
}
