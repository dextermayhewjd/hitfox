using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Cinemachine;

public class PlayerMovement : MonoBehaviour, ICatchable
{
    // define the speed of an object
    private CharacterController controller;
    private Animator animator;

    [SerializeField] private float moveSpeed;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float sprintMultiplier;
    [SerializeField] private float rotationSpeed;

    // Jumping / Falling
    // To account for charging up the jump animiation.
    [SerializeField] private float jumpDelay;
    [SerializeField] private float jumpSpeed;
    [SerializeField] private float jumpGracePeriod;
    private float? lastGroundedTime;
    private float? jumpedTime;
    private float ySpeed;
    private float stepOffset;
    private bool isJumping;
    private bool isFalling;
    private bool isGrounded;

    private Vector3 moveDirection;
    private Vector3 velocity;

    public bool captured = false;
    public CinemachineFreeLook cam;
    public Transform cameraTransform;
    [SerializeField] AudioSource jumpSFX;
    public PhotonView view;
    public static bool onground = false;
    public bool driving = false;
    public GameObject cage;
    public GameObject footstep;
    

    public void Catch() {
        captured = true;
        // spawn a cage around fox
        Vector3 cagePosition = new Vector3(transform.position.x , transform.position.y - 0.5f, transform.position.z);
        // Debug.Log(cagePosition);
        Instantiate(cage, cagePosition, Quaternion.identity);
        // FoxPlayer.GetComponent<PhotonView>().Owner.NickName();
        // playerCage.tag = "MyCage";
    }

    void Start()
    {
        // Character Controller.
        controller = GetComponent<CharacterController>();

        // Fox Animator Controller.
        animator = GetComponentInChildren<Animator>();

        footstep.SetActive(false);
        captured = false;
        stepOffset = controller.stepOffset;
        Cursor.lockState = CursorLockMode.Locked;
        view = GetComponent<PhotonView>();
        if (view.IsMine)
        {
            cameraTransform = Camera.main.transform;
            cam = FindObjectOfType<CinemachineFreeLook>();
            cam.LookAt = transform;
            cam.Follow = transform;
        }
    }

    public void Teleport(Vector3 position, Quaternion rotation)
    {
        transform.position = position;
        Physics.SyncTransforms();
        transform.rotation = rotation;
        cameraTransform.rotation = rotation;
    }

    private bool locked = true;

    // Update is called once per frame
    void Update()
    {
        if (view.IsMine)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                if (!captured)
                {
                    Catch();
                } 
                else if (captured)
                {
                    captured = false;
                }   
            }

            // temporary cursor unlock 
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                locked = locked;
                
            } 
            if (locked) {
                Cursor.lockState = CursorLockMode.Locked;
            } else {
                Cursor.lockState = CursorLockMode.None;
            }

            if (!captured && !driving)
            {
                Movement();
            }
        }
        // fox captured
        if (captured){
            //
        }
        //added for footsteps
    }

    void Movement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        moveDirection = new Vector3(horizontalInput, 0, verticalInput);

        // Stop moving when in the air.
        // At the same time need to allow for momemntum when in the air.
        // Need to fix collider first and improve how grounding works.
        // if (controller.isGrounded)
        // {
            if (moveDirection != Vector3.zero)
            {
                animator.SetBool("isMoving", true);

                if (Input.GetButton("Sprint"))
                {
                    Run();
                }
                else
                {

                    Walk();
                }
            }
            else
            {
                Idle();
            }
        // }

        moveDirection = Quaternion.AngleAxis(cameraTransform.rotation.eulerAngles.y, Vector3.up) * moveDirection;
        moveDirection.Normalize();

        // Jumping and Falling
        // Gravity being slow. Need to do some playing around with values or change how things are done.
        ySpeed += Physics.gravity.y * Time.deltaTime;

        //so footsteps SFX don't play when in air (share with Footsteps.cs)
        if (controller.isGrounded) {
            onground = true;
            lastGroundedTime = Time.time;
        } else {
            onground = false;
        }

        if (Input.GetButtonDown("Jump"))
        {
            jumpedTime = Time.time;
            jumpSFX.Play();
        }

        if (Time.time - lastGroundedTime <= jumpGracePeriod) {
            controller.stepOffset = stepOffset;
            ySpeed = -0.5f;

            animator.SetBool("isGrounded", true);
            isGrounded = true;
            animator.SetBool("isJumping", false);
            isJumping = false;
            animator.SetBool("isFalling", false);

            if (Time.time - jumpedTime <= jumpGracePeriod)
            {
                // Delay before jumping.
                if (Time.time - jumpedTime >= jumpDelay)
                {
                    ySpeed = jumpSpeed;
                    jumpedTime = null;
                    lastGroundedTime = null;
                }
                animator.SetBool("isJumping", true);
                isJumping = true;
            }
        } else
        {
            controller.stepOffset = 0.0f;
            animator.SetBool("isGrounded", false);
            isGrounded = false;

            if ((isJumping && ySpeed < 0) || ySpeed < -2)
            {
                animator.SetBool("isFalling", true);
            }
        }

        // Need to consider rotating with respect to a slope.
        // Which might fix gravity keeping up with the movement speed.
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);

        velocity = moveDirection * moveSpeed;
        velocity.y = ySpeed;
        controller.Move(velocity * Time.deltaTime);

        // if player is moving, rotate towards the direction of the movement
        if (moveDirection != Vector3.zero)
        {
            Quaternion rotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void Idle()
    {
        animator.SetFloat("Speed", 0, 0.1f, Time.deltaTime);
        animator.SetBool("isMoving", false);
        StopFootsteps();
    } 

    private void Walk()
    {
        moveSpeed = walkSpeed;
        animator.SetFloat("Speed", 0.5f, 0.1f, Time.deltaTime);
        // Need to sync with animation and move speed. Or replace how this is played.
        Footsteps();
    }

    private void Run()
    {
        moveSpeed = walkSpeed * sprintMultiplier;
        animator.SetFloat("Speed", 1, 0.1f, Time.deltaTime);
        // Need to speed up to sync with animation and move speed. Or replace how this is played.
        Footsteps();
    }

    private void Footsteps()
    {
        footstep.SetActive(true);
    }

    private void StopFootsteps()
    {
        footstep.SetActive(false);
    }

    private void OnApplicationFocus(bool focusStatus) {
        if (focusStatus) {
            Cursor.lockState = CursorLockMode.Locked;
        } else {
            Cursor.lockState = CursorLockMode.None;
        }
        
    }
}
