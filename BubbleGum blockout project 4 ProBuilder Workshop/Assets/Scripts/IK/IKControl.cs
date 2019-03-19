using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(Animator))] 

public class IKControl : MonoBehaviour {
    
    protected Animator animator;
    
    public bool useFootIk = true;
    public bool useHandIk = true;
    public Transform rightFoot = null;
    public Transform leftFoot = null;
	public Transform rightHand = null;
    public Transform leftHand = null;

    void Start () 
    {
        animator = GetComponent<Animator>();
    }
    
    //a callback for calculating IK
    void OnAnimatorIK()
    {
        if(animator) {
            
            //if the IK is active, set the position and rotation directly to the goal. 
            if(useFootIk) {

                // // Set the look target position, if one has been assigned
                // if(rightFoot != null) {
                //     animator.SetLookAtWeight(1);
                //     animator.SetLookAtPosition(rightFoot.position);
                // }    

                // Set the right hand target position and rotation, if one has been assigned
                if(rightFoot != null) {
                    animator.SetIKPositionWeight(AvatarIKGoal.RightFoot,1);
                    animator.SetIKRotationWeight(AvatarIKGoal.RightFoot,1);  
                    animator.SetIKPosition(AvatarIKGoal.RightFoot,rightFoot.position);
                    animator.SetIKRotation(AvatarIKGoal.RightFoot,rightFoot.rotation);
                }

                if(leftFoot != null){
					animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot,1);
                    animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot,1);  
                    animator.SetIKPosition(AvatarIKGoal.LeftFoot,leftFoot.position);
                    animator.SetIKRotation(AvatarIKGoal.LeftFoot,leftFoot.rotation);
                }
            }

            if(useHandIk){
                if(leftHand != null){
					animator.SetIKPositionWeight(AvatarIKGoal.LeftHand,1);
                    animator.SetIKRotationWeight(AvatarIKGoal.LeftHand,1);  
                    animator.SetIKPosition(AvatarIKGoal.LeftHand,leftHand.position);
                    animator.SetIKRotation(AvatarIKGoal.LeftHand,leftHand.rotation);
                }

                if(rightHand != null){
					animator.SetIKPositionWeight(AvatarIKGoal.RightHand,1);
                    animator.SetIKRotationWeight(AvatarIKGoal.RightHand,1);  
                    animator.SetIKPosition(AvatarIKGoal.RightHand,rightHand.position);
                    animator.SetIKRotation(AvatarIKGoal.RightHand,rightHand.rotation);
                }        
                
            }
            
            //if the IK is not active, set the position and rotation of the hand and head back to the original position
            else {          
                animator.SetIKPositionWeight(AvatarIKGoal.RightFoot,0);
                animator.SetIKRotationWeight(AvatarIKGoal.RightFoot,0); 
                animator.SetLookAtWeight(0);
            }
        }
    }    
}