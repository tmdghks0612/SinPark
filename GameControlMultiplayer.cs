using System.Collections;
using System.Collections.Generic;
using System.Net;
using System;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class GameControlMultiplayer : GameControl
{

    private int bufferSize = 1024;
    protected override void Start()
    {
        // call GameControl.Start()
        base.Start();
        aiplayer.AIplayerStop();
    }

    // Send spawn request of creature to server
    protected override void SummonProcedure(int laneNum)
    {
        StartCoroutine(SendSpawnRequest(laneNum, GameControl.Sides.Friendly, selectedCreatureType));
    }

    // create and send a SpawnRequestForm
    IEnumerator SendSpawnRequest(int _laneNum, GameControl.Sides _side, int _selectedCreatureType)
    {

        try
        {
            Byte[] buffer = new Byte[bufferSize];

            // save variables to class sending through socket
            SpawnRequestForm spawnRequestForm = new SpawnRequestForm();
            spawnRequestForm.SetlaneNum(_laneNum);
            spawnRequestForm.SetSelectedCreatureType(_selectedCreatureType);
            spawnRequestForm.SetSide(_side);
            string serverMessage = JsonUtility.ToJson(spawnRequestForm);
            buffer = Encoding.ASCII.GetBytes(serverMessage);
            serverControl.GetServerStream().Write(buffer, 0, buffer.Length);

            ClearBuffer(buffer);
        }
        catch (SocketException socketException)
        {
            Debug.Log("SocketException " + socketException.ToString());
        }

        yield return null;
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

    public void ClearBuffer(byte[] buffer)
    {
        for (int i = 0; i < buffer.Length; ++i)
        {
            buffer[i] = 0;
        }
    }
}
