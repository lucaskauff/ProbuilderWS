using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CanvasGroup))]
public class EndScreen : MonoBehaviour
{
    [SerializeField] private float fadeTime = 1f;
    [SerializeField] private bool beginOnEndScreen;

    [Header("References")]
    [SerializeField] private CanvasGroup beginText = null;
    [SerializeField] private CanvasGroup endOfTimeText = null;
    [SerializeField] private CanvasGroup catchText = null;
    [SerializeField] private CanvasGroup scoreGroup = null;
    [SerializeField] private TMP_Text scoreText = null;

    private CustomInputManager inputManager;

    private CanvasGroup canvasGroup;

    public bool IsEnabled {get; private set;}

    private void Awake() {
        canvasGroup = GetComponent<CanvasGroup>();
        inputManager = CustomInputManager.Instance;
        canvasGroup.alpha = 0f;
        IsEnabled = false;

        if(beginOnEndScreen){
            IsEnabled = true;
            Time.timeScale = 0f;
            beginText.alpha = 1f;
            catchText.alpha = 0f;
            endOfTimeText.alpha = 0f;
            scoreGroup.alpha = 0f;

            canvasGroup.alpha = 1f;
        }
    }

    private void Update() {
        if(!IsEnabled){
            return;
        }

        if(inputManager.GetRestartButtonDown()){
            Time.timeScale = 1f;
            if(beginOnEndScreen){
                beginOnEndScreen = false;
                IsEnabled = false;

                canvasGroup.alpha = 0f;
                return;
            }
            
            inputManager.UnLockAll();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        else if(inputManager.GetQuitButtonDown()){
            Application.Quit();
        }
    }

    public void EnableEndScreen(PlayerCharacter character, int finalScore, bool catched) {
        IsEnabled = true;

        Time.timeScale = 0f;

        if(catched){
            catchText.alpha = 1f;
            endOfTimeText.alpha = 0f;
        }
        else{
            catchText.alpha = 0f;
            endOfTimeText.alpha = 1f;
        }

        beginText.alpha = 0f;
        scoreGroup.alpha = 1f;

        scoreText.text = finalScore.ToString();

        DOTween.To(x => canvasGroup.alpha = x, 0f, 1f, fadeTime).SetUpdate(true);
        
    }
}
