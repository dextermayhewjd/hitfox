using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // define the speed of an object
    public float walkSpeed;
    public float rotationSpeed;
    public float jumpSpeed;
    public float jumpGracePeriod;
    public bool hidden;

    public Transform cameraTransform;

    private float verticalSpeed;
    private float stepOffset;
    private float? lastGroundedTime;
    private float? jumpedTime;
    private CharacterController characterController;

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        stepOffset = characterController.stepOffset;
        Cursor.lockState = CursorLockMode.Locked;
    }
    
    public void Teleport(Vector3 position, Quaternion rotation)
    {
        transform.position = position;
        Physics.SyncTransforms();
        transform.rotation = rotation;
        cameraTransform.rotation = rotation;
        //velocity = Vector3.zero;
    }
    
    // Update is called once per frame
    void Update()
    {

        float horizontalInput = Input.GetAxisRaw("Horizontal") * walkSpeed * Time.deltaTime;
        float verticalInput = Input.GetAxisRaw("Vertical") * walkSpeed * Time.deltaTime; 

        Vector3 movementDirection = new Vector3(horizontalInput, 0, verticalInput);
        float inputMagnitude = Mathf.Clamp01(movementDirection.magnitude);
        
        if (Input.GetButton("Sprint")) {
            inputMagnitude *= 1.5f;
        }

        float speed = inputMagnitude * walkSpeed;
        movementDirection = Quaternion.AngleAxis(cameraTransform.rotation.eulerAngles.y, Vector3.up) * movementDirection;
        movementDirection.Normalize();

        verticalSpeed += Physics.gravity.y * Time.deltaTime;

        if (characterController.isGrounded) 
        {
            lastGroundedTime = Time.time;
        }

        if (Input.GetButtonDown("Jump"))
        {
            jumpedTime = Time.time;
        }

        if (Time.time - lastGroundedTime <= jumpGracePeriod) 
        {

            characterController.stepOffset = stepOffset;
            verticalSpeed = -0.5f;

            if (Time.time - jumpedTime <= jumpGracePeriod) 
            {
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

    private void OnApplicationFocus(bool focusStatus) {
        if (focusStatus) {
            Cursor.lockState = CursorLockMode.Locked;
        } else {
            Cursor.lockState = CursorLockMode.None;
        }
        
    }
}
