﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMouseControl : MonoBehaviour
{
    GameObject target;                           // Target to follow
    float targetHeight = 0.45f;                   // Vertical offset adjustment
    float distance = 4.0f;                       // Default Distance
    float offsetFromWall = 0.1f;                 // Bring camera away from any colliding objects
    public float maxDistance = 20f;                     // Maximum zoom Distance
    public float minDistance = 2f;                    // Minimum zoom Distance
    public float xSpeed = 10.0f;                       // Orbit speed (Left/Right)
    public float ySpeed = 10.0f;                       // Orbit speed (Up/Down)
    float yMinLimit = -40f;                      // Looking up limit
    float yMaxLimit = 60f;                       // Looking down limit
    public float zoomRate = 1f;                        // Zoom Speed
    //float rotationDampening = 3.0f;              // Auto Rotation speed (higher = faster)
    float zoomDampening = 5.0f;                  // Auto Zoom speed (Higher = faster)
    public LayerMask collisionLayers = -1;       // What the camera will collide with
    //bool lockToRearOfTarget = false;             // Lock camera to rear of target
    bool allowMouseInputX = true;                // Allow player to control camera angle on the X axis (Left/Right)
    bool allowMouseInputY = true;                // Allow player to control camera angle on the Y axis (Up/Down)

    private float xDeg = 0.0f;
    private float yDeg = 0.0f;
    private float currentDistance;
    private float desiredDistance;
    private float correctedDistance;
    //private bool rotateBehind = false;
    //private bool mouseSideButton = false;
    private float pbuffer = 0.0f;                //Cooldownpuffer for SideButtons
    //private float coolDown = 0.5f;               //Cooldowntime for SideButtons 

    bool HideCursor = true;
    public bool LockCamera;
    public float moveSpeed = 5;
    public SkinnedMeshRenderer targetRenderer;
    public bool changeTransparency = true;
    public float closestDistanceToPlayer = 1.2f;

    public Player_Input _playerInput;
    public Vector2 _currentMouseDelta;
    public float _currentMouseZoom;

    void Awake()
    {
        _playerInput = new Player_Input();
        _playerInput.CharacterControls.Look.started += OnMouseMove;
        _playerInput.CharacterControls.Look.canceled += OnMouseMove;
        _playerInput.CharacterControls.Look.performed += OnMouseMove;
        _playerInput.CharacterControls.Zoom.started += OnMouseZoom;
        _playerInput.CharacterControls.Zoom.canceled += OnMouseZoom;
        _playerInput.CharacterControls.Zoom.performed += OnMouseZoom;

        Vector3 angles = transform.eulerAngles;
        xDeg = angles.x;
        yDeg = angles.y;
        currentDistance = distance;
        desiredDistance = distance;
        correctedDistance = distance;

        // Make the rigid body not change rotation
        if (GetComponent<Rigidbody>())
            GetComponent<Rigidbody>().freezeRotation = true;

        //if (lockToRearOfTarget)
        //rotateBehind = true;

        LockCamera = false;
        if (HideCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void OnMouseMove(InputAction.CallbackContext context)
    {
        _currentMouseDelta = context.ReadValue<Vector2>();
    }

    void OnMouseZoom(InputAction.CallbackContext context)
    {
        _currentMouseZoom = context.ReadValue<float>();
    }

    private void OnEnable()
    {
        _playerInput.CharacterControls.Enable();
    }

    private void OnDisable()
    {
        _playerInput.CharacterControls.Disable();
    }

    void Update()
    {
        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player") as GameObject;
            //targetRenderer = target.transform.Find("Body").GetComponent<SkinnedMeshRenderer>();
            //Debug.Log("Looking for Player");
        }
    }

    //Only Move camera after everything else has been updated
    void LateUpdate()
    {
        if (!LockCamera)
            OrbitAndZoom();
    }

    void OrbitAndZoom()
    {
        // Don't do anything if target is not defined
        if (target == null)
            return;
        //pushbuffer
        if (pbuffer > 0)
            pbuffer -= Time.deltaTime;
        if (pbuffer < 0) pbuffer = 0;

        /*//Sidebuttonmovement
        if ((Input.GetAxis("Toggle Move") != 0) && (pbuffer == 0))
        {
            pbuffer = coolDown;
            mouseSideButton = !mouseSideButton;
        }
        if (mouseSideButton && Input.GetAxis("Vertical") != 0)
            mouseSideButton = false;
            */
        Vector3 vTargetOffset;

        // If either mouse buttons are down, let the mouse govern camera position
        if (GUIUtility.hotControl == 0)
        {
            //if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
            //{
            //Check to see if mouse input is allowed on the axis
            if (allowMouseInputX)
                xDeg += _currentMouseDelta.x * xSpeed * 0.02f;
            //else
            //RotateBehindTarget();
            if (allowMouseInputY)
                yDeg -= _currentMouseDelta.y * ySpeed * 0.02f;

            //Interrupt rotating behind if mouse wants to control rotation
            //if (!lockToRearOfTarget)
            //rotateBehind = false;
            //}

            /*// otherwise, ease behind the target if any of the directional keys are pressed
            else if (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0 || rotateBehind || mouseSideButton)
            {
                RotateBehindTarget();
            }*/
        }
        yDeg = ClampAngle(yDeg, yMinLimit, yMaxLimit);

        // Set camera rotation
        Quaternion rotation = Quaternion.Euler(yDeg, xDeg, 0);

        // Calculate the desired distance
        desiredDistance -= _currentMouseZoom * Time.deltaTime * zoomRate * Mathf.Abs(desiredDistance);
        desiredDistance = Mathf.Clamp(desiredDistance, minDistance, maxDistance);
        correctedDistance = desiredDistance;

        // Calculate desired camera position
        vTargetOffset = new Vector3(0, -targetHeight*2.25f, 0);
        Vector3 position = target.transform.position - (rotation * Vector3.forward * desiredDistance + vTargetOffset);

        // Check for collision using the true target's desired registration point as set by user using height
        RaycastHit collisionHit;
        Vector3 trueTargetPosition = new Vector3(target.transform.position.x, target.transform.position.y + targetHeight, target.transform.position.z);

        // If there was a collision, correct the camera position and calculate the corrected distance
        var isCorrected = false;
        if (Physics.Linecast(trueTargetPosition, position, out collisionHit, collisionLayers))
        {
            // Calculate the distance from the original estimated position to the collision location,
            // subtracting out a safety "offset" distance from the object we hit.  The offset will help
            // keep the camera from being right on top of the surface we hit, which usually shows up as
            // the surface geometry getting partially clipped by the camera's front clipping plane.
            correctedDistance = Vector3.Distance(trueTargetPosition, collisionHit.point) - offsetFromWall;
            isCorrected = true;
            //TransparencyCheck();
        }
        //else
            //FullTransparency();

        // For smoothing, lerp distance only if either distance wasn't corrected, or correctedDistance is more than currentDistance
        currentDistance = !isCorrected || correctedDistance > currentDistance ? Mathf.Lerp(currentDistance, correctedDistance, Time.deltaTime * zoomDampening) : correctedDistance;

        // Keep within limits
        currentDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance);

        // Recalculate position based on the new currentDistance
        position = target.transform.position - (rotation * Vector3.forward * currentDistance + vTargetOffset);

        //Finally Set rotation and position of camera
        transform.rotation = rotation;
        transform.position = position /* + transform.right * targetHeight*/;
    }

    /*private void RotateBehindTarget()
    {
        float targetRotationAngle = target.transform.eulerAngles.y;
        float currentRotationAngle = transform.eulerAngles.y;
        xDeg = Mathf.LerpAngle(currentRotationAngle, targetRotationAngle, rotationDampening * Time.deltaTime);

        // Stop rotating behind if not completed
        if (targetRotationAngle == currentRotationAngle)
        {
            //if (!lockToRearOfTarget)
                //rotateBehind = false;
        }
        else
            rotateBehind = true;
    }*/

    private float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360f)
            angle += 360f;
        if (angle > 360f)
            angle -= 360f;
        return Mathf.Clamp(angle, min, max);
    }

    
    private void TransparencyCheck()
    {
        if (changeTransparency)
        {
            if (Vector3.Distance(transform.position, target.transform.position) <= closestDistanceToPlayer)
            {
                Color temp = targetRenderer.sharedMaterial.color;
                temp.a = Mathf.Lerp(temp.a, 0.2f, moveSpeed * Time.deltaTime);
                targetRenderer.sharedMaterial.color = temp;
            }
            else
            {
                if (targetRenderer.sharedMaterial.color.a <= 0.99f)
                {
                    Color temp = targetRenderer.sharedMaterial.color;
                    temp.a = Mathf.Lerp(temp.a, 1, moveSpeed * Time.deltaTime);
                    targetRenderer.sharedMaterial.color = temp;
                }
            }
        }
    }

    private void FullTransparency()
    {
        if (changeTransparency)
        {
            if (targetRenderer.sharedMaterial.color.a <= 0.99f)
            {
                Color temp = targetRenderer.sharedMaterial.color;
                temp.a = Mathf.Lerp(temp.a, 1, moveSpeed * Time.deltaTime);
                targetRenderer.sharedMaterial.color = temp;
            }
        }
    }
    

    public void SetLockCamera(bool b) { LockCamera = b; }

    public bool GetLockCamera() { return LockCamera; }

    public void SetHideCursor(bool b)
    {
        HideCursor = b;
        if (HideCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public bool GetHideCursor() { return HideCursor; }
}