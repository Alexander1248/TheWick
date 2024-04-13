using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
    using UnityEditor;
    using System.Net;
#endif

public class FirstPersonController2 : MonoBehaviour
{
    private Rigidbody rb;

    public Camera playerCamera;

    public float fov = 60f;
    public bool invertCamera = false;
    public bool cameraCanMove = true;
    public float mouseSensitivity = 2f;
    public float maxLookAngle = 50f;

    // Crosshair
    public bool lockCursor = true;

    // Internal Variables
    private float yaw = 0.0f;
    private float pitch = 0.0f;

    private float[] minMaxYaw = new float[2] { -1, -1 };

    public bool playerCanMove = true;
    public float walkSpeed = 5f;
    public float maxVelocityChange = 10f;

    private float speedMultiplier = 1;
    private bool underwater;

    private bool isWalking = false;

    public bool enableSprint = true;
    public float sprintSpeed = 7f;


    private bool isSprinting = false;


    public bool enableJump = true;
    public float jumpPower = 5f;

    public bool isGrounded = false;


    public bool enableHeadBob = true;
    public Transform joint;
    public float bobSpeed = 10f;
    public Vector3 bobAmount = new Vector3(.15f, .05f, 0f);


    private Vector3 jointOriginalPos;
    private float timer = 0;


    [Space] 
    public Collider ventCollider;
    public Animator ventAnimator;
    public bool InVent { get; private set; }

    private Hands _hands;
    private Collider _inUse;

    private void ChangeCollider(Collider coll)
    {
        if (coll.IsUnityNull()) return;
        _inUse.enabled = false;
        _inUse = coll;
        coll.enabled = true;
    }
    
    

    private Collider _ventBuffer;
    public void ChangeVentState()
    {
        if (InVent) VentExit();
        else VentEnter();
    }
    private void VentEnter()
    {
        _ventBuffer = _inUse;
        ChangeCollider(ventCollider);
        InVent = true;
        _hands.enabled = false;
        ventAnimator.Play("VentEnter", -1, 0);
    }
    private void VentExit()
    {
        InVent = false;
        ChangeCollider(_ventBuffer);
        _hands.enabled = true;
        ventAnimator.Play("VentExit", -1, 0);
    }
    

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        playerCamera.fieldOfView = fov;
        jointOriginalPos = joint.localPosition;
    }

    private void Start()
    {
        _hands = GetComponent<Hands>();
        foreach (var coll in GetComponents<Collider>())
        {
            if (!coll.enabled) continue;
            if (_inUse == null) _inUse = coll;
            else coll.enabled = false;
        }

        if(lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    float camRotation;

    public void StartFirstCutscene(float[] angle1, float angle2)
    {
        minMaxYaw = angle1;
        playerCanMove = false;
        maxLookAngle = angle2;
    }

    public void EndCutscene()
    {
        minMaxYaw = new float[] { -1, -1 };
        playerCanMove = true;
        maxLookAngle = 89;
    }

    private void Update()
    {
        if(cameraCanMove)
        {
            yaw = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * mouseSensitivity;

            if (!invertCamera)
            {
                pitch -= mouseSensitivity * Input.GetAxis("Mouse Y");
            }
            else
            {
                // Inverted Y
                pitch += mouseSensitivity * Input.GetAxis("Mouse Y");
            }

            pitch = Mathf.Clamp(pitch, -maxLookAngle, maxLookAngle);

            if (minMaxYaw[0] != -1 && yaw < minMaxYaw[0]) yaw = minMaxYaw[0];
            else if (minMaxYaw[1] != -1 && yaw > minMaxYaw[1]) yaw = minMaxYaw[1];

            transform.localEulerAngles = new Vector3(0, yaw, 0);
            playerCamera.transform.localEulerAngles = new Vector3(pitch, 0, 0);
        }

        if(enableJump && Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }

        CheckGround();

        if(enableHeadBob)
        {
            HeadBob();
        }
    }

    void FixedUpdate()
    {
        if (playerCanMove)
        {
            //Vector3 targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
            // Question 69: to be, or not to be, that is the question
            Vector3 targetVelocity = new Vector3(Input.GetAxis("Horizontal"),
                                                0,
                                                Input.GetAxis("Vertical")).normalized;
            if ((targetVelocity.x != 0 || targetVelocity.z != 0))
            {
                isWalking = true;
            }
            else
            {
                isWalking = false;
            }

            // All movement calculations shile sprint is active
            if (Input.GetKey(KeyCode.LeftShift) && enableSprint)
            {
                //targetVelocity = targetVelocity * sprintSpeed;
                targetVelocity = transform.TransformDirection(targetVelocity) * sprintSpeed * speedMultiplier;
                isSprinting = true;
                // Apply a force that attempts to reach our target velocity
                Vector3 velocity = rb.velocity;
                Vector3 velocityChange = (targetVelocity - velocity);
                velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
                velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
                velocityChange.y = 0;

                rb.AddForce(velocityChange, ForceMode.VelocityChange);
            }
            // All movement calculations while walking
            else
            {
                isSprinting = false;

                targetVelocity = transform.TransformDirection(targetVelocity) * walkSpeed * speedMultiplier;

                Vector3 velocity = rb.velocity;
                Vector3 velocityChange = (targetVelocity - velocity);
                velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
                velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
                velocityChange.y = 0;

                rb.AddForce(velocityChange, ForceMode.VelocityChange);
            }

            if (underwater)
            {
                rb.AddForce(new Vector3(0, -rb.velocity.y * 15, 0), ForceMode.Force);
            }
        }
    }
    private void CheckGround()
    {
        Vector3 origin = new Vector3(transform.position.x, transform.position.y - (transform.localScale.y * .5f), transform.position.z);
        Vector3 direction = transform.TransformDirection(Vector3.down);
        float distance = 0.2f;

        if (Physics.Raycast(origin, direction, out RaycastHit hit, distance))
        {
            Debug.DrawRay(origin, direction * distance, Color.red);
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    private void Jump()
    {
        if (!playerCanMove) return;
        // Adds force to the player rigidbody to jump
        if (isGrounded)
        {
            rb.AddForce(0f, jumpPower, 0f, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    public void StopPlayer(){
        playerCanMove = false;
        isWalking = false;
        isSprinting = false;
    }

    public void NoRotate(){
        cameraCanMove = false;
    }

    public void enablePlayer(){
        playerCanMove = true;
        cameraCanMove = true;
    }

    private void HeadBob()
    {
        if(isWalking)
        {
            if(isSprinting)
            {
                timer += Time.deltaTime * (bobSpeed + sprintSpeed);
            }
            else
            {
                timer += Time.deltaTime * bobSpeed;
            }

            joint.localPosition = new Vector3(jointOriginalPos.x + Mathf.Sin(timer) * bobAmount.x, jointOriginalPos.y + Mathf.Sin(timer) * bobAmount.y, jointOriginalPos.z + Mathf.Sin(timer) * bobAmount.z);
        }
        else
        {
            // Resets when play stops moving
            timer = 0;
            joint.localPosition = new Vector3(Mathf.Lerp(joint.localPosition.x, jointOriginalPos.x, Time.deltaTime * bobSpeed), Mathf.Lerp(joint.localPosition.y, jointOriginalPos.y, Time.deltaTime * bobSpeed), Mathf.Lerp(joint.localPosition.z, jointOriginalPos.z, Time.deltaTime * bobSpeed));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            enableSprint = false;
            speedMultiplier = 0.5f;
            underwater = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            enableSprint = true;
            speedMultiplier = 1f;
            underwater = false;
        }
    }
}