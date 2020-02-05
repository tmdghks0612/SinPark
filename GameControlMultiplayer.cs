using System.Collections;
using System.Collections.Generic;

using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class GameControlMultiplayer : GameControl
{
    protected override void Start()
    {
        // call GameControl.Start()
        base.Start();
    }

    // Send spawn request of creature to server
    protected override void SummonProcedure(int laneNumber)
    {
        StartCoroutine(SendSpawnRequest(laneNumber, GameControl.Sides.Friendly, monsterType, upgradeType[monsterType]));
        
        // test codes for spawning creatures easily
        Debug.Log("monsterType " + monsterType + "typeCreature" + typeCreature);
        spawnControl.SpawnCreatureLane(laneNumber, GameControl.Sides.Friendly, monsterType);
        spawnControl.SpawnCreatureLane(laneNumber, GameControl.Sides.Hostile, monsterType);
    }

    // create and send a SpawnRequestForm
    IEnumerator SendSpawnRequest(int laneNumber, GameControl.Sides side, int creatureType, int upgradeType)
    {
        SpawnRequestForm newRequestForm = new SpawnRequestForm();
        // initialize variables with parameters passed from SummonProcedure()
        newRequestForm.laneNumber = laneNumber;
        newRequestForm.side = side;
        newRequestForm.creatureType = creatureType;
        newRequestForm.upgradeType = upgradeType;
        // serialize class instance into JSON API
        string newJson = JsonUtility.ToJson(newRequestForm);

        // send post request to a hidden url of server
        UnityWebRequest newRequest = new UnityWebRequest(SpawnRequestForm.getUrl(), "POST");
        byte[] bodyByte = Encoding.UTF8.GetBytes(newJson);
        newRequest.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyByte);
        newRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        // set request header to JSON
        newRequest.SetRequestHeader("Content-Type", "application/json");
        yield return newRequest.SendWebRequest();
    }

    // update PublicLevel to save win on exitting game
    public override void GameOver(bool isWin)
    {
        if (isWin)
        {
            PublicLevel.SetPlayerWin(PublicLevel.GetPlayerWin() + 1);
        }
    }
}
