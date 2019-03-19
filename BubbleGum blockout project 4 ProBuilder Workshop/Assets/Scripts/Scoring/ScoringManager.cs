using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;

[RequireComponent(typeof(ScoringFeedback))]
public class ScoringManager : MonoBehaviour {

	private enum FunctionType{EXP, LOG}
	private delegate float ScoringFunction(float x);
	public static ScoringManager Instance {get; private set;}

	[Header("Timer Parameters")]
	[Tooltip("The game time (in seconds)")]
	[SerializeField] private float gameTime = 180;
	[SerializeField] private bool endGameWithTimer = true;

	[Header("Air Parameters")]
	[SerializeField] private float secondBeforeAirTime = 0f;
	
	[SerializeField] private FunctionType functionType = FunctionType.EXP;
	[MinValue(0f)]
	[SerializeField] private float functionA = 1f;

	[MinValue(0f)]
	[SerializeField] private float functionB = 1f;

	[Header("Bump parameters")]
	[SerializeField] private int firstBumpScoring = 0;

	[Header("Strike Parameters")]
	[SerializeField] private int maxStrike = 3;
	[SerializeField] private bool endGameWithStrike = true;


	[Header("References")]
	[SerializeField] private EndScreen endScreen = null;

	[Header("Debug")]
	[SerializeField] private float timeUpValue = 10f;
	
	public ScoringFeedback ScoringFeedback {get; private set;}

	public PlayerCharacter Player {get; private set;}


	private ScoringFunction scoringFunction = null;

	private bool beforeInAir;
	private bool inAir;
	private int actualScore;
	private float actualAirScore;
	private int nbJump;

	private float airTime;

	private float timer;

	private int strike;

	private void Awake() {
		if(Instance != null){
			if(Instance != this){
				Destroy(gameObject);
			}

			return;
		}

		Instance = this;
		// DontDestroyOnLoad(gameObject);

		Player = FindObjectOfType<PlayerCharacter>();
		ScoringFeedback = GetComponent<ScoringFeedback>();

		beforeInAir = false;
		inAir = false;
		airTime = 0f;
		nbJump = 0;
		strike = 0;

		switch(functionType){
			case FunctionType.EXP:
				scoringFunction = ExpScoringFunction;
				break;
			case FunctionType.LOG:
				scoringFunction = LogScoringFunction;
				break;
		}
	}

	private void Start() {
		Player.PlayerComponents.Strike.OnGrabEvent += TakeStrike;

		Player.BikeComponents.AirState.RegisterOnGround(OnGround);
		Player.BikeComponents.AirState.RegisterOnGround(EndScoring);
		Player.BikeComponents.AirState.RegisterNoBreakJump(NoBreakJump);
		Player.BikeComponents.AirState.RegisterInAir(BeginInAir);
		Player.BikeComponents.AirState.RegisterInFall(EndScoring);

		timer = gameTime;
		ScoringFeedback.SetTime(timer);
	}

	private void Update() {
		if(endScreen.IsEnabled){
			return;
		}

		if(CustomInputManager.Instance.GetTimeUp()){
			timer += timeUpValue;
		}
		
		if(CustomInputManager.Instance.GetTimeDown()){
			timer -= timeUpValue;
		}

		timer = Mathf.Max(timer - Time.deltaTime, 0f);
		ScoringFeedback.SetTime(timer);

		if(endGameWithTimer && (timer == 0f)){
			endScreen.EnableEndScreen(Player, actualScore, false);
			return;
		}

		

		if(beforeInAir){
			airTime += Time.deltaTime;	
			if(airTime > secondBeforeAirTime){
				BeginScoring();
				beforeInAir = false;
				airTime = 0f;
			}
		}

		if(inAir){
			actualAirScore += Time.deltaTime;
			ScoringFeedback.SetAirScore((int) scoringFunction(actualAirScore));
		}
	}

	public void TakeStrike(){
		if(endScreen.IsEnabled){
			return;
		}

		strike++;
		ScoringFeedback.SetStrike(strike);

		if(endGameWithStrike && (strike >= maxStrike)){
			endScreen.EnableEndScreen(Player, actualScore, true);
			return;
		}
	}

	private void OnGround(){
		if(!beforeInAir){
			return;
		}

		beforeInAir = false;
		inAir = false;
	}

	private void NoBreakJump(NoBreakJump other){
		if(other is JumpScoring){
			nbJump++;
			if(nbJump > (firstBumpScoring - 1)){
				ScoringFeedback.SetJump(nbJump);
			}
		}
	}

	private void BeginInAir(){
		beforeInAir = true;
		airTime = 0f;
	}

	private void BeginScoring(){
		beforeInAir = false;
		inAir = true;
		ScoringFeedback.BeginAirScore();
	}

	private void EndScoring(){
		if(!inAir){
			return;
		}

		inAir = false;
		int finalAirScore;
		if(nbJump < 2){
			ScoringFeedback.FinishAirScore();
			finalAirScore = (int) scoringFunction(actualAirScore);
		}
		else{
			finalAirScore = (int) scoringFunction(actualAirScore) * nbJump;
			ScoringFeedback.SetAirScore(finalAirScore);
			ScoringFeedback.StopJump();
		}

		AddScore(finalAirScore);
		actualAirScore = 0f;
		nbJump = 0;
	}

	public void AddScore(int score){
		actualScore += score;
		ScoringFeedback.SetGlobalScore(actualScore);
	}

	private void OnValidate() {
		firstBumpScoring = Mathf.Max(firstBumpScoring, 1);
	}

	/* A * ln(B*x + 1) */
	private float LogScoringFunction(float x){
		return functionA * Mathf.Log(functionB * x + 1f);
	}

	/* ((x^(A / 10)) / B) * 100 */
	private float ExpScoringFunction(float x){
		return (Mathf.Pow(x, functionA / 10f) / functionB) * 100f;
	}
}
