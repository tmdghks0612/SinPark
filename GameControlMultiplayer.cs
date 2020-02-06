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
    protected override void SummonProcedure(int laneNum)
    {
        StartCoroutine(SendSpawnRequest(laneNum, GameControl.Sides.Friendly, selectedCreatureType));
    }

    // create and send a SpawnRequestForm
    IEnumerator SendSpawnRequest(int _laneNum, GameControl.Sides _side, int _selectedCreatureType)
    {
        SpawnRequestForm newRequestForm = new SpawnRequestForm();
        // initialize variables with parameters passed from SummonProcedure()
        newRequestForm.SetlaneNum(_laneNum);
        newRequestForm.SetSide(_side);
        newRequestForm.SetSelectedCreatureType(_selectedCreatureType);
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

    // create and set information of SpawnRequestForm
    public void ReceiveSpawnRequest(string jsonString)
    {
        SpawnRequestForm newRequestForm = new SpawnRequestForm();
        newRequestForm = JsonUtility.FromJson<SpawnRequestForm>(jsonString);
        spawnControl.SummonCreature(newRequestForm.GetlaneNum(), newRequestForm.GetSide(), newRequestForm.GetSelectedCreatureType());
        return;
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
