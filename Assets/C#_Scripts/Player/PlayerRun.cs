using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRun : MonoBehaviour
{
    PlayerMovement pm;
    public float runSpeed;
    public bool isRunPressed { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        isRunPressed = false;
        pm = GetComponent<PlayerMovement>();
        pm.playerInput.CharacterControls.Run.started += OnRun;
        pm.playerInput.CharacterControls.Run.canceled += OnRun;
    }

    // Update is called once per frame
    void Update()
    {
        if(isRunPressed)
        {
            Vector3 movementVector = new Vector3(pm.currentMovement.x * runSpeed, pm.appliedMovement.y, pm.currentMovement.z * runSpeed);
            pm.characterController.Move(movementVector * Time.deltaTime);
        }
    }

    void OnRun(InputAction.CallbackContext context)
    {
        isRunPressed = context.ReadValueAsButton();
        pm.useDefaultHorizontalMovement = !isRunPressed;
    }
}
