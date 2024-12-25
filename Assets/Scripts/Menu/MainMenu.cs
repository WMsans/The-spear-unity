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
    [SerializeField] List<SceneField> levelScene;
    List<AsyncOperation> _scenesToLoad = new List<AsyncOperation>();
    private void Awake()
    {
        
        loadingBarObject.SetActive(false);
        
    }
    public void PlayGame()
    {
        loadingBarObject.SetActive(true);
        HideMenu();
        //Load the scene
        _scenesToLoad.Add(SceneManager.LoadSceneAsync(persistanceGameplay));
        foreach(var scene in levelScene)
            _scenesToLoad.Add(SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive));   
        
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
        var scenesNum = _scenesToLoad.Count;
        foreach (var go in _scenesToLoad)
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
