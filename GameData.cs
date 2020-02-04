using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class GameData : GameDataForm
{
    #region save/load functions

    public void SaveGameData()
    {
        GameDataForm currentGameData = GameObject.Find("GameDataControl").GetComponent<GameData>();

        currentGameData.SetGameData(PublicLevel.GetPlayerLevel(), PublicLevel.GetPlayerWin(), PublicLevel.friendlyType);

        SaveGame(currentGameData, GetFileName());
    }

    public GameDataForm LoadGameData()//int _playerLevel, long _playerWin, Vector2Int[] _friendlyType)
    {
        GameDataForm currentGameData = LoadGame(GetFileName(), this);
        //when local save does not exist
        if (currentGameData == null)
        {
            /*
            //initialize player info
            _playerLevel = 1;
            _playerWin = 0;
            //initialize friendlyType
            for (int i = 0; i < 5; ++i)
            {
                _friendlyType[i] = new Vector2Int(i, 0);
            }*/
            return null;
        }
        else
        {
            return currentGameData;
            /*
            //load player info from local save
            _playerLevel = currentGameData.GetPlayerLevel();
            _playerWin = currentGameData.GetPlayerWin();
            for (int i = 0; i < 5; ++i)
            {
                _friendlyType[i] = new Vector2Int(currentGameData.GetFriendlyType()[i].x, currentGameData.GetFriendlyType()[i].y);
            }
            */
        }
    }

    public static bool SaveGame(GameDataForm saveGame, string name)
    {
        BinaryFormatter formatter = new BinaryFormatter();

        using (FileStream stream = new FileStream(GetSavePath(name), FileMode.Create))
        {
            Debug.Log("class internal data" + saveGame.GetPlayerLevel().ToString());
            try
            {
                Debug.Log("try save!");
                formatter.Serialize(stream, saveGame.GetSaveString());
                //formatter.Serialize(stream, saveGame);
            }
            catch (Exception)
            {
                Debug.Log("save failed!");
                return false;
            }
        }

        return true;
    }

    public static GameDataForm LoadGame(string name, GameDataForm loadData)
    {
        if (!DoesSaveGameExist(name))
        {
            return null;
        }

        BinaryFormatter formatter = new BinaryFormatter();
        Debug.Log("loading from : " + GetSavePath(name));
        using (FileStream stream = new FileStream(GetSavePath(name), FileMode.Open))
        {
            try
            {
                loadData.loadGameDataFromString(formatter.Deserialize(stream) as String);
                
                return loadData;
            }
            catch (Exception)
            {
                Debug.Log("hello");
                return null;
            }
        }
    }

    public static bool DeleteSaveGame(string name)
    {
        try
        {
            File.Delete(GetSavePath(name));
        }
        catch (Exception)
        {
            return false;
        }

        return true;
    }

    public static bool DoesSaveGameExist(string name)
    {
        return File.Exists(GetSavePath(name));
    }

    private static string GetSavePath(string name)
    {
        return Path.Combine(Application.persistentDataPath, name + ".sav");
    }

    #endregion

    
}
