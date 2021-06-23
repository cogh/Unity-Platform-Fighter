using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubmitScoreScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Get game manager
        gameManagerScript = GameObject.Find("GameManager").GetComponent<GameManagerScript>();

        // Set high score text
        highScoreText.text = "Score: " + gameManagerScript.totalScore.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Text highScoreText;
    GameManagerScript gameManagerScript;
}
