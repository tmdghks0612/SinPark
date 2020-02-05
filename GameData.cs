using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class GameData : GameDataForm
{
    #region save/load functions

    // create and save a GameDataForm
    public void SaveGameData()
    {
        // find GameData in the scene
        GameDataForm currentGameData = GameObject.Find("GameDataControl").GetComponent<GameData>();
        // set GameData variables according to PublicLevel
        currentGameData.SetGameData(PublicLevel.GetPlayerLevel(), PublicLevel.GetPlayerWin(), PublicLevel.friendlyType);
        // save current GameData
        SaveGame(currentGameData, GetFileName());
    }

    // return GameDataForm if found, else return null
    public GameDataForm LoadGameData()
    {
        GameDataForm currentGameData = LoadGame(GetFileName(), this);
        return currentGameData;
    }

    // save GameData as a file in persistent datapath
    public static bool SaveGame(GameDataForm saveGame, string name)
    {
        // serializer
        BinaryFormatter formatter = new BinaryFormatter();

        // Open a file stream as create(write)
        using (FileStream stream = new FileStream(GetSavePath(name), FileMode.Create))
        {
            try
            {
                // serialize GameData using a converted format of string
                formatter.Serialize(stream, saveGame.GetSaveString());
            }
            catch (Exception)
            {
                // when save failed
                return false;
            }
        }

        return true;
    }

    // return GameDataForm from saved file, if none exist, return null
    public static GameDataForm LoadGame(string name, GameDataForm loadData)
    {
        if (!DoesSaveGameExist(name))
        {
            //when file does not exist
            return null;
        }

        // deserializer
        BinaryFormatter formatter = new BinaryFormatter();

        // temporary stream for taking string input
        using (FileStream stream = new FileStream(GetSavePath(name), FileMode.Open))
        {
            try
            {
                // get formatted string from serialized file
                loadData.loadGameDataFromString(formatter.Deserialize(stream) as String);
                return loadData;
            }
            catch (Exception)
            {
                //when load file was found but failed to read
                return null;
            }
        }
    }

    #endregion

    // delete a existing save
    public static bool DeleteSaveGame(string name)
    {
        try
        {
            //delete the file
            File.Delete(GetSavePath(name));
        }
        catch (Exception)
        {
            return false;
        }

        return true;
    }

    // return true if save file exists, null if none exists
    public static bool DoesSaveGameExist(string name)
    {
        return File.Exists(GetSavePath(name));
    }

    // return save / load path for a given name of file
    private static string GetSavePath(string name)
    {
        return Path.Combine(Application.persistentDataPath, name + ".sav");
    }

    

    
}
