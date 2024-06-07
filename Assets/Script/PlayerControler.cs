using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControler : MonoBehaviour
{
    [SerializeField] private float sensibility = 1f; 
    [SerializeField] private float walkSpeed = 1f;
    [SerializeField] private float squatSpeed = 1f;
    [SerializeField] private float jumpForce = 1f;
    [SerializeField] private float squatHeight = 1f;
    [SerializeField] private Camera eyes;

    private float speed;

    private Vector3 velocity; 

    private Vector2 moveInputs, lookInputs;

    private bool jumpPerformed, squatPerformed;

    private float eyesRotation = 0f;

    private CharacterController characterController;
    private void Awake()
    {

        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        speed = walkSpeed;
    }

    private void FixedUpdate()
    {
        Vector3 direction = transform.forward * moveInputs.y + transform.right * moveInputs.x;
        Vector3 _horizontalvelocity = speed * direction;
        float _gravityvelocity = Gravity(velocity.y);

        velocity = _horizontalvelocity + _gravityvelocity * Vector3.up;

        TryJump(); 

        characterController.Move(velocity * Time.fixedDeltaTime);
    }

    private void Update()
    {
       
        float yrotation = lookInputs.x * sensibility * Time.deltaTime;
        float xRotation = lookInputs.y * sensibility * Time.deltaTime;
        transform.localEulerAngles += Vector3.up * yrotation;

        eyesRotation += xRotation;
        eyesRotation = Mathf.Clamp(eyesRotation, -90, 90);

        eyes.transform.localEulerAngles = Vector3.left * eyesRotation;
    }
    private float Gravity(float _verticalVelocity)
    {

        if (characterController.isGrounded) return 0f; 

        _verticalVelocity += Physics.gravity.y * Time.fixedDeltaTime;

        return _verticalVelocity;
    }
    private void TryJump()
    {
        if (!jumpPerformed || !characterController.isGrounded) return;

        velocity.y += jumpForce;
        jumpPerformed = false;
    }
    public void MovePerformed(InputAction.CallbackContext _ctx) => moveInputs = _ctx.ReadValue<Vector2>();
    public void LookPerformed(InputAction.CallbackContext _ctx) => lookInputs = _ctx.ReadValue<Vector2>();
    public void JumpPerformed(InputAction.CallbackContext _ctx) => jumpPerformed = _ctx.performed;
    public void SquatPerformed(InputAction.CallbackContext _ctx)
    {
        squatPerformed = !squatPerformed;
        if (squatPerformed)
        {
            speed = squatSpeed;
            eyes.transform.position -= squatHeight * Vector3.up;
        }
        else
        {
            speed = walkSpeed;
            eyes.transform.position += squatHeight * Vector3.up;
        }
    }

    public Vector3 GetEyesPosition()
    {
        return eyes.transform.position;
    }
}
