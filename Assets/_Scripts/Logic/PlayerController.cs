using System.Linq;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputManager inputManager;
    [SerializeField] private Slider speedSlider; // 新添加的滑块引用

    [Header("Movement Parameters")]
    [SerializeField] private float maxSpeed;
    [SerializeField] private float acceleration;
    [SerializeField] private float jumpForce;
    [SerializeField] private float dashForce;
    [SerializeField] private float airControl;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private int jumpCount;

    [Header("Physics Materials")]
    [SerializeField] private PhysicsMaterial rough;
    [SerializeField] private PhysicsMaterial smooth;

    private readonly int doubleJump = 2;
    private Rigidbody rb;
    private CinemachineCamera freeLookCamera;
    private bool isDashing;
    private Collider col;

    private RaycastHit rightHit;
    private RaycastHit leftHit;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        freeLookCamera = FindAnyObjectByType<CinemachineCamera>();
    }

    private void Start()
    {
        // 注册输入事件监听器
        if (inputManager != null)
        {
            inputManager.OnMove.AddListener(MovePlayer);
            inputManager.OnJump.AddListener(Jump);
            inputManager.OnDash.AddListener(Dash);
        }

        // 设置滑块初始值并添加监听器
        if (speedSlider != null)
        {
            speedSlider.value = maxSpeed;
            speedSlider.onValueChanged.AddListener(UpdatePlayerMaxSpeed);
        }
    }

    private void OnDestroy()
    {
        // 移除输入事件监听器
        if (inputManager != null)
        {
            inputManager.OnMove.RemoveListener(MovePlayer);
            inputManager.OnJump.RemoveListener(Jump);
            inputManager.OnDash.RemoveListener(Dash);
        }

        // 移除滑块监听器
        if (speedSlider != null)
        {
            speedSlider.onValueChanged.RemoveListener(UpdatePlayerMaxSpeed);
        }
    }

    private void Update()
    {
        transform.rotation = Quaternion.Euler(0, freeLookCamera.transform.rotation.eulerAngles.y, 0);
    }

    private void FixedUpdate()
    {
        if (isDashing)
        {
            if (IsTouchingGround())
            {
                isDashing = false;
            }
        }
        else
        {
            Vector3 velocity = rb.linearVelocity;
            Vector2 horizontalVelocity = new Vector2(velocity.x, velocity.z);

            if (horizontalVelocity.magnitude > maxSpeed)
            {
                horizontalVelocity = horizontalVelocity.normalized * maxSpeed;
            }

            rb.linearVelocity = new Vector3(horizontalVelocity.x, velocity.y, horizontalVelocity.y);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        int contactCount = collision.contactCount;
        float angle = collision.contacts.Sum(contact => Vector3.Angle(contact.normal, Vector3.up)) / contactCount;
        // swapping out materials to avoid sticking on the walls
        col.material = angle > 45 ? smooth : rough;
    }

    private bool IsTouchingGround() => Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1.1f, groundLayer);

    private void MovePlayer(Vector2 dirn)
    {
        Vector3 direction = new Vector3(dirn.x, 0f, dirn.y);
        Quaternion rotation = Quaternion.LookRotation(transform.forward, Vector3.up);
        Vector3 reorientedDirection = rotation * direction;
        if (IsTouchingGround())
        {
            rb.AddForce(reorientedDirection * acceleration);
        }
        else
        {
            rb.AddForce(reorientedDirection * acceleration * airControl);
        }
    }

    private void Jump()
    {
        Vector3 jumpDir = Vector3.up;
        if (IsTouchingGround())
        {
            jumpCount = 0;
        }

        if (jumpCount < doubleJump)
        {
            // resetting vertical velocity before applying jump force
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(jumpDir * jumpForce, ForceMode.Impulse);
            jumpCount++;
        }
    }

    private void Dash()
    {
        isDashing = true;
        rb.linearVelocity = Vector3.zero;
        rb.AddForce(transform.forward * dashForce, ForceMode.Impulse);
    }

    public void UpdatePlayerMaxSpeed(float speed)
    {
        Debug.Log($"Setting player speed to: {speed}");
        maxSpeed = speed;
    }

    // 获取当前最大速度的方法
    public float GetMaxSpeed()
    {
        return maxSpeed;
    }
}