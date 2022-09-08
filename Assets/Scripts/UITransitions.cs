using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Yarn.Unity;
using Yarn;
using UnityEngine.SceneManagement;


public class UITransitions : MonoBehaviour
{
    

    [Header("UI Display")]
    public GameObject UIDisplay;
    public TMP_Text TextDisplay;

    public GameObject Button;
    public TMP_Text ButtonText;

    //public GameObject darker;

    private RectTransform UiDisplayRectTransform;
    private RectTransform ButtonRectTransform;

    //private RectTransform darkerRectTransform;

    [Header("Scene fade in/out")]
    public GameObject fadeOut;
    public GameObject fadeIn;
    public TMP_Text displayDateText;
    public float FadeTransitionDelayTime=2f;

    [Header("Other")]
    public InMemoryVariableStorage varStorage;
    private DialogueRunner dlgRunnerScript;
    public GameObject dialogueRunner;

    [Header("Gameplay")]
    public GameObject word1;
    private RectTransform rTword1;
    public GameObject word2;
    private RectTransform rTword2;

    public TMP_Text memoryWord;
    public RectTransform rTnWord;
    public RectTransform rTbWord;

    public float answearTime = 4f;
    private float nextAppearTime;
    private bool hasRemembered=false;

    private int currentWordType = 1;//0-n, 1-bad
    private string currentWord="default";

    private int missedNeutral = 0;

    [Header("lostGame")]
    public RectTransform lostGameImage;
    public RectTransform neutralGameimage;
    private bool lostGame = false;
    public RectTransform missEffect;
    public RectTransform restartButton;
    public RectTransform restartButton2;
    public RectTransform exitButton;
    public RectTransform exitButton2;

    public bool neutralEnding = false;


    public List<string> neutralMemory= new List<string>();
    public List<string> badMemory = new List<string>();

    void Awake()
    {
        UiDisplayRectTransform = UIDisplay.GetComponent<RectTransform>();
        UiDisplayRectTransform.LeanAlpha(0f, 0f);
        ButtonRectTransform = Button.GetComponent<RectTransform>();
        ButtonRectTransform.LeanAlpha(0f, 0f);
        //darkerRectTransform = darker.GetComponent<RectTransform>();
        //darkerRectTransform.LeanAlpha(0f, 0f);
    }
    
    void Start()
    {
        dlgRunnerScript = dialogueRunner.GetComponent<DialogueRunner>();
        rTword1 = word1.GetComponent<RectTransform>();
        rTword2 = word2.GetComponent<RectTransform>();
    }

    void Update()
    {
        CheckDayFinish();
        
        if ((int)varStorage.GetValue("$day").AsNumber < 4)
        {
            CheckTimer();
        }
    }

    [Header("Transitions")]
    public string[] transitionTexts = new string[10];

    void CheckDayFinish()
    {
        if (varStorage.GetValue("$dayHasEnded").AsBool)
        {
            varStorage.SetValue("$dayHasEnded", false);
            if ((int)varStorage.GetValue("$day").AsNumber == 5)
            {
                FadeTransitionDelayTime = 10f;
                neutralEnding = true;
                lostGame = true;
                fadeOut.SetActive(true);
                LeanTween.alpha(fadeOut.GetComponent<RectTransform>(), 1f, 6f).setOnComplete(action =>
                {
                    LeanTween.alpha(neutralGameimage, 0f, 0f);
                    neutralGameimage.gameObject.SetActive(true);
                    LeanTween.alpha(neutralGameimage, 1f, 0.5f).setOnComplete(acsac => {

                        restartButton2.gameObject.SetActive(true);
                        LeanTween.alpha(restartButton2, 1f, 0.3f).setOnComplete(ac2 =>
                        {
                            exitButton2.gameObject.SetActive(true);
                            LeanTween.alpha(exitButton2, 1f, 0.3f);
                        });
                    });
                });
            }
            else
            {

            FadeTransition(transitionTexts[(int)varStorage.GetValue("$day").AsNumber], FadeTransitionDelayTime);
                if ((int)varStorage.GetValue("$day").AsNumber == 2)
                    gameObject.GetComponent<AudioManagerScript>().SetLoop("addedP3", false);
                if ((int)varStorage.GetValue("$day").AsNumber == 4)
                    gameObject.GetComponent<AudioManagerScript>().SetLoop("addedP2", false);
            }
        }
    }

    void CheckTimer()
    {
        if (lostGame)
        {
            return;
        }

        if (dlgRunnerScript.IsDialogueRunning == true)
        {
            return;
        }

        if (Time.time < nextAppearTime && hasRemembered==false)
        {
            return;
        }
        else
        {

            if (varStorage.GetValue("$dayHasEnded").AsBool)
            {
                currentWordType = 1;
                ResetGpUI();
            }

            nextAppearTime = Time.time + answearTime;
            
            if(currentWordType==0 && hasRemembered==false)
            {
                if (dlgRunnerScript.IsDialogueRunning == true)
                {
                    ResetGpUI();
                    return;
                }
                //apha 22
                missedNeutral++;
                LeanTween.alpha(missEffect, 0f, 0f);
                missEffect.gameObject.SetActive(true);
                LeanTween.alpha(missEffect, 0.22f, 0.1f).setOnComplete(act => {
                    LeanTween.alpha(missEffect, 0f, 0.1f);
                    missEffect.gameObject.SetActive(false);
                });
                if (missedNeutral > 2)
                {
                    LostGame();
                }
            }
            hasRemembered = false;
            
            ResetGpUI();

            word1.SetActive(true);

            LeanTween.alpha(rTword1, 1f, 0.3f).setOnComplete(action3 =>
            {
                if (dlgRunnerScript.IsDialogueRunning == true || hasRemembered)
                {
                    ResetGpUI();
                    return;
                }

                word2.SetActive(true);
                LeanTween.alpha(rTword2, 1f, 0.3f).setOnComplete(action4 =>
                {
                    if (dlgRunnerScript.IsDialogueRunning == true || hasRemembered)
                    {
                        ResetGpUI();
                        return;
                    }

                    currentWordType = Random.Range(0, 2);
                    if (currentWordType == 0)
                    {
                        //n
                        currentWord = neutralMemory[Random.Range(0, neutralMemory.Count)];
                        rTnWord.gameObject.SetActive(true);
                        LeanTween.alpha(rTnWord, 1f, 0.3f);
                    }
                    else
                    {
                        //b
                        currentWord = badMemory[Random.Range(0, badMemory.Count)];
                        rTbWord.gameObject.SetActive(true);
                        LeanTween.alpha(rTbWord, 1f, 0.3f);
                    }
                    memoryWord.SetText(currentWord);
                    memoryWord.gameObject.SetActive(true);
                });
            });
        }
    }


    public void StopRememberGameplay()
    {
        if (dlgRunnerScript.IsDialogueRunning)
        {
        currentWordType = 1;
        ResetGpUI();
        }
    }

    public void Remember()
    {
        if (dlgRunnerScript.IsDialogueRunning == true)
        {
            ResetGpUI();
            nextAppearTime = Time.time + answearTime;
            return;
        }
        hasRemembered = true;
        ResetGpUI();
        if (currentWordType == 1)
        {
            missedNeutral++;
            LeanTween.alpha(missEffect, 0f, 0f);
            missEffect.gameObject.SetActive(true);
            LeanTween.alpha(missEffect, 0.22f, 0.1f).setOnComplete(act => {
                LeanTween.alpha(missEffect, 0f, 0.1f);
                missEffect.gameObject.SetActive(false);
            });
            if (missedNeutral > 2)
            {
                LostGame();
            }
        }
    }

    private void LostGame()
    {
        lostGame = true;
        fadeOut.SetActive(true);
        LeanTween.alpha(fadeOut.GetComponent<RectTransform>(), 1f, 1f).setOnComplete(action =>
        {
            LeanTween.alpha(lostGameImage, 0f, 0f);
            lostGameImage.gameObject.SetActive(true);
            LeanTween.alpha(lostGameImage, 1f, 0.5f).setOnComplete(acsac => {

                restartButton.gameObject.SetActive(true);
                LeanTween.alpha(restartButton, 1f, 0.3f).setOnComplete(ac2 =>
                {
                    exitButton.gameObject.SetActive(true);
                    LeanTween.alpha(exitButton, 1f, 0.3f);
                });
            });
        });
    }

    public void ExitACtion()
    {
        Application.Quit();
    }

    public void RestartAction()
    {
        SceneManager.LoadScene(1);
    }

    private void ResetGpUI()
    {
        word1.SetActive(false);
        word2.SetActive(false);
        memoryWord.gameObject.SetActive(false);
        rTnWord.gameObject.SetActive(false);
        rTbWord.gameObject.SetActive(false);

        LeanTween.alpha(rTword1, 0f, 0f);
        LeanTween.alpha(rTword2, 0f, 0f);
        LeanTween.alpha(rTbWord, 0f, 0f);
        LeanTween.alpha(rTnWord, 0f, 0f);
    }
    
    public void FadeIn()
    {
        LeanTween.alpha(UiDisplayRectTransform, 1f, .1f);
        LeanTween.alpha(ButtonRectTransform, 1f, .1f);
        //LeanTween.alpha(darkerRectTransform, 0.33f, .2f);
    }

    public void FadeOut()
    {
        LeanTween.alpha(UiDisplayRectTransform, 0f, 0f);
        LeanTween.alpha(ButtonRectTransform, 0f, 0f);
        //LeanTween.alpha(darkerRectTransform, 0f, .2f);
    }
    
    public void FadeInScene()
    {
        fadeIn.SetActive(true);
        LeanTween.alpha(fadeIn.GetComponent<RectTransform>(), 0f, 1f).setOnComplete(actionDeux =>
        {
            fadeIn.SetActive(false);
            fadeIn.GetComponent<RectTransform>().LeanAlpha(1f, 0f);
        });
    }

    public void FadeOutScene()
    {
        fadeOut.SetActive(true);
        LeanTween.alpha(fadeOut.GetComponent<RectTransform>(), 1f, 1f).setOnComplete(action =>
        {
            fadeOut.SetActive(false);
            fadeOut.GetComponent<RectTransform>().LeanAlpha(1f, 0f);
        });
    }
    
    public void FadeTransition(string displayText, float transitionDelay)
    {
        fadeOut.SetActive(true);
        LeanTween.alpha(fadeOut.GetComponent<RectTransform>(), 1f, 1f).setOnComplete(action =>
        {
            fadeOut.SetActive(false);
            fadeOut.GetComponent<RectTransform>().LeanAlpha(0f, 0f);
            fadeIn.SetActive(true);
            displayDateText.gameObject.SetActive(true);
            displayDateText.SetText(displayText);
            LeanTween.delayedCall(transitionDelay, delay => { FadeInScene(); displayDateText.gameObject.SetActive(false); });
        });
    }
}
