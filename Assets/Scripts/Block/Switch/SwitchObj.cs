using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SwitchObj : MonoBehaviour, IDataPersistence
{
    public bool saving;
    public string id;
    [ContextMenu("Generate Guid")]
    void GenerateGuid()
    {
        id = System.Guid.NewGuid().ToString();
    }
    public bool Switched { get; set; }
    public delegate void SwitchDelegate(bool state);
    public SwitchDelegate miMethod;
    public bool Switch()
    {
        Switched ^= true;
        miMethod(Switched);
        return Switched;
    }
    public bool Switch(bool tarState)
    {
        Switched = tarState;
        miMethod(Switched);
        return Switched;
    }
    public void LoadData(GameData gameData)
    {
        if (saving)
        {
            bool swi = Switched;
            gameData.switchOpened.TryGetValue(id, out swi);
            Switched = swi;
            Switch(Switched);
        }
    }
    public void SaveData(ref GameData gameData)
    {
        if (saving)
        {
            if (gameData.switchOpened.ContainsKey(id))
            {
                gameData.switchOpened.Remove(id);
            }
            gameData.switchOpened.Add(id, Switched);
        }

    }
}
