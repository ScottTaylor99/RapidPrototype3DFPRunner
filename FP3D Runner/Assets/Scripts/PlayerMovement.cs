using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
public class PlayerMovement : MonoBehaviour
{
    //Turned out to be more then just a PlayerMovement Script

    //Movement
    float playerHeight = 2f;

    [SerializeField] Transform orientation;

    public float moveSpeed = 6f;
    public float airMultiplier = 0.4f;
    float movementMultiplier = 10f;

    public float walkSpeed = 4f;
    public float sprintSpeed = 6f;
    public float acceleration = 10f;

    public float jumpForce = 5f;

    [SerializeField] KeyCode jumpKey = KeyCode.Space;
    [SerializeField] KeyCode sprintKey = KeyCode.LeftShift;

    public float groundDrag = 6f;
    public float airDrag = 2f;

    float horizontalMovement;
    float verticalMovement;

    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundMask;
    [SerializeField] float groundDistance = 0.2f;
    public bool isGrounded { get; private set; }

    Vector3 moveDirection;
    Vector3 slopeMoveDirection;

    Rigidbody rb;

    RaycastHit slopeHit;

    public GameObject particles;

    //SlideCode
    CapsuleCollider collider;

    float originalHeight;
    public float reduceHeight;
    public float slideSpeed;
    bool isSliding;

    [SerializeField] KeyCode slidekey = KeyCode.LeftControl;

    //Dash 
    public float dashSpeed;
    bool isDashing;
    public float dashCD = 5; //Ability Cooldown 
    private float nextDash = 0; //Next time we can use the Ability
    public TMP_Text dashCooldown;
    float cdTimer = 6;


    //Tutorial 
    public TMP_Text Tutorial;
    public GameObject tutorials;

    //SurfCode
    bool isSurfing;
    public float surfSpeed = 8;

    //Timer
    public float clock;
    public TMP_Text timer;
    bool gamePaused = false;

    public GameObject thanks;
    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight / 2 + 0.5f))
        {
            if (slopeHit.normal != Vector3.up)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        //Slide
        collider = GetComponent<CapsuleCollider>();
        originalHeight = collider.height;

    }

    private void FixedUpdate()
    {
        MovePlayer();

        if (isDashing)
            Dashing();
    }

    private void Update()
    {


        //Timer
        if (!gamePaused)
        {
            clock += Time.deltaTime;
            timer.text = clock.ToString("F2");
        }
        //Game ShortCuts

        //Restart

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (SceneManager.GetActiveScene().name == "SampleScene")
            {
                SceneManager.LoadScene("SampleScene");
            }
            if (SceneManager.GetActiveScene().name == "Level1")
            {
                SceneManager.LoadScene("Level1");
            }
            if(SceneManager.GetActiveScene().name == "Level0")
            {
                SceneManager.LoadScene("Level0");
            }
            if (SceneManager.GetActiveScene().name == "Level1.1")
            {
                SceneManager.LoadScene("Level1.1");
            }
        }
        //Menu
        if(Input.GetKeyDown(KeyCode.M))
        {
            SceneManager.LoadScene("StartScreen");
        }

        //Slide
        if (Input.GetKeyDown(slidekey) && isGrounded) //KeyBoard Input and checking if the player is grounded before sliding
        {
            Sliding();
        }
        else if (Input.GetKeyUp(slidekey))
            GotUp();

        //Dash
        if (Time.time > nextDash)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                isDashing = true;
                nextDash = Time.time + dashCD;
                cdTimer = dashCD;
                
            }
        }
        if (cdTimer <= dashCD)
        {
            if (cdTimer >= 0)
            {
                cdTimer -= Time.deltaTime;
                dashCooldown.text = cdTimer.ToString("F1");
            }
        }

        //print(cdTimer);

        if (cdTimer <= 0)
        {
            dashCooldown.text = "E";
        }

        //Movement
        if(Input.GetKey(sprintKey))
        {
            particles.SetActive(true);
        }
        else
        {
            particles.SetActive(false);
        }

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        MyInput();
        ControlDrag();
        ControlSpeed();

        if (Input.GetKeyDown(jumpKey) && isGrounded)
        {
            Jump();
        }

        slopeMoveDirection = Vector3.ProjectOnPlane(moveDirection, slopeHit.normal);
    }

    //SlidingFunctions
    private void Sliding()
    {
        collider.height = reduceHeight;
        rb.AddForce(transform.forward * slideSpeed, ForceMode.VelocityChange);
    }

    private void GotUp()
    {
        collider.height = originalHeight;
    }

    //DashingFunctions
    private void Dashing()
    {
        rb.AddForce(transform.forward * dashSpeed, ForceMode.Impulse);
        isDashing = false;
    }

    //MovementFunctions
    void MyInput()
    {
        horizontalMovement = Input.GetAxisRaw("Horizontal");
        verticalMovement = Input.GetAxisRaw("Vertical");

        moveDirection = orientation.forward * verticalMovement + orientation.right * horizontalMovement;
    }

    //SurfFunctions
    void Surfing()
    {
        rb.AddForce(transform.forward * surfSpeed, ForceMode.Acceleration);
    }

    private void OnCollisionStay(Collision other)
    {
        //When surfing increase speed
        if (other.gameObject.tag == "SurfPlatform")
        {
            isSurfing = true;
            if(isSurfing == true)
            {
                Surfing();
                surfSpeed += 1;
                
            }
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        //Increase Speed when wall running
        if (other.gameObject.tag == "Wall")
        {
            walkSpeed += 12;
            moveSpeed += 16;
            sprintSpeed += 16;
        }
    }
    private void OnCollisionExit(Collision other)
    {
        //Reset back to original numbers when exiting wallrun or surf
        if (other.gameObject.tag == "SurfPlatform")
        {
            isSurfing = false;
            surfSpeed = 8;
        }

        if (other.gameObject.tag == "Wall")
        {
            walkSpeed = 6;
            moveSpeed = 8;
            sprintSpeed = 8;
        }
    }

    void Jump()
    {
        if (isGrounded)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }
    }

    void ControlSpeed()
    {
        if (Input.GetKey(sprintKey) && isGrounded)
        {
            moveSpeed = Mathf.Lerp(moveSpeed, sprintSpeed, acceleration * Time.deltaTime);
        }
        else
        {
            moveSpeed = Mathf.Lerp(moveSpeed, walkSpeed, acceleration * Time.deltaTime);
        }
    }

    void ControlDrag()
    {
        if (isGrounded)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = airDrag;
        }
    }

    void MovePlayer()
    {
        if (isGrounded && !OnSlope())
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * movementMultiplier, ForceMode.Acceleration);
        }
        else if (isGrounded && OnSlope())
        {
            rb.AddForce(slopeMoveDirection.normalized * moveSpeed * movementMultiplier, ForceMode.Acceleration);
        }
        else if (!isGrounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * movementMultiplier * airMultiplier, ForceMode.Acceleration);
        }
    }

    //Tutorial

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "SlideTutorial")
        {
            tutorials.SetActive(true);
            Tutorial.text = "Left Ctrl to Slide";
        }

        if (other.gameObject.name == "WallJumpTutorial")
        {
            tutorials.SetActive(true);
            Tutorial.text = "Run and Jump to Wall Run, Press Space to Jump off";
        }

        if (other.gameObject.name == "WallClimbTutorial")
        {
            tutorials.SetActive(true);
            Tutorial.text = "Hold A or D relative to the wall to latch on";
        }

        if (other.gameObject.name == "Finish Tutorial")
        {
            tutorials.SetActive(true);
            Tutorial.text = "Tutorial Finish, Finished in: " + clock.ToString("F2");
            StartCoroutine("loadNextLevel");
            gamePaused = true;
        }
        if (other.gameObject.name == "RestartTutorial")
        {
            tutorials.SetActive(true);
            Tutorial.text = "Press R, to Restart from last Checkpoint";
        }
        if (other.gameObject.name == "LevelOne.One")
        {
            tutorials.SetActive(true);
            Tutorial.text = "Level 1.1 Finished, Finished in: " + clock.ToString();
            gamePaused = true;
            thanks.SetActive(true);
            StartCoroutine("loadNextLevel");
        }
        if (other.gameObject.name == "LevelZero")
        {
            tutorials.SetActive(true);  
            Tutorial.text = "Level 0 Finished, Finished in: " + clock.ToString();
            gamePaused = true;
            StartCoroutine("loadNextLevel");
        }
        if (other.gameObject.name == "LevelOne")
        {
            tutorials.SetActive(true);
            Tutorial.text = "Level 1 Finished, Finished in: " + clock.ToString();
            gamePaused = true;
            StartCoroutine("loadNextLevel");
        }
    }

    public void OnTriggerExit(Collider other)
    {
        tutorials.SetActive(false);
    }

    IEnumerator loadNextLevel()
    {
       yield return new WaitForSeconds(3);
        if (SceneManager.GetActiveScene().name == "SampleScene")
        {
            SceneManager.LoadScene("StartScreen");
        }
        if (SceneManager.GetActiveScene().name == "Level0")
        {
            SceneManager.LoadScene("Level1");
        }
        if (SceneManager.GetActiveScene().name == "Level1")
        {
            SceneManager.LoadScene("Level1.1");
        }
        if (SceneManager.GetActiveScene().name == "Level1.1")
        {
            SceneManager.LoadScene("StartScreen");
        }
    }
}
