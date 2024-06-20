using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class FileDataHandler
{
    string dataDirPath = "";
    string dataFileName = "";
    bool useEncryption = false;
    readonly string encryptionCode = "bpm.goamzaigc.com";
    public FileDataHandler(string dataDirPath, string dataFileName, bool useEncryption)
    {
        this.dataDirPath = dataDirPath;
        this.dataFileName = dataFileName;
        this.useEncryption = useEncryption;
    }
    public void Save(GameData data)
    {
        var fullPath = Path.Combine(dataDirPath, dataFileName);
        try
        {
            // Create the dictionary path in case it does not exist
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            // Serialize C# game data into JSON
            var dataToStore = JsonUtility.ToJson(data, true);
            // Encrypt
            if (useEncryption)
            {
                dataToStore = EncryptDecrypt(dataToStore);
            }
            // Write the file to the file system
            using (FileStream stream = new(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new(stream))
                {
                    writer.Write(dataToStore);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error occured when trying to save data to file:" + fullPath + '\n' + e);
        }
    }
    public GameData Load()
    {
        var fullPath = Path.Combine(dataDirPath, dataFileName);
        GameData loadedData = null;
        if (File.Exists(fullPath))
        {
            try
            {
                // Load serialized data from the file
                var dataToLoad = "";
                using(FileStream stream = new(fullPath, FileMode.Open))
                {
                    using(StreamReader reader = new(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }
                // Decrypt
                if (useEncryption)
                {
                    dataToLoad = EncryptDecrypt(dataToLoad);
                }
                // Deserialize the data
                loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
            }
            catch (Exception e)
            {
                Debug.LogError("Error occured when trying to load data from file:" + fullPath + '\n' + e);
            }
        }
        return loadedData; 
    }
    private string EncryptDecrypt(string data)
    {
        string modifiedData = "";
        for (int i = 0; i < data.Length; i++)
        {
            modifiedData += (char)(data[i] ^ encryptionCode[i % encryptionCode.Length]);
        }
        return modifiedData;
    }
}
