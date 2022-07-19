using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private bool hasWalked = false;
    private bool isGrounded;
    private bool hasJumped;
    private bool bounceCheck = true;
    public bool hasDashed;
    private bool dashCheck;
    private bool dashing;
    private bool isFacingWall;
    private bool isFacingFloor;
    private bool dashHeight = false;

    public Vector3 spawnPoint;
    private Vector3 lastDir = new Vector3(0, 0, 0);
    private Vector3 lastPos;
    public LayerMask groundMask;
    public LayerMask wallMask;
    public Camera cam;
    public Rigidbody rigid;
    public Goal goal;

    public float moveSpeed;
    public float turnSpeed = 4.0f;
    public float jumpForce = 5;
    public float dashForce = 3;
    private float minTurnAngle = -90.0f;
    private float maxTurnAngle = 90.0f;
    private float rotX;
    public float groundDistance = 0.4f;
    public float groundWidth;
    public float wallDistance = 0.4f;
    public float wallWidth;

    public float timesJumped = 6;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        GroundCheck();
    }

    private void FixedUpdate()
    {
        //MouseAiming();
    }

    void Update()
    {
        if (gameObject.transform.position.y > lastPos.y + 5)
        {
            dashHeight = true;
        }
        
        NoseCheck();
        if (!goal.paused && !goal.goalReached)
        {
            Inputs();
        }
        Respawn();
    }

    void Inputs()
    {
        WalkingInput();
        MouseAiming();
        JumpInput();
        DashInput();
    }

    void Respawn()
    {
        if (transform.position.y < -50 || Input.GetKeyDown(KeyCode.R))
        {
            transform.position = spawnPoint;
            hasWalked = false;
            hasJumped = false;
            hasDashed = false;
            dashing = false;
            timesJumped = 7;
        }
    }

    void MouseAiming()
    {
        // get the mouse inputs
        float y = Input.GetAxis("Mouse X") * turnSpeed;
        rotX += Input.GetAxis("Mouse Y") * turnSpeed;
        // clamp the vertical rotation
        rotX = Mathf.Clamp(rotX, minTurnAngle, maxTurnAngle);
        // rotate the camera    
        cam.transform.eulerAngles = new Vector3(-rotX, transform.eulerAngles.y + y, 0);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y + y, 0);
    }

    void WalkingInput()
    {
        //Walk-Inputs
        Vector3 dir = new Vector3(0, 0, 0);
        dir.x = Input.GetAxis("Horizontal");
        dir.z = Input.GetAxis("Vertical");
        if (dir != new Vector3(0, 0, 0) && !hasWalked)
        {
            hasWalked = true;
            lastDir = dir.normalized;
            if (!isFacingWall)
            {
                transform.Translate(dir * moveSpeed * Time.deltaTime);
            }
        }

        //Walking
        if (hasWalked && !isFacingWall)
        {
            transform.Translate(lastDir * moveSpeed * Time.deltaTime);
        }

    }

    void JumpInput()
    {
        GroundCheck();
        if (Input.GetKeyDown("space") && isGrounded && !hasJumped)
        {
            bounceCheck = true;
            hasJumped = true;
        }
        if (isGrounded && bounceCheck && hasJumped && !dashing) // ignore if value isn't changing
        {
            rigid.AddForce(0, jumpForce * 4, 0, ForceMode.Impulse);
            Debug.Log(timesJumped);
            StartCoroutine(waitForJump(0.05f));
        }

    }

    void DashInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && !hasDashed && !isFacingWall)
        {
            dashCheck = true;
            hasDashed = true;
        }
        if (dashCheck && hasDashed) // ignore if value isn't changing
        {
            if (hasWalked)
            {
                if (hasJumped && dashHeight && timesJumped >= 6)
                {
                    timesJumped = 0;
                    Debug.Log(timesJumped);
                    StartCoroutine(dashTime(0.5f));
                    StartCoroutine(waitForDash(4f));
                }
                else if (!hasJumped)
                {
                    StartCoroutine(dashTime(0.5f));
                    StartCoroutine(waitForDash(4f));
                }
            } else if (!hasWalked)
            {
                if (hasJumped && dashHeight && timesJumped >= 6)
                {
                    timesJumped = 0;
                    Debug.Log(timesJumped);
                    StartCoroutine(dash(0.5f));
                    StartCoroutine(waitForDash(4f));
                }
                else if (!hasJumped)
                {
                    StartCoroutine(dash(0.5f));
                    StartCoroutine(waitForDash(4f));
                }
            }
        }
        if (dashing && !hasWalked && !isFacingWall)
        {
            transform.Translate(new Vector3(0, 0, 1) * moveSpeed * 4 * Time.deltaTime);
        }
    }

    void GroundCheck()
    {
        //Debug.Log(bounceCheck);
        isGrounded = Physics.CheckBox(transform.position, new Vector3(groundWidth, groundDistance, groundWidth), Quaternion.identity, groundMask);
        if (isGrounded)
        {
            lastPos = gameObject.transform.position;
            dashHeight = false;
        }
    }

    void NoseCheck()
    {
        isFacingWall = Physics.CheckBox(transform.position + new Vector3(0, 1, 0 + wallDistance /2), new Vector3(wallWidth, wallWidth, wallDistance), transform.rotation, wallMask);
        if (isFacingWall && moveSpeed > 10)
        {
            moveSpeed = 10;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(groundWidth, groundDistance, groundWidth));
        Gizmos.DrawWireCube(new Vector3(0, 1, 0 + wallDistance/2), new Vector3(wallWidth, wallWidth, wallDistance));
    }

    private IEnumerator waitForJump(float time)
    {
        timesJumped++;
        bounceCheck = false;
        yield return new WaitForSecondsRealtime(time);
        bounceCheck = true;
    }

    private IEnumerator waitForDash(float time)
    {
        dashCheck = false;
        yield return new WaitForSecondsRealtime(time);
        dashCheck = true;
    }

    private IEnumerator dashTime(float time)
    {
        moveSpeed = moveSpeed * 4;
        dashing = true;
        yield return new WaitForSecondsRealtime(time);
        moveSpeed = 10;
        dashing = false;
    }

    private IEnumerator dash(float time)
    {
        dashing = true;
        yield return new WaitForSecondsRealtime(time);
        dashing = false;
    }

}
