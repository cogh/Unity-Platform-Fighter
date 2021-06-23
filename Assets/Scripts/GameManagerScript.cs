using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManagerScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Set upgrade stuff
        PlayerPrefs.SetString("SpriteSet", "Normal");

        // Dont destroy on load
        DontDestroyOnLoad(transform.gameObject);

        // Time
        StartRoom();

        // Health
        health = maxHealth;
    }

    void StartRoom()
    {
        // Time set
        time = maxTime;
        score = 0;
    }

    public void DecreaseHealth()
    {
        health--;
        if (health <= 0)
        {
            health = maxHealth;
            if (PlayerPrefs.GetString("SpriteSet") == "Crown")
            {
                health = maxHealth * 2;
            }
            totalScore = 0;
            SceneManager.LoadScene("MainMenu");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Change time
        if (SceneManager.GetActiveScene().name.Contains("Level_"))
        {
            time -= Time.deltaTime;
        }

        // End room
        if (time < 0.0f || Input.GetKeyDown(KeyCode.P))
        {
            // Set score
            totalScore += score;
            money += score / 10;

            // Get level
            levelIndex++;
            
            // Go to level
            if (levelIndex < levelCount)
            {
                string levelName = "Level_" + levelIndex.ToString();
                SceneManager.LoadScene(levelName);
                StartRoom();
            }
            else
            {
                // Reset health
                health = maxHealth;
                if (PlayerPrefs.GetString("SpriteSet") == "Crown")
                {
                    health = maxHealth * 2;
                }
                // Go to main menu
                levelIndex = 0;
                SceneManager.LoadScene("EndScene");
                StartRoom();
            }
        }
    }

    // Levels
    int levelCount = 3;
    int levelIndex = 0;

    // Health
    public int maxHealth;
    public int health;

    // Money
    public int money;

    // Score
    public int score;
    public int totalScore;

    // Time
    public float maxTime;
    public float time;
}
