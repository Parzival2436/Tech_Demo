using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;
using TMPro;
using System.Timers;

public class PlayerController : MonoBehaviour
{

    //Text
    public TextMeshProUGUI lifeText;
    public TextMeshProUGUI countText;
    public GameObject winTextObject;
    public GameObject controlText;
    public GameObject checkpointTextObject;

    //Hud Variables
    private float life;
    private int lifeDisplay;
    private int count;
    public int pickupTotal;
    private bool ctrlText;

    //Movement
    public CharacterController controller;
    private float turnTime = 0.1f;
    private float turnVelocity = 3;
    public Transform cam;
    public float speed = 20f;
    public float gravity = -20f;
    public Transform groundCheck;
    private float groundDistance = 0.5f;
    public LayerMask groundMask;
    Vector3 velocity;

    //Spawning
    private Vector3 levelSpawn;
    private Vector3 respawnPosition;

    //Jump
    public int jumpStrength = 10;
    public int doubleJumpStrength = 5;
    public bool  doubleJumpCheck = false;
    private bool grounded = true;
    public float jumpCooldown;

    //Dash
    public int dashSpeed = 5;
    public float dashDelay = 1;
    public float dashTime = 0.15f;
    private int dDashCheck = 0;
    private int aDashCheck = 0;
    private int wDashCheck = 0;
    private int sDashCheck = 0;
    int DashLimit = 1;

    //Initialize
    void Start()
    {
        Cursor.visible = false;
        life = 100;
        count = 0;
        SetLifeText();
        SetCountText();
        winTextObject.SetActive(false);
        checkpointTextObject.SetActive(false);

        controlText.SetActive(true);
        ctrlText = true;

        //initialize position
        levelSpawn = transform.position;
        respawnPosition = levelSpawn;
    }

    void Update()
    {
        
        //Win Conditions
        if (count >= pickupTotal)
        {
            winTextObject.SetActive(true);
        }

        //Toggle Tutorial Text
        if (Input.GetKeyDown("x"))
        {
            if (ctrlText)
            {
             controlText.SetActive(false);
                ctrlText = false;
            }
            else
            {
            controlText.SetActive(true);
                ctrlText = true;

            }
        }

        if (Input.GetKey(KeyCode.D)) //Dash Right
            {
              if (Input.GetKeyDown(KeyCode.LeftShift))
              {
                if (dDashCheck < DashLimit)
                {
                    StartCoroutine(Dash());
                    dDashCheck = dDashCheck + 1;
                }
              }
            }

            if (Input.GetKey(KeyCode.A)) //Dash Left
            {
              if (Input.GetKeyDown(KeyCode.LeftShift))
              {
                if (aDashCheck < DashLimit)
                {
                    StartCoroutine(Dash());
                    aDashCheck = aDashCheck + 1;
                }
              }
            }

            if (Input.GetKey(KeyCode.S)) //Dash Back
            {
              if (Input.GetKeyDown(KeyCode.LeftShift))
              {
                if (sDashCheck < DashLimit)
                {
                    StartCoroutine(Dash());
                    sDashCheck = sDashCheck + 1;
                }
              }
            }

            if (Input.GetKey(KeyCode.W)) //Dash Forward
            {

              if (Input.GetKeyDown(KeyCode.LeftShift))
              {
                if (wDashCheck < DashLimit)
                {
                    StartCoroutine(Dash());
                    wDashCheck = wDashCheck + 1;
                }
              }

            }
       

        //groundcheck
        grounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if(grounded && velocity.y < 0)
        {
            velocity.y = -3f;
        }

        //Gravity
        velocity.y += gravity * Time.deltaTime;

        //Movement
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf. Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnVelocity, turnTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
           controller.Move(moveDir.normalized * speed * Time.deltaTime);
        }

        //Convert Life Float to Int & Display
        lifeDisplay = (int) life;
        SetLifeText();

        //If Grounded
        if (grounded == true)
        {
            doubleJumpCheck = false;
            jumpCooldown = 0;

            //Jump
            if (Input.GetKeyDown(KeyCode.Space))
            {
                velocity.y = velocity.y + jumpStrength;


            }
            //Jump Limit
            doubleJumpCheck = false;

            //Airdash Limit
            dDashCheck = 0;
            aDashCheck = 0;
            wDashCheck = 0;
            sDashCheck = 0;
        }
        else
        {
            jumpCooldown = jumpCooldown + (1 * Time.deltaTime);

            if (doubleJumpCheck == false && Input.GetKeyDown(KeyCode.Space))
            {

                if (jumpCooldown >= 0.3f)
                {
                    if (doubleJumpCheck == false && Input.GetKeyDown(KeyCode.Space))
                    {
                        velocity.y = 0;
                        velocity.y = velocity.y + doubleJumpStrength;
                        doubleJumpCheck = true;
                    }
                }

            }
        }
            controller.Move(velocity * Time.deltaTime);
    }

    IEnumerator Dash()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;
        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
        Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        float startTime = Time.time;

        while(Time.time < startTime + dashTime)
        {
            controller.Move(moveDir * dashSpeed * Time.deltaTime);

            yield return null;
        }
        
    }
    void SetLifeText()
    {
        lifeText.text = "Life: " + lifeDisplay.ToString();

        //Respawn
        if(life <= 0)
        {   
            gameObject.transform.position = new Vector3(0.0f, 2.0f, 0.0f);
            life = 100;
            lifeText.text = "Life: " + lifeDisplay.ToString();
        }
    }

    void SetCountText()
    {
        countText.text = "Count: " + count.ToString();
    }

    private void OnTriggerEnter(Collider other)
    {
        //GameObject Interaction

        if (other.gameObject.CompareTag("PickUp"))
        {
            other.gameObject.SetActive(false);
            count = (count + 1);
            SetCountText();
        }


        //if (other.gameObject.CompareTag("Spike"))
        //{
        //    life = life - 20 ;
        //    SetLifeText();
        //}

        if (other.gameObject.CompareTag("Killbox"))
        {
            RespawnPlayer();
        }

        if (other.gameObject.CompareTag("Checkpoint"))
        {
            respawnPosition = transform.position;
            //checkpointTextObject.SetActive(true);
            //TextTimer(checkpointTextObject, 5.0f);
            other.gameObject.SetActive(false);
        }

        //if (other.gameObject.CompareTag("Lava"))
        //{
        //    life = life - 10;
        //    SetLifeText();
        //}

    }


    void RespawnPlayer()
    {
        transform.position = respawnPosition;
        life = 100;
        lifeText.text = "life: " + lifeDisplay.ToString();
    }

    public void texttimer(GameObject text, float targetTime)
    {
        Console.WriteLine("timer started");
        //float initTime = Time.time;
        float timepassed = 0;
        timepassed += Time.deltaTime;
        Console.WriteLine(text.name + timepassed);

        if (timepassed == targetTime)
        {
            text.SetActive(false);
        }
    }

}

