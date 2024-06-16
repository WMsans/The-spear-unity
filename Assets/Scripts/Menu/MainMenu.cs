using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        DataPersistenceManager.instance.LoadGame();
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Debug.Log("Game closed");
        Application.Quit();
    }
}
