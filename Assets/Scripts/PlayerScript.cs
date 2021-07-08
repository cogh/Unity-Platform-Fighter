using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class SpriteSet
{
    public Sprite[] idleSprites;
    public Sprite[] runSprites;
    public Sprite[] airSprites;
    public Sprite[] attackSprites;
    public Sprite[] aerialSprites;
}


public class PlayerScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Sprite
        spriteRenderer = GetComponent<SpriteRenderer>();
        string current = PlayerPrefs.GetString("SpriteSet");
        if (PlayerPrefs.GetString("SpriteSet") == "Crown")
        {
            currentSpriteSet = spriteSetCrown;
        }
        else
        {
            currentSpriteSet = spriteSetNormal;
        }
        currentSprites = currentSpriteSet.idleSprites;
        spriteIndex = 0;

        // Collision
        physicsCollider = GetComponent<BoxCollider2D>();

        // Calculate forces
        gravityForce = new Vector3(0.0f, -gravityStrength, 0.0f);

        // Game manager script
        gameManager = GameObject.Find("GameManager");
        gameManagerScript = gameManager.GetComponent<GameManagerScript>();

        // Physics inference
        groundDrag      = 1.0f - Mathf.Pow((runSpeedMin / runSpeedMax), (1 / runStopTime));
        airDrag         = 1.0f - Mathf.Pow((runSpeedMin / runSpeedMax), (1 / driftStopTime)); // but maybe this is wrong cause the number is too big
        runSpeed        = runSpeedMax * groundDrag; // think these are wrong
        driftSpeed      = runSpeedMax * airDrag;
        gravityStrength = gravityStrength = ((2 * jumpHeight) / (jumpTime*jumpTime));
        jumpSpeed       = Mathf.Sqrt(2 * jumpHeight * gravityStrength);

        // Sprite
        spriteSpeed /= ticksPerSecond;
    }

    private void Update()
    {
        // Get input
        GetInput();

        // Tick timer
        tickTimer += Time.deltaTime;
        if (tickTimer >= 1.0f / ticksPerSecond)
        {
            RunTick();
            tickTimer = 0;

            // Reset inputs
            inputJump = false;
            inputAttack = false;
        }
    }

    void RunTick()
    {
        // Attack timer
        if (inputAttack)
        {
            if (grounded) { currentSprites = currentSpriteSet.attackSprites; }
            else          { currentSprites = currentSpriteSet.aerialSprites; }
            spriteIndex = 0;
            state = "attack";
        }
        if (state == "attack" && spriteIndex + (spriteSpeed) > currentSprites.Length)
        {
            currentSprites = currentSpriteSet.idleSprites;
            spriteIndex = 0;
            state = "idle";
        }

        // End attack on landing
        if (state == "attack" && landed)
        {
            currentSprites = currentSpriteSet.idleSprites;
            spriteIndex = 0;
            state = "idle";
        }
        landed = false;

        // Sprite selection
        if (state == "attack")
        {
            if (grounded)
            {
                currentSprites = currentSpriteSet.attackSprites;
            }
            else
            {
                currentSprites = currentSpriteSet.aerialSprites;
            }
        }
        else
        {
            if (grounded)
            {
                if (Mathf.Abs(velocity.x) > runSpeedMin)
                {
                    currentSprites = currentSpriteSet.runSprites;
                }
                else
                {
                    currentSprites = currentSpriteSet.idleSprites;
                }
            }
            else
            {
                currentSprites = currentSpriteSet.airSprites;
            }
        }

        // Sprite index wrap
        if (spriteIndex >= currentSprites.Length)
        {
            spriteIndex = 0;
        }

        // Sprite flip
        if (velocity.x < 0)
        {
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.flipX = false;
        }

        // Sprite management
        int spriteIndexFloored = (int)Mathf.Floor(spriteIndex);
        spriteRenderer.sprite = currentSprites[spriteIndexFloored];
        spriteIndex += spriteSpeed;
        if (spriteIndex >= currentSprites.Length)
        {
            spriteIndex = 0;
        }

        // Set grounded
        if (PlaceMeeting(transform.position + Vector3.down * overlapEpsilon, "solid"))
        {
            if (!grounded)
            {
                landed = true;
            }
            grounded = true;
        }
        else
        {
            grounded = false;
        }

        // Drag
        if (grounded) { velocity.x -= velocity.x * groundDrag; }
        else { velocity.x -= velocity.x * airDrag; }

        // Set movespeed
        if (grounded) { moveSpeed = runSpeed; }
        else { moveSpeed = driftSpeed; }

        // Apply horizontal movement
        if (inputMoveRight)
        {
            velocity += Vector3.right * moveSpeed;
        }
        if (inputMoveLeft)
        {
            velocity += Vector3.left * moveSpeed;
        }

        // Apply jumping
        if (inputJump && grounded == true)
        {
            velocity += Vector3.up * jumpSpeed;
            jumpSound.Play();
        }

        // Gravity
        velocity += Vector3.down * gravityStrength;

        // Clamp fallspeed
        if (velocity.y < -maxFallSpeed)
        {
            velocity.y = -maxFallSpeed;
        }

        // Final movement vectors
        Vector3 xVector = new Vector3(velocity.x, 0.0f, 0.0f);
        Vector3 yVector = new Vector3(0.0f, velocity.y, 0.0f);

        // Collide velocity x
        if (PlaceMeeting(transform.position + xVector, "solid"))
        {
            Vector3 xVectorEpsilon = xVector.normalized * overlapEpsilon;
            while (!PlaceMeeting(transform.position + xVectorEpsilon, "solid"))
            {
                transform.position += xVectorEpsilon;
            }
            xVector.x = 0.0f;
            velocity.x = 0.0f;
        }
        transform.position += xVector;

        // Collide velocity y
        if (PlaceMeeting(transform.position + yVector, "solid"))
        {
            Vector3 yVectorEpsilon = yVector.normalized * overlapEpsilon;
            while (!PlaceMeeting(transform.position + yVectorEpsilon, "solid"))
            {
                transform.position += yVectorEpsilon;
            }
            yVector.y = 0.0f;
            velocity.y = 0.0f;
        }
        transform.position += yVector;

        // Collide with targets
        GameObject target = InstancePlace(transform.position, "target");
        if (target != null && state == "attack")
        {
            gameManagerScript.score++;
            Destroy(target);
            targetSound.Play();
        }

        // Collide with enemies
        GameObject collidedEnemy = InstancePlace(transform.position, "enemy");
        if (collidedEnemy != null)
        {
            hurtParticles.Play();
            hurtSound.Play();
            gameManagerScript.DecreaseHealth();
            Destroy(collidedEnemy);
        }
    }

    void GetInput()
    {
        // Move inputs
        inputMoveRight = Input.GetKey(KeyCode.D);
        inputMoveLeft = Input.GetKey(KeyCode.A);

        // Dash inputs
        inputDashRight = Input.GetKeyDown(KeyCode.D);
        inputDashLeft = Input.GetKeyDown(KeyCode.A);

        // Jump input
        if (Input.GetKeyDown(KeyCode.Space)) { inputJump = true; }

        // Attack input
        if (Input.GetKeyDown(KeyCode.J)) 
        { 
            inputAttack = true; 
        }
    }

    private bool PlaceMeeting(Vector3 position, string tag)
    {
        // Get all overlapping colliders
        float x1 = position.x - physicsCollider.bounds.extents.x;
        float y1 = position.y - physicsCollider.bounds.extents.y;
        float x2 = position.x + physicsCollider.bounds.extents.x;
        float y2 = position.y + physicsCollider.bounds.extents.y;
        Vector2 p1 = new Vector2(x1, y1);
        Vector2 p2 = new Vector2(x2, y2);

        // Get overlapping colliders
        Collider2D[] overlappingColliders = Physics2D.OverlapAreaAll(p1,p2);

        // Check if any colliders are relevant
        bool overlap = false;
        foreach (Collider2D overlappingCollider in overlappingColliders)
        {
            if (overlappingCollider != null && overlappingCollider != physicsCollider && overlappingCollider.gameObject.tag == tag)
            {
                overlap = true;
            }
        }

        // Return
        return overlap;
    }

    private GameObject InstancePlace(Vector3 position, string tag)
    {
        // Get all overlapping colliders
        float x1 = position.x - physicsCollider.bounds.extents.x;
        float y1 = position.y - physicsCollider.bounds.extents.y;
        float x2 = position.x + physicsCollider.bounds.extents.x;
        float y2 = position.y + physicsCollider.bounds.extents.y;
        Vector2 p1 = new Vector2(x1, y1);
        Vector2 p2 = new Vector2(x2, y2);

        // Get overlapping colliders
        Collider2D[] overlappingColliders = Physics2D.OverlapAreaAll(p1, p2);

        // Check if any colliders are relevant
        GameObject overlappingObject = null;
        bool overlap = false;
        foreach (Collider2D overlappingCollider in overlappingColliders)
        {
            if (overlappingCollider != null && overlappingCollider != physicsCollider && overlappingCollider.gameObject.tag == tag)
            {
                overlap = true;
                overlappingObject = overlappingCollider.gameObject;
            }
        }

        // Return
        return overlappingObject;
    }

    // States
    public string state;

    // Sprite
    public Sprite[] currentSprites;
    public SpriteSet currentSpriteSet;
    public SpriteSet spriteSetNormal;
    public SpriteSet spriteSetCrown;

    public float spriteIndex;
    public float spriteSpeed;

    // Input
    bool inputMoveLeft;
    bool inputMoveRight;
    bool inputDashLeft;
    bool inputDashRight;
    bool inputJump;
    bool inputAttack;

    // Collision
    BoxCollider2D physicsCollider;
    bool grounded = false;
    bool landed = true;
    public float overlapEpsilon;

    // Force descriptors
    public float runSpeed;
    public float runSpeedMin;
    public float runSpeedMax;
    public float jumpHeight;
    public float jumpTime;
    public float runStopTime;
    public float maxFallSpeed;
    public float driftStopTime;

    // Static forces
    float moveSpeed;
    float dashSpeed;
    float driftSpeed;
    float jumpSpeed;
    float gravityStrength;
    Vector3 gravityForce;
    float airDrag;
    float groundDrag;

    // Dynamic forces
    public Vector3 velocity;
    public Vector3 controlForce;

    // Timing
    public float timer;
    public float attackTimer;
    public float tickTimer = 0;
    public float ticksPerSecond = 60;

    // Sounds
    public AudioSource jumpSound;
    public AudioSource targetSound;
    public AudioSource hurtSound;

    // Particle systems
    public ParticleSystem hurtParticles;

    // Misc
    public GameObject targetPrefab;
    public GameObject gameManager;
    GameManagerScript gameManagerScript;
    SpriteRenderer spriteRenderer;
}
