using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class InGameMenu : MonoBehaviour
{
    public static InGameMenu Instance {get; private set;}

    [SerializeField] private bool beginOnPause = true;

    public bool InPause {get; private set;}

    private CanvasGroup canvasGroup;

    private void Awake()
    {
        if(Instance != null){
            if(Instance != this){
                Destroy(gameObject);
            }

            return;
        }

        Instance = this;
        canvasGroup = GetComponent<CanvasGroup>();

        if(beginOnPause){
            DisplayMenu();
        }
        else{
            HideMenu();
        }
    }


    void Update()
    {
        if(!InPause){
            return;
        }

        if(CustomInputManager.Instance.GetForwardButton()){
            HideMenu();
        }
    }

    public void DisplayMenu(){
        Time.timeScale = 0f;
        canvasGroup.alpha = 1f;
        InPause = true;
    }

    public void HideMenu(){
        Time.timeScale = 1f;
        InPause = false;
        canvasGroup.alpha = 0f;
    }
}
