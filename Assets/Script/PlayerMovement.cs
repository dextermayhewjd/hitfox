using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // define the speed of an object
    public float walkingSpeed;
    public float speed;
    public float rotationSpeed;
    public float jumpSpeed;
    public bool hidden;

    [SerializeField]
    private Transform cameraTransform;

    private float verticalSpeed;
    private float stepOffset;
    private CharacterController characterController;

    // Start is called before the first frame update
    void Start()
    {

        characterController = GetComponent<CharacterController>();
        stepOffset = characterController.stepOffset;

    }
    
    // Update is called once per frame
    void Update()
    {

        // sprint
        if (characterController.isGrounded) {
            speed = Input.GetButtonDown(KeyCode.LeftShift) ? walkingSpeed * 1.5f : walkingSpeed;  
        }

        // moving towards left or right using key A and D
        float horizontalInput = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        
        // moving forwards or backwards using key W and S
        float verticalInput = Input.GetAxis("Vertical") * speed * Time.deltaTime; 

        Vector3 movementDirection = new Vector3(horizontalInput, 0, verticalInput);
        float magnitude = Mathf.Clamp01(movementDirection.magnitude) * speed;
        movementDirection = Quaternion.AngleAxis(cameraTransform.rotation.eulerAngles.y, Vector3.up) * movementDirection;
        movementDirection.Normalize();

        verticalSpeed += Physics.gravity.y * Time.deltaTime;

        if (characterController.isGrounded) {

            characterController.stepOffset = stepOffset;
            verticalSpeed = -0.5f;

            if (Input.GetButtonDown("Jump")) {
                verticalSpeed = jumpSpeed;
            }
        } else {
            characterController.stepOffset = 0.0f;
        }

        transform.Translate(movementDirection * speed * magnitude * Time.deltaTime, Space.World);

        Vector3 velocity = movementDirection * magnitude;
        velocity.y = verticalSpeed;
        characterController.Move(velocity * Time.deltaTime);

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
