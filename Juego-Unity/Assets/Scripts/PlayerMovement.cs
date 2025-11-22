using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    public float walkSpeed = 4f;
    public float sprintSpeed = 6f;
    public float jumpHeight = 2f;
    public float gravity = -20f;
    public float rotationSpeed = 10f;

    [Header("Referencias")]
    public Transform cameraTransform;
    public Animator animator;

    private CharacterController controller;
    private Vector3 velocity;
    private float currentSpeed;

    public bool IsGrounded { get; private set; }
    public bool IsMoving { get; private set; }

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        HandleMovement();
        HandleRotation();
        UpdateAnimator();
    }

    void HandleMovement()
    {
        IsGrounded = controller.isGrounded;

        if (IsGrounded && velocity.y < 0)
            velocity.y = -2f;

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 inputDirection = new Vector3(horizontal, 0f, vertical).normalized;
        IsMoving = inputDirection.magnitude >= 0.1f;

        Vector3 moveDirection = Vector3.zero;

        if (IsMoving)
        {
            moveDirection = Quaternion.Euler(0f, cameraTransform.eulerAngles.y, 0f) * inputDirection;
            bool isSprinting = Input.GetKey(KeyCode.LeftShift);
            currentSpeed = isSprinting ? sprintSpeed : walkSpeed;
        }

        // Salto
        if (Input.GetButtonDown("Jump") && IsGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            animator.SetBool("isJumping", true);
        }

        velocity.y += gravity * Time.deltaTime;

        Vector3 finalMovement = (moveDirection * currentSpeed) * Time.deltaTime;
        finalMovement.y += velocity.y * Time.deltaTime;

        controller.Move(finalMovement);

        if (IsGrounded && velocity.y < 0f)
            animator.SetBool("isJumping", false);
    }

    void HandleRotation()
    {
        if (IsMoving)
        {
            // Rotar SOLO al moverse (Estilo Fortnite)
            Vector3 forward = new Vector3(cameraTransform.forward.x, 0, cameraTransform.forward.z).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    void UpdateAnimator()
    {
        float speedPercent = IsMoving ? (currentSpeed == sprintSpeed ? 1f : 0.5f) : 0f;
        animator.SetFloat("Speed", speedPercent, 0.1f, Time.deltaTime);
        animator.SetBool("IsGrounded", IsGrounded);
        animator.SetFloat("VerticalVelocity", velocity.y);
    }
}
