using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Cinemachine;

public class PlayerMovement : MonoBehaviour, ICatchable
{
    // define the speed of an object
    public float walkSpeed;
    public float rotationSpeed;
    public float jumpSpeed;
    private float jumpGracePeriod;
    public float sprintMultiplier;
    public bool captured = false;
    public CinemachineFreeLook cam;
    public Transform cameraTransform;
    private float verticalSpeed;
    private float stepOffset;
    private float? lastGroundedTime;
    private float? jumpedTime;
    private CharacterController characterController;
    [SerializeField] AudioSource jumpSFX;
    public PhotonView view;
    public static bool onground = false;
    public bool driving = false;
    public GameObject footstep;


    public void Catch() {
        captured = true;
    }

    void Start()
    {
        footstep.SetActive(false);
        captured = false;
        characterController = GetComponent<CharacterController>();
        stepOffset = characterController.stepOffset;
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

    void Movement()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal") * walkSpeed * Time.deltaTime;
        float verticalInput = Input.GetAxisRaw("Vertical") * walkSpeed * Time.deltaTime;

        Vector3 movementDirection = new Vector3(horizontalInput, 0, verticalInput);
        float inputMagnitude = Mathf.Clamp01(movementDirection.magnitude);

        if (Input.GetButton("Sprint")) {
            inputMagnitude *= sprintMultiplier;
        }

        float speed;
        speed = inputMagnitude * walkSpeed;


        movementDirection = Quaternion.AngleAxis(cameraTransform.rotation.eulerAngles.y, Vector3.up) * movementDirection;
        movementDirection.Normalize();

        verticalSpeed += Physics.gravity.y * Time.deltaTime;

        //so footsteps SFX don't play when in air (share with Footsteps.cs)
        if (characterController.isGrounded) {
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
            characterController.stepOffset = stepOffset;
            verticalSpeed = -0.5f;

            if (Time.time - jumpedTime <= jumpGracePeriod) {
                verticalSpeed = jumpSpeed;
                jumpedTime = null;
                lastGroundedTime = null;
            }
        } else {
            characterController.stepOffset = 0.0f;
        }

        transform.Translate(movementDirection * walkSpeed * speed * Time.deltaTime, Space.World);

        Vector3 velocity = movementDirection * speed;
        velocity.y = verticalSpeed;
        characterController.Move(velocity * Time.deltaTime);

        // if player is moving, rotate towards the direction of the movement
        if (movementDirection != Vector3.zero) {

            Quaternion rotation = Quaternion.LookRotation(movementDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, rotationSpeed * Time.deltaTime);

        }
    }
    
    private bool locked = true;

    // Update is called once per frame
    void Update()
    {
        if (view.IsMine)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                captured = true;
            }

            // temporary cursor unlock 
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                locked = locked ? false : true;
                
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
        //added for footsteps
        if (Input.GetKey("w") && onground)
        {
            footsteps();
        }

        if (Input.GetKeyDown("s") && onground)
        {
            footsteps();
        }

        if (Input.GetKeyDown("a") && onground)
        {
            footsteps();
        }

        if (Input.GetKeyDown("d") && onground)
        {
            footsteps();
        }

        if (Input.GetKeyUp("w"))
        {
            StopFootsteps();
        }

        if (Input.GetKeyUp("s"))
        {
            StopFootsteps();
        }

        if (Input.GetKeyUp("a"))
        {
            StopFootsteps();
        }

        if (Input.GetKeyUp("d"))
        {
            StopFootsteps();
        }
    }

    void footsteps()
    {
        footstep.SetActive(true);
    }

    void StopFootsteps()
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
