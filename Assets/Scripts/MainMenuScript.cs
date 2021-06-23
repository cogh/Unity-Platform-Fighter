using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MainMenuScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //text.text = "High score: " + PlayerPrefs.GetInt("score").ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Play()
    {
        PlayerPrefs.SetInt("level_index", 1);
        PlayerPrefs.SetInt("score", 0);
        SceneManager.LoadScene("Level_1");
    }

    public void Exit()
    {

    }

    public void OpenTwitter()
    {
        Application.OpenURL("http://www.twitter.com/cogh_");
    }

    public Text text;
}
