using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using AudioSystem;
using DG.Tweening;

public class ScoringFeedback : MonoBehaviour {

	[Tooltip("High Five Display Time")]
	[SerializeField] private float hfDisplayTime = 0f;

	[Header("Prefabs")]
	[SerializeField] private UIElement airScorePrefab = null;
	[SerializeField] private UIElement jumpPrefab = null;

	[Header("Score References")]
	[SerializeField] private UIElement timerUI = null;
	[SerializeField] private UIElement finalScoreUI = null;

	[Header("Air Score References")]
	[SerializeField] private RectTransform airScoreSpawn = null;
	[SerializeField] private UIElement.Movement[] airScoreDestinations = null;

	[Header("Bump References")]
	[SerializeField] private float multiplyTime = 0.3f;
	[SerializeField] private RectTransform jumpSpawn = null;

	[Header("Strike References")]
	[SerializeField] private Image[] strikeImages = null;
	[SerializeField] private Sprite strikedSprite = null;

	[Header("High Five References")]
	[SerializeField] private TMP_Text highFiveText = null;
	[SerializeField] private CanvasGroup highFiveGroup = null;

	private Coroutine displayJumpCoroutine;
	private Coroutine displayHighFiveCoroutine;

	private bool isJumping;
	private bool inHighFive;

	private UIElement actualAirScore;
	private UIElement actualJump;

	private void Awake() {
		DOTween.Init();
		isJumping = false;
	}

	public void SetTime(float time){
		timerUI.SetTime((int) time);
	}

	public void SetAirScore(int score){
		actualAirScore?.SetNumber(score);
	}

	public void SetGlobalScore(int score){
		finalScoreUI.SetNumber(score);
	}

	public void BeginAirScore(){
		if(actualAirScore != null){
			return;
		}

		actualAirScore = Instantiate<UIElement>(airScorePrefab, airScoreSpawn.position, airScoreSpawn.rotation, airScoreSpawn.parent);
	}

	public void FinishAirScore(){
		actualAirScore.Move(airScoreDestinations, true);
		actualAirScore = null;
	}

	public void SetJump(int number){
		if(!isJumping){
			isJumping = true;
			actualJump = Instantiate<UIElement>(jumpPrefab, jumpSpawn.position, jumpSpawn.rotation, jumpSpawn.parent);
		}
		actualJump.SetNumber(number);
	}

	public void StopJump(){
		if(!isJumping){
			return;
		}

		isJumping = false;
		RectTransform airTransform = actualAirScore.GetComponent<RectTransform>();

		UIElement.Movement[] airScoreMovements = new UIElement.Movement[airScoreDestinations.Length + 1];
		airScoreMovements[0] = new UIElement.Movement(airTransform, multiplyTime);
		for(int i = 0; i < airScoreDestinations.Length; i++){
			airScoreMovements[i + 1] = airScoreDestinations[i];
		}

		actualJump.Move(airTransform.position, multiplyTime, true);
		actualAirScore.Move(airScoreMovements, true);

		actualAirScore = null;
		actualJump = null;
	}

	public void SetStrike(int nbStrike){
		for(int i = 0; i < nbStrike; i++){
			strikeImages[i].sprite = strikedSprite;
		}
	}

	public void SetHighFive(int number){
		if(!inHighFive){
			inHighFive = true;
			if(displayHighFiveCoroutine != null){
				StopCoroutine(displayHighFiveCoroutine);
			}

			displayHighFiveCoroutine = StartCoroutine(DisplayCoroutine(1f, highFiveGroup, hfDisplayTime));
		}
		highFiveText.text = number.ToString();
	}

	public void StopHighFive(){
		if(!inHighFive){
			return;
		}

		inHighFive = false;
		if(displayHighFiveCoroutine != null){
			StopCoroutine(displayHighFiveCoroutine);
		}
		
		displayHighFiveCoroutine = StartCoroutine(DisplayCoroutine(0f, highFiveGroup, hfDisplayTime));
	}

	public IEnumerator DisplayCoroutine(float lastValue, CanvasGroup group, float displayTime){
		float time = (1f - Mathf.Abs(lastValue - group.alpha)) * displayTime;

		while(time < displayTime){
			group.alpha = Mathf.Lerp(1 - lastValue, lastValue, time / displayTime);
			time += Time.deltaTime;
			yield return null;
		}

		group.alpha = lastValue;
	}
}
