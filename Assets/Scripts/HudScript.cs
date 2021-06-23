using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HudScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameManagerScript = GameObject.Find("GameManager").GetComponent<GameManagerScript>();

    }

    // Update is called once per frame
    void Update()
    {
        // Health text
        healthText.text = "Health: " + gameManagerScript.health;

        // Score text
        scoreText.text = "Score: " + (gameManagerScript.totalScore + gameManagerScript.score);

        // Time text
        timeText.text = "Time: " + Mathf.Floor(gameManagerScript.time);
    }

    // Variables
    public Text timeText;
    public Text scoreText;
    public Text healthText;
    public GameManagerScript gameManagerScript;
}
