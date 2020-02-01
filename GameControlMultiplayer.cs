using System.Collections;
using System.Collections.Generic;

using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class GameControlMultiplayer : GameControl
{

    protected override void Start()
    {
        base.Start();
        //connect to tcp server
        //clientListener.ConnectToServer();
    }

    protected override void SummonProcedure(int laneNumber)
    {
        StartCoroutine(SendSpawnRequest(laneNumber, GameControl.Sides.Friendly, monsterType, upgradeType[monsterType]));
        
        Debug.Log("monsterType " + monsterType + "typeCreature" + typeCreature);
        spawnControl.SpawnCreatureLane(laneNumber, GameControl.Sides.Friendly, monsterType);
        spawnControl.SpawnCreatureLane(laneNumber, GameControl.Sides.Hostile, monsterType);
    }

    IEnumerator SendSpawnRequest(int laneNumber, GameControl.Sides side, int creatureType, int upgradeType)
    {
        SpawnRequestForm newRequestForm = new SpawnRequestForm();
        newRequestForm.laneNumber = laneNumber;
        newRequestForm.side = side;
        newRequestForm.creatureType = creatureType;
        newRequestForm.upgradeType = upgradeType;
        string newJson = JsonUtility.ToJson(newRequestForm);

        UnityWebRequest newRequest = new UnityWebRequest(SpawnRequestForm.getUrl(), "POST");
        byte[] bodyByte = Encoding.UTF8.GetBytes(newJson);
        newRequest.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyByte);
        newRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        newRequest.SetRequestHeader("Content-Type", "application/json");
        yield return newRequest.SendWebRequest();
    }
}
