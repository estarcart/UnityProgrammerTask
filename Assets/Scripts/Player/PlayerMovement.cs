using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    // References
    [SerializeField] private Rigidbody2D rb2D;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private PlayerAnimator playerAnimator;
    [SerializeField] private InventoryController inventoryController;
    // Configurations
    [SerializeField] private float speedMovement = 5f;
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float groundCheckRadius = 0.2f;
    // State
    private Controller controller;
    private Vector2 direction;
    private bool isGrounded;


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
        if (inventoryController != null && inventoryController.IsOpen)
        {
            direction = Vector2.zero;
            return;
        }
        
        direction = controller.Base.Movement.ReadValue<Vector2>();
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void FixedUpdate()
    {
        rb2D.linearVelocity = new Vector2(speedMovement * direction.x, rb2D.linearVelocity.y);
        playerAnimator.UpdateAnimation(rb2D.linearVelocity, isGrounded);
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (inventoryController != null && inventoryController.IsOpen)
            return;
            
        if (isGrounded)
        {
            rb2D.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }


}
