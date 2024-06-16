using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DataPersistenceManager : MonoBehaviour
{
    [Header("File Storage Config")]
    [SerializeField] string fileName;
    [SerializeField] bool useEncryption;

    private GameData gameData;
    private List<IDataPersistence> dataPersistencesObjects;
    private FileDataHandler dataHandler;
    public static DataPersistenceManager instance;

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
            Debug.LogError("Found more than one DataPersistenceManager in the scene.");
        }
        else
        {
            instance = this;
        }
            
    }
    private void Start()
    {
        dataHandler = new(Application.persistentDataPath, fileName, useEncryption);
        dataPersistencesObjects = FindAllDataPersistenceObjects();
        LoadGame();
    }
    public void NewGame()
    {
        gameData = new();
    }
    public void LoadGame()
    {
        // load from a file using data handler
        gameData = dataHandler.Load();

        // if no file, start a new game
        if(gameData == null)
        {
            Debug.Log("No data found");
            gameData = new();
        }

        // Push the data to all other scirpts that need it
        foreach(IDataPersistence p in dataPersistencesObjects)
        {
            p.LoadData(gameData);
        }
    }
    public void SaveGame()
    {
        // Pass the data from other scripts
        foreach(IDataPersistence p in dataPersistencesObjects)
        {
            p.SaveData(ref gameData); 
        }
        // save that data using the data handler
        dataHandler.Save(gameData);
    }

    private void OnApplicationQuit()
    {
        //SaveGame();
    }
    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>()
            .OfType<IDataPersistence>();
        return new(dataPersistenceObjects);
    }
}
