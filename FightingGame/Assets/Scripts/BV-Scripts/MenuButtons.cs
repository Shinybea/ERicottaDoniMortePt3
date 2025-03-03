using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuButtons : MonoBehaviour
{
    private GameObject _mainMenuPanel;
    private GameObject _levelPanel;
    private GameObject _optionsPanel;
    
    void Start()
    {
        _mainMenuPanel = GameObject.Find("SelectionPanel");
        _levelPanel = GameObject.Find("LevelPanel");
        _optionsPanel = GameObject.Find("OptionsPanel");
    }

    void Update()
    {
        
    }

    public void GoToLevels()
    {
        _mainMenuPanel.SetActive(false);
        _optionsPanel.SetActive(false);
        _levelPanel.SetActive(true);
    }

    public void GoToOptions()
    {
        _mainMenuPanel.SetActive(false);
        _optionsPanel.SetActive(true);
        _levelPanel.SetActive(false);
    }

    public void QuitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
    public void BackToMainMenu()
    {
        _mainMenuPanel.SetActive(true);
        _optionsPanel.SetActive(false);
        _levelPanel.SetActive(false);
    }

    public void SaveOptions()
    {
        Debug.Log("Saving Options");
        //save audio - commands etc.
    }
    
    public void ChooseLevel(Button button)
    {
        string selectedLevelName = button.gameObject.name;
        Debug.Log($"Selected Level: {selectedLevelName}");
        Regex buttonMashLevel = new Regex($@"^{selectedLevelName}$");
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            if (buttonMashLevel.IsMatch(sceneName))
            { 
                SceneManager.LoadSceneAsync(sceneName); 
                return; 
            }
        }
    }
}
