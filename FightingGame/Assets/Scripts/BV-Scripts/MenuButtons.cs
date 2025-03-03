using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuButtons : MonoBehaviour
{
    
    void Start()
    {
        
 
    }

    void Update()
    {
        
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
