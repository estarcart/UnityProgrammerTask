using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb2D;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private PlayerAnimator playerAnimator;
    [SerializeField] private InventoryController inventoryController;
    [SerializeField] private DialogueController dialogueController;
    [SerializeField] private float speedMovement = 5f;
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private float jumpCooldown = 0.1f;

    private Controller controller;
    private Vector2 direction;
    private bool isGrounded;
    private float lastJumpTime;
    private bool isJumping;


    private void Awake()
    {
        controller = new();
    }

    private void OnEnable()
    {
        controller.Enable();
        controller.Base.Jump.started += Jump;
    }
    private void OnDisable()
    {
        controller.Disable();
    }

    private void Update()
    {
        if ((inventoryController != null && inventoryController.IsOpen) ||
            (dialogueController != null && dialogueController.IsDialogueActive))
        {
            direction = Vector2.zero;
            return;
        }
        
        direction = controller.Base.Movement.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        bool onGround = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        
        if (isJumping)
        {
            if (rb2D.linearVelocity.y < 0)
            {
                isJumping = false;
            }
            isGrounded = false;
        }
        else
        {
            isGrounded = groundCheck;
        }
        
        rb2D.linearVelocity = new Vector2(speedMovement * direction.x, rb2D.linearVelocity.y);
        playerAnimator.UpdateAnimation(rb2D.linearVelocity, isGrounded);
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if ((inventoryController != null && inventoryController.IsOpen) ||
            (dialogueController != null && dialogueController.IsDialogueActive))
            return;

        if (Time.time - lastJumpTime < jumpCooldown)
            return;

        bool grounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        
        if (grounded && rb2D.linearVelocity.y <= 0.1f)
        {
            lastJumpTime = Time.time;
            isJumping = true;
            rb2D.linearVelocity = new Vector2(rb2D.linearVelocity.x, 0f);
            rb2D.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }
}
