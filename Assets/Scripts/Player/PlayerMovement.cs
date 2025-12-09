using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Controller controller;
    private Vector2 direction;
    [SerializeField] private Rigidbody2D rb2D;
    public float speedMovement;
    [SerializeField] private float jumpForce;
    [SerializeField] private Transform groundCheck;
    private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;
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
        direction = controller.Base.Movement.ReadValue<Vector2>();
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void FixedUpdate()
    {
        rb2D.linearVelocity = new Vector2(speedMovement * direction.x, rb2D.linearVelocity.y);
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (isGrounded)
        {
            rb2D.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }


}
