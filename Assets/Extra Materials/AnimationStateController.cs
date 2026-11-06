using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Simple walk + run controller.
/// WASD / arrow keys (or gamepad left stick) to move, hold Left Shift
/// (or click the left stick) to run. The character turns to face the
/// direction it moves and feeds the Animator blend tree.
/// </summary>
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
public class SimpleWalkRun : MonoBehaviour
{
    [Header("Input")]
    [Tooltip("Optional. Drag your .inputactions file here to also support the gamepad stick.")]
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private string actionMapName = "Player";
    [SerializeField] private string moveActionName = "Move";

    [Header("Animator Parameters")]
    [SerializeField] private string moveXParam = "MoveX";
    [SerializeField] private string moveYParam = "MoveY";

    [Header("Movement")]
    [Tooltip("ON: W moves away from the camera (use this with Cinemachine). OFF: W always moves toward world forward.")]
    [SerializeField] private bool cameraRelative = true;
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float runSpeed = 6f;
    [Tooltip("Degrees per second the character turns toward its movement direction.")]
    [SerializeField] private float turnSpeed = 720f;
    [Tooltip("Gravity so the character stays on the ground (negative number).")]
    [SerializeField] private float gravity = -20f;

    [Header("Animation")]
    [Tooltip("How far out the blend tree is pushed when walking. Match your Walk clips' position (e.g. 0.5). Running uses 1.")]
    [SerializeField] private float walkBlendScale = 0.5f;
    [Tooltip("Smoothing for the blend tree. Small = snappy, bigger = floaty.")]
    [SerializeField] private float dampTime = 0.1f;

    private InputAction moveAction;
    private Animator animator;
    private CharacterController controller;
    private float verticalVelocity;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();

        if (inputActions != null)
        {
            var map = inputActions.FindActionMap(actionMapName, throwIfNotFound: false);
            if (map != null)
                moveAction = map.FindAction(moveActionName, throwIfNotFound: false);
        }
    }

    private void OnEnable()
    {
        moveAction?.Enable();
    }

    private void OnDisable()
    {
        moveAction?.Disable();
    }

    private void Update()
    {
        // Read input: keyboard (WASD + arrows), falling back to the gamepad stick
        Vector2 input = moveAction != null ? moveAction.ReadValue<Vector2>() : Vector2.zero;

        Keyboard kb = Keyboard.current;
        if (kb != null)
        {
            Vector2 keys = Vector2.zero;
            if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) keys.x += 1f;
            if (kb.aKey.isPressed || kb.leftArrowKey.isPressed)  keys.x -= 1f;
            if (kb.wKey.isPressed || kb.upArrowKey.isPressed)    keys.y += 1f;
            if (kb.sKey.isPressed || kb.downArrowKey.isPressed)  keys.y -= 1f;

            if (keys != Vector2.zero) input = Vector2.ClampMagnitude(keys, 1f);
        }

        bool running = (kb != null && (kb.leftShiftKey.isPressed || kb.rightShiftKey.isPressed))
                       || (Gamepad.current != null && Gamepad.current.leftStickButton.isPressed);

        Vector3 direction = new Vector3(input.x, 0f, input.y);

        // Camera-relative: W moves away from the camera, A/D move left/right of the camera view
        if (cameraRelative && Camera.main != null)
        {
            Transform cam = Camera.main.transform;
            Vector3 forward = Vector3.ProjectOnPlane(cam.forward, Vector3.up).normalized;
            Vector3 right = Vector3.ProjectOnPlane(cam.right, Vector3.up).normalized;
            direction = forward * input.y + right * input.x;
        }

        // Turn toward the movement direction
        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion target = Quaternion.LookRotation(direction.normalized, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, target, turnSpeed * Time.deltaTime);
        }

        // Feed the blend tree: walking = middle ring, running = outer ring
        float amount = input.magnitude * (running ? 1f : walkBlendScale);
        animator.SetFloat(moveXParam, 0f, dampTime, Time.deltaTime);
        animator.SetFloat(moveYParam, amount, dampTime, Time.deltaTime);

        // Move (with gravity so the character stays on the ground)
        if (controller.isGrounded && verticalVelocity < 0f) verticalVelocity = -2f;
        verticalVelocity += gravity * Time.deltaTime;

        float speed = running ? runSpeed : walkSpeed;
        Vector3 motion = direction * speed + Vector3.up * verticalVelocity;
        controller.Move(motion * Time.deltaTime);
    }
}