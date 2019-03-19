using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using Sirenix.OdinInspector;

[RequireComponent(typeof(CanvasGroup))]
public class UIElement : MonoBehaviour
{
    [System.Serializable]
    public struct Movement{
        public Transform destination;
        public float time;

        public Movement(Transform destination, float time){
            this.destination = destination;
            this.time = time;
        }
    }

    [SerializeField] private float textSpeed = 0f;
    [SerializeField] private string textPrefix = "";
    [SerializeField] private string textSuffix = "";
    [SerializeField] private bool fadeOnInit = true;
    [ShowIf("fadeOnInit"), SerializeField] private float fadeTime = 0.3f;

    [Header("References")]
    [SerializeField] private TMP_Text numberText = null;

    private RectTransform selfTransform;
    private CanvasGroup canvasGroup;

    private float actualNumber;
    private int finalNumber;

    private void Awake() {
        selfTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        if(fadeOnInit){
            DOFadeCanvas(0f, 1f, fadeTime);
        }

        numberText.text = textPrefix + "0" + textSuffix;
    }

    private void Update() {
        if(actualNumber == finalNumber){
            return;
        }

        float speed = textSpeed * Time.deltaTime;
        float diff = finalNumber - actualNumber;

        if(Mathf.Abs(diff) < speed){
            actualNumber = finalNumber;
        }
        else{
            actualNumber += speed * Mathf.Sign(diff);
        }

        numberText.text = textPrefix + actualNumber.ToString("0") + textSuffix;
    }

    public void SetNumber(int number){
        finalNumber = number;
    }

    public void SetInstantNumber(int number){
        finalNumber = number;
        actualNumber = number;
        numberText.text = textPrefix + actualNumber.ToString("0") + textSuffix;
    }

    public void SetTime(int time){
        finalNumber = time;
        actualNumber = time;
        
        int nbMinutes = time / 60;
		int nbSeconds = time % 60;

        numberText.text = nbMinutes.ToString() + ':' + nbSeconds.ToString("00");
    }

    public void Move(Vector3 destination, float time, bool fade = true){
        Sequence sequence = DOTween.Sequence();
        sequence.Append(selfTransform.DOMove(destination, time));
        if(fade){
            sequence.Join(DOFadeCanvas(1f, 0f, time));
        }

        sequence.Play().OnComplete(DestroyUI);
    }

    public void Move(Movement[] movements, bool fadeAtLast = true){
        Sequence sequence = DOTween.Sequence();
        for(int i = 0; i < movements.Length; i++){
            sequence.Append(selfTransform.DOMove(movements[i].destination.position, movements[i].time));
            if(i == (movements.Length - 1)){
                sequence.Join(DOFadeCanvas(1f, 0f, movements[i].time));
            }
        }

        sequence.Play().OnComplete(DestroyUI);
    }

    public void FadeAndDestroy(float time){
        DOFadeCanvas(1f, 0f, time).OnComplete(DestroyUI);
    }

    private void DestroyUI(){
        Destroy(gameObject);
    }

    private Tween DOFadeCanvas(float start, float end, float time){
        return DOTween.To(x => canvasGroup.alpha = x, start, end, time);
    }
}
