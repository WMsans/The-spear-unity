using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Main Menus")]
    [SerializeField] GameObject loadingBarObject;
    [SerializeField] GameObject[] objectsToHide;
    [SerializeField] Slider loadingBar;
    [Header("Scenes to Load")]
    [SerializeField] SceneField persistanceGameplay;
    [SerializeField] SceneField levelScene;
    List<AsyncOperation> scenesToLoad = new List<AsyncOperation>();
    private void Awake()
    {
        
        loadingBarObject.SetActive(false);
        
    }
    public void PlayGame()
    {
        loadingBarObject.SetActive(true);
        HideMenu();
        //Load the scene
        scenesToLoad.Add(SceneManager.LoadSceneAsync(persistanceGameplay));
        scenesToLoad.Add(SceneManager.LoadSceneAsync(levelScene, LoadSceneMode.Additive));   
        
        //Update the bar
        StartCoroutine(ProgressLoadingBar());
    }

    public void QuitGame()
    {
        Debug.Log("Game closed");
        Application.Quit();
    }
    void HideMenu()
    {
        foreach(GameObject go in objectsToHide)
        {
            go.SetActive(false) ;
        }
    }
    IEnumerator ProgressLoadingBar()
    {
        var loadProgress = 0f;
        var scenesNum = scenesToLoad.Count;
        foreach (var go in scenesToLoad)
        {
            while (go.isDone)
            {
                loadProgress += go.progress;
                loadingBar.value = loadProgress / scenesNum;
                yield return null;
            }
        }
    }
}
