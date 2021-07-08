using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameManagerScript gameManagerScript = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
        storeText.text = "Money: " + gameManagerScript.money;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddMoney()
    {
        GameManagerScript gameManagerScript = GameObject.Find("GameManager").GetComponent<GameManagerScript>();

        gameManagerScript.money += 5;
    }

    public void BuyCrown()
    {
        // Get game manager script
        GameManagerScript gameManagerScript = GameObject.Find("GameManager").GetComponent<GameManagerScript>();

        // Check money
        if (gameManagerScript.money >= 5)
        {
            // Increase health
            gameManagerScript.health = gameManagerScript.maxHealth * 2;

            // Decrease money
            gameManagerScript.money -= 5;

            // Store text
            storeText.text = "Money: " + gameManagerScript.money + "\n" + "Crown bought!";

            // Set sprite preference
            PlayerPrefs.SetString("SpriteSet", "Crown");
        }
    }

    // Variables
    public Text storeText;
}
