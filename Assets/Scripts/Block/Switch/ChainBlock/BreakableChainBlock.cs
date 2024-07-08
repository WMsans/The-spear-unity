using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableChainBlock : MonoBehaviour, IDataPersistence
{
    [SerializeField] bool saving = true;
    [SerializeField] string id;
    [ContextMenu("Generate Guid")]
    void GenerateGuid()
    {
        id = System.Guid.NewGuid().ToString();
    }
    public bool Connected { get; private set; }
    Rigidbody2D rb;
    HingeJoint2D joint;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        joint = GetComponent<HingeJoint2D>();
    }
    public void Disconnect()
    {
        Connected = false;
        joint.enabled = false;
    }
    public void LoadData(GameData gameData)
    {
        if (saving)
        {
            var tX = transform.position.x;
            if(!gameData.blockPositionX.TryGetValue(id, out tX))
            {
                tX = transform.position.x;
            }
            var tY = transform.position.y;
            if (!gameData.blockPositionY.TryGetValue(id, out tY))
            {
                tY = transform.position.y;
            }
            transform.position = new(tX, tY);
        }
    }
    public void SaveData(ref GameData gameData)
    {
        if (saving)
        {
            if (gameData.blockPositionX.ContainsKey(id))
            {
                gameData.blockPositionX.Remove(id);
            }
            if (gameData.blockPositionY.ContainsKey(id))
            {
                gameData.blockPositionY.Remove(id);
            }
            gameData.blockPositionX.Add(id, transform.position.x);
            gameData.blockPositionY.Add(id, transform.position.y);
        }
    }
}
