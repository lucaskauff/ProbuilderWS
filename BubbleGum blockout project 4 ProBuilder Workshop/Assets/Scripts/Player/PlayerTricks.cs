using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTricks : MonoBehaviour
{

    private enum TrickState {NO_TRICK, BEGIN_MOTION, TRICK, END_MOTION}

    [SerializeField] private bool onlyInAir = false;

    [Header("References")]
    [SerializeField] private Renderer leftBox = null;
	[SerializeField] private Renderer rightBox = null;
    private CustomInputManager inputManager;

    private TrickState leftActualState = TrickState.NO_TRICK;
    private TrickState rightActualState = TrickState.NO_TRICK;

    private PlayerCharacter player;

	private void Awake() {
		inputManager = CustomInputManager.Instance;
        player = GetComponent<PlayerCharacter>();

		Material leftMaterial = leftBox.material;
		Material rightMaterial = rightBox.material;

		Color leftColor = inputManager.GetJoyconColor(CustomInputManager.JoyconType.LEFT);
		Color rightColor = inputManager.GetJoyconColor(CustomInputManager.JoyconType.RIGHT);
		leftMaterial.SetColor("_TintColor", leftColor);
		rightMaterial.SetColor("_TintColor", rightColor);

		leftBox.material = leftMaterial;
		rightBox.material = rightMaterial;

		leftBox.gameObject.SetActive(false);
		rightBox.gameObject.SetActive(false);
	}

	private void Update() {
        if(onlyInAir && (player.BikeComponents.AirState.InFall || player.BikeComponents.AirState.OnGround)){
            leftActualState = TrickState.NO_TRICK;
            rightActualState = TrickState.NO_TRICK;

            leftBox.gameObject.SetActive(false);
		    rightBox.gameObject.SetActive(false);
            return;
        }

		bool left = inputManager.GetLeftAirTrick();
		bool right = inputManager.GetRightAirTrick();

        leftActualState = UpdateState(leftActualState, left, leftBox.gameObject);
        rightActualState = UpdateState(rightActualState, right, rightBox.gameObject);
	}

    public bool InTricks(){
        return (leftActualState != TrickState.NO_TRICK) || (rightActualState != TrickState.NO_TRICK);
    }

    private static TrickState UpdateState(TrickState actualState, bool motion, GameObject obj){
        switch(actualState){
            case TrickState.NO_TRICK:
                if(motion){
                    obj.SetActive(true);
                    return TrickState.BEGIN_MOTION;
                }
                break;
            case TrickState.BEGIN_MOTION:
                if(!motion){
                    return TrickState.TRICK;
                }
                break;
            case TrickState.TRICK:
                if(motion){
                    obj.SetActive(false);
                    return TrickState.END_MOTION;
                }
                break;
            case TrickState.END_MOTION:
                if(!motion){
                    return TrickState.NO_TRICK;
                }
                break;
        }

        return actualState;
    }
}
