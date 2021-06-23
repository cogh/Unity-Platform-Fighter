using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        state = "sink";

        spriteIndex = 0;
        spriteIndexFloat = 0;
        spriteCount = sprites.Length;

        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        // Check player distance
        playerDistance = Vector2.Distance(transform.position, player.transform.position);
        playerClose = (playerDistance < minimumPlayerDistance);

        // Run states
        if (state == "sink")
        {
            // Sprite
            spriteIndexFloat -= Time.deltaTime * spritesPerSecond;
            if (spriteIndexFloat < 0) { spriteIndexFloat = 0; }
            spriteIndex = (int)Mathf.Floor(spriteIndexFloat);
            spriteRenderer.sprite = sprites[spriteIndex];

            // Transition
            if (playerClose)
            {
                state = "rise";
            }
        }
        else if (state == "rise")
        {
            // Sprite
            spriteIndexFloat += Time.deltaTime * spritesPerSecond;
            if (spriteIndexFloat > spriteCount) { spriteIndexFloat = spriteCount-1; }
            spriteIndex = (int)Mathf.Floor(spriteIndexFloat);
            spriteRenderer.sprite = sprites[spriteIndex];

            // Transition
            if (!playerClose)
            {
                state = "sink";
            }
        }
    }

    // Internal references
    SpriteRenderer spriteRenderer;

    // External references
    public Sprite[] sprites;
    public GameObject player;

    // AI
    public float minimumPlayerDistance;
    public float playerDistance;
    public bool playerClose;

    // State
    public string state;

    // Sprite handling
    float spriteIndexFloat;
    int spriteIndex;
    int spriteCount;
    float spriteSpeed;
    public int spritesPerSecond;
    
}
