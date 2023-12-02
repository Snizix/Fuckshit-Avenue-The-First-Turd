using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    public Animator anim;
    PlayerMovement pm;
    PlayerRun pr;

    int isWalkingHash;
    int isRunningHash;
    int isJumpingHash;
    int isFallingHash;

    bool isWalking;
    bool isRunning;

    // Start is called before the first frame update
    void Start()
    {
        pm = GetComponent<PlayerMovement>();
        pr = GetComponent<PlayerRun>();

        // Using a hash will increase performance; idk why though...
        isWalkingHash = Animator.StringToHash("isWalking");
        isRunningHash = Animator.StringToHash("isRunning");
        isJumpingHash = Animator.StringToHash("isJumping");
        isFallingHash = Animator.StringToHash("isFalling");
    }

    // Update is called once per frame
    void Update()
    {
        if(anim != null)
        {
            GetParameters();
            SetAnimationStates();
        }
    }

    void GetParameters()
    {
        isWalking = anim.GetBool(isWalkingHash);
        isRunning = anim.GetBool(isRunningHash);
    }
    
    void SetAnimationStates()
    {
        if(pm != null)
        {
            // Walking
            if (pm.isMovementPressed && isWalking == false)
                anim.SetBool(isWalkingHash, true);
            else if (!pm.isMovementPressed && isWalking)
                anim.SetBool(isWalkingHash, false);

            
            // Jumping
            if (pm.isJumping && pm.appliedMovement.y > 0.05f)
            {
                anim.SetBool(isJumpingHash, true);
                anim.SetBool(isFallingHash, false);
            }
            else if (pm.appliedMovement.y < 0 && !pm.characterController.isGrounded)
            {
                anim.SetBool(isJumpingHash, false);
                anim.SetBool(isFallingHash, true);
                Debug.Log(pm.appliedMovement.y);
            }
            else
            {
                anim.SetBool(isJumpingHash, false);
                anim.SetBool(isJumpingHash, false);
            }

        }
        
        // Running
        if(pr != null)
        {
            if (pr.isRunPressed == true && pm.isMovementPressed == true && isRunning == false)
                anim.SetBool(isRunningHash, true);
            else if ((pr.isRunPressed == false || pm.isMovementPressed == false) && isRunning == true)
                anim.SetBool(isRunningHash, false);
        }
    }
}
