using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Cinemachine;


public class PlayerMovement : MonoBehaviourPun, ICatchable
{
    // Main UI Controller.
    private GameObject uiControllerObj;
    private UIController uiController;
    private InputController inputController;

    // define the speed of an object
    private CharacterController controller;
    private Animator animator;

    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float sprintMultiplier;

    [Header("Camera")]
    [SerializeField] private float rotationSpeed;

    [Header("Jumping")]
    [SerializeField] private float jumpDelay;
    [SerializeField] private float jumpHeight;
    [SerializeField] private float gravityMultiplier;
    [SerializeField] private float jumpGracePeriod;
    private float? lastGroundedTime;
    private float? jumpedTime;
    private float ySpeed;
    private float stepOffset;
    private bool isJumping;
    private bool isFalling;
    private bool isGrounded;

    [Header("Audio")]
    [SerializeField] private AudioSource walkSFX;
    [SerializeField] private AudioSource runSFX;
    [SerializeField] private AudioSource jumpSFX;

    private Vector3 moveDirection;
    private Vector3 velocity;

    public bool captured;
    public bool hidden;
    public CinemachineFreeLook cam;
    public Transform cameraTransform;
    public PhotonView view;
    public static bool onground = false;
    public bool driving = false;
    public GameObject cage;
    public GameObject footstep;
    

    public void Catch()
    {
        Vector3 cagePosition = new Vector3(transform.position.x , transform.position.y - 0.5f, transform.position.z);
        GameObject newCage = PhotonNetwork.Instantiate(cage.name, cagePosition, Quaternion.identity);
        this.photonView.RPC("RPC_Catch", RpcTarget.AllBuffered, view.ViewID, newCage.GetComponent<PhotonView>().ViewID);
    }

    [PunRPC]
    void RPC_Catch(int playerID, int cageID) 
    {
        PhotonView player = PhotonView.Find(playerID);
        player.GetComponent<PlayerMovement>().captured = true;
        player.gameObject.transform.Find("Collider").gameObject.SetActive(false);
        PhotonView.Find(cageID).GetComponent<CageScript>().ownerId = playerID;
    }

    void Start()
    {
        // Character Controller.
        controller = GetComponent<CharacterController>();

        // Fox Animator Controller.
        animator = GetComponentInChildren<Animator>();

        // UI controller.
        uiControllerObj = GameObject.Find("UIController");
        uiController = uiControllerObj.GetComponent<UIController>();

        // Input controller.
        inputController = GameObject.Find("InputController").GetComponent<InputController>();

        footstep.SetActive(false);
        captured = false;
        hidden = false;
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
            // manual capture only for testing
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

            if (!driving)
            {
                Movement();
            }
            else
            {
                Idle();
            }
        }
    }

    void Movement()
    {
        float horizontalInput = inputController.GetInputAxis("Horizontal");
        float verticalInput = inputController.GetInputAxis("Vertical");

        moveDirection = new Vector3(horizontalInput, 0, verticalInput);
        // float magnitude = Mathf.Clamp01(moveDirection.magnitude) * moveSpeed;
        // moveDirection.Normalize();

        // Stop moving when in the air.
        // At the same time need to allow for momemntum when in the air.
        // Need to fix collider first and improve how grounding works.
        if (isGrounded)
        {
            if (moveDirection != Vector3.zero)
            {
                animator.SetBool("isMoving", true);

                if (inputController.GetInput("Sprint"))
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

        }

        moveDirection = Quaternion.AngleAxis(cameraTransform.rotation.eulerAngles.y, Vector3.up) * moveDirection;
        moveDirection.Normalize();

        // Jumping and Falling
        float gravity = Physics.gravity.y * gravityMultiplier;
        if (isJumping && ySpeed > 0 && inputController.GetInput("Jump") == false)
        {
            gravity *= 1.5f;
        }
        ySpeed += gravity * Time.deltaTime;

        //so footsteps SFX don't play when in air (share with Footsteps.cs)
        if (controller.isGrounded) {
            onground = true;
            lastGroundedTime = Time.time;
            if (inputController.GetInputDown("Jump"))
            {
                jumpedTime = Time.time;
                walkSFX.enabled = false;
                runSFX.enabled = false;
            }
        } else {
            onground = false;
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
                    jumpSFX.Play();
                    ySpeed = Mathf.Sqrt(jumpHeight * -3 * gravity);
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
            walkSFX.enabled = false;
            runSFX.enabled = false;

            if ((isJumping && ySpeed < 0) || ySpeed < -2)
            {
                animator.SetBool("isFalling", true);
            }
        }

        // transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);

        velocity = moveDirection * moveSpeed;
        velocity = AdjustVelocityToSlope(velocity);
        velocity.y += ySpeed;
        controller.Move(velocity * Time.deltaTime);

        // if player is moving, rotate towards the direction of the movement
        if (moveDirection != Vector3.zero)
        {
            Quaternion rotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
        }
    }

    private Vector3 AdjustVelocityToSlope(Vector3 velocity)
    {
        var ray = new Ray(transform.position, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hitInfo, 0.2f))
        {
            var slopeRotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);
            var adjustedVelocity = slopeRotation * velocity;

            if (adjustedVelocity.y < 0)
            {
                return adjustedVelocity;
            }
        }

        return velocity;
    }

    private void Idle()
    {
        animator.SetFloat("Speed", 0, 0.1f, Time.deltaTime);
        animator.SetBool("isMoving", false);
        walkSFX.enabled = false;
        runSFX.enabled = false;
    } 

    private void Walk() {
        moveSpeed = walkSpeed;
        animator.SetFloat("Speed", 0.5f, 0.1f, Time.deltaTime);
        walkSFX.enabled = true;
        runSFX.enabled = false;
    }

    private void Run()
    {
        moveSpeed = walkSpeed * sprintMultiplier;
        animator.SetFloat("Speed", 1, 0.1f, Time.deltaTime);
        walkSFX.enabled = false;
        runSFX.enabled = true;
    }
}
