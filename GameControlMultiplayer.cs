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
    // queue for which spawn request waits
    private Queue<SpawnRequestForm> spawnQueue = new Queue<SpawnRequestForm>();
    private int bufferSize = 1024;

    //variable for synchronizing spawnQueue
    private readonly object lock_spawn = new object();
    //check rate for checking if queue is empty
    private float checkRate = 0.01f;

    protected override void Start()
    {
        // call GameControl.Start()
        base.Start();
        aiplayer.AIplayerStop();
        InvokeRepeating("CheckQueue", checkRate, checkRate);
    }

    // check queue if there are any creatures to spawn
    public void CheckQueue()
    {
        lock (lock_spawn)
        {
            if (spawnQueue.Count == 0)
            {
                return;
            }
            else
            {
                SpawnRequestForm newRequestForm;
                CancelInvoke("CheckQueue");
                while (spawnQueue.Count != 0)
                {
                    newRequestForm = spawnQueue.Dequeue();
                    spawnControl.SummonCreature(newRequestForm.GetlaneNum(), newRequestForm.GetSide(), newRequestForm.GetSelectedCreatureType());
                }
                InvokeRepeating("CheckQueue", checkRate, checkRate);
            }
        }
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

            // create a JSON string with SpawnRequestForm instance
            string serverMessage = JsonUtility.ToJson(spawnRequestForm);
            buffer = Encoding.ASCII.GetBytes(serverMessage);
            serverControl.GetServerStream().Write(buffer, 0, buffer.Length );

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
        try
        {
            Debug.Log(jsonString);
            SpawnRequestForm newRequestForm = new SpawnRequestForm();
            newRequestForm = JsonUtility.FromJson<SpawnRequestForm>(jsonString);
            lock (lock_spawn)
            {
                spawnQueue.Enqueue(newRequestForm);
            }
        }
        catch (ArgumentException argumentException)
        {
            Debug.Log("ArgumentException " + argumentException.ToString());
        }
        return;
    }

    // update PublicLevel to save win on exitting game
    public override void GameOver(bool isWin)
    {
        if (isWin)
        {
            PublicLevel.SetPlayerWin(PublicLevel.GetPlayerWin() + 1);
        }
        base.GameOver(true);
    }

    public void ClearBuffer(byte[] buffer)
    {
        for (int i = 0; i < buffer.Length; ++i)
        {
            buffer[i] = 0;
        }
    }
}
