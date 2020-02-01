using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class GameData : GameDataForm
{
    #region save/load functions

    public void SaveGameData(int _playerLevel, long _playerWin, Vector2Int[] _friendlyType)
    {
        GameDataForm currentGameData = new GameData();

        currentGameData.SetGameData(_playerLevel, _playerWin, _friendlyType);

        SaveGame(currentGameData, GetFileName());
    }

    public void LoadGameData(int _playerLevel, long _playerWin, Vector2Int[] _friendlyType)
    {
        GameDataForm currentGameData = LoadGame(GetFileName());
        //when local save does not exist
        if (currentGameData == null)
        {
            //initialize player info
            _playerLevel = 1;
            _playerWin = 0;
            //initialize friendlyType
            for (int i = 0; i < 5; ++i)
            {
                _friendlyType[i] = new Vector2Int(i, 0);
            }
        }
        else
        {
            //load player info from local save
            _playerLevel = currentGameData.GetPlayerLevel();
            _playerWin = currentGameData.GetPlayerWin();
            for (int i = 0; i < 5; ++i)
            {
                _friendlyType[i] = new Vector2Int(currentGameData.GetFriendlyType()[i].x, currentGameData.GetFriendlyType()[i].y);
            }
        }
    }

    public static bool SaveGame(GameDataForm saveGame, string name)
    {
        BinaryFormatter formatter = new BinaryFormatter();

        using (FileStream stream = new FileStream(GetSavePath(name), FileMode.Create))
        {
            try
            {
                formatter.Serialize(stream, saveGame);
            }
            catch (Exception)
            {
                return false;
            }
        }

        return true;
    }

    public static GameData LoadGame(string name)
    {
        if (!DoesSaveGameExist(name))
        {
            return null;
        }

        BinaryFormatter formatter = new BinaryFormatter();

        using (FileStream stream = new FileStream(GetSavePath(name), FileMode.Open))
        {
            try
            {
                return formatter.Deserialize(stream) as GameData;
            }
            catch (Exception)
            {
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
