using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour
{
    [SerializeField]
    private GameObject upgradeShop;
    [SerializeField]
    private GameObject stageMap;
    [SerializeField]
    private Button[] upgradeButton = new Button[PublicLevel.friendlyTypeCreatureNum * PublicLevel.friendlyTypeUpgradeNum];
    [SerializeField]
    private Button[] locationButton = new Button[5];
    [SerializeField]
    private Vector2Int[] upgradeOption = new Vector2Int[PublicLevel.friendlyTypeCreatureNum * PublicLevel.friendlyTypeUpgradeNum];
    private GameObject listObject;
    private Vector2Int changingInfo;
    private Sprite Image;
    void Start()
    {
        upgradeButton[0].GetComponent<Button>().onClick.AddListener(delegate { TargetCreature(upgradeOption[0]); });
        for(int i=0; i<5; i++)
        {
            int temp = i;
            locationButton[temp].onClick.AddListener(delegate { ChangeCreature(temp); });
        }
        upgradeShop.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void ChangeCreature(int location)
    {
        Debug.Log(location);
        PublicLevel.friendlyCreatureList[location] = PublicLevel.friendlyPrefab[changingInfo.x, changingInfo.y];
        PublicLevel.friendlyImageList[location] = PublicLevel.friendlyImage[changingInfo.x, changingInfo.y];
        //LoadUpgrade();
        upgradeShop.transform.GetChild(2 + location).GetChild(0).GetComponent<Image>().sprite = PublicLevel.friendlyImageList[location];
        for (int i = 0; i < 5; i++)
        {
            locationButton[i].interactable = false;
        }

    }
    void TargetCreature(Vector2Int type)
    {
        Debug.Log("in");
        changingInfo = type;
        
        for(int i=0; i<5; i++)
        {
            locationButton[i].interactable = true;
        }
        
    }
    public void LoadUpgrade()
    {
        upgradeShop.SetActive(true);
        for (int i = 0; i < 5; i++)
        {
            listObject = upgradeShop.transform.GetChild(2 + i).GetChild(0).gameObject;
            listObject.GetComponent<Image>().sprite = PublicLevel.friendlyImageList[i];
            locationButton[i].interactable = false;
        }
        
        stageMap.SetActive(false);
    }
    public void LoadMap()
    {
        stageMap.SetActive(true);
        upgradeShop.SetActive(false);
    }
}
