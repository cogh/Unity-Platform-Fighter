using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;



/* I think this object should eventually become the only way to change rooms
 * 
 * Not only that, but it should also trigger the room start and room end events of every other object
 */



public class RoomManagerScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Move from first room
        if (SceneManager.GetActiveScene().name == "Ini")
        {
            LoadScene("MainMenu");
        }
    }

    // Go to
    public void LoadScene(string sceneName)
    {
        // Can do "if sceneName = x" stuff here
        if (sceneName == "Level_1")
        {
            PlayerPrefs.SetInt("level_index", 1);
        }

        // Load
        SceneManager.LoadScene(sceneName);
    }

    // Variables
    bool firstRoom = true;
}
