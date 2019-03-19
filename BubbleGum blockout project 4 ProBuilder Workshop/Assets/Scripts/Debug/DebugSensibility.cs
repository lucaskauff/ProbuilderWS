using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

[RequireComponent(typeof(CanvasGroup))]
public class DebugSensibility : MonoBehaviour
{
    [SerializeField] private float fadeTime = 0f;

    [SerializeField] private float apparitionTime = 1f;

    [SerializeField] private TMP_Text levelText = null;

    private CustomInputManager inputManager;

    private CanvasGroup canvasGroup;

    private Coroutine apparitionCoroutine;
    private Tween fadeTween;

    private void Awake() {
        inputManager = CustomInputManager.Instance;
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
    }

    private void Update() {
        int actualLevel = 0;
        if(inputManager.GetSensibilityDown()){
            actualLevel = inputManager.ChangeSensibilityLevel(-1);
        }
        else if(inputManager.GetSensibilityUp()){
            actualLevel = inputManager.ChangeSensibilityLevel(1);
        }
        else{
            return;
        }

        levelText.text = actualLevel.ToString();

        if(apparitionCoroutine != null){
            StopCoroutine(apparitionCoroutine);
        }

        fadeTween?.Kill();

        apparitionCoroutine = StartCoroutine(DisplayLevelCoroutine());
    }

    private IEnumerator DisplayLevelCoroutine(){
        canvasGroup.alpha = 1f;
        yield return new WaitForSecondsRealtime(apparitionTime);

        fadeTween = DOTween.To(x => canvasGroup.alpha = x, 1f, 0f, fadeTime);
    }
}
