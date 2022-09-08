using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;
using UnityEngine.UI;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [SerializeField] public GameObject dlgRunner;
    
    public float walkSpeed = 3.8f;

    [HideInInspector] public PlayeInputActions playerInputActions;
    [SerializeField] LayerMask dialogueLayer;
    DialogueRunner dialogueRunner;
    DialogueUI dialogueUI;
    public UITransitions gameManager;


    [Header("UI Display")]
    public Button ContinueButton;
    public GameObject UIDisplay;
    public TMP_Text TextDisplay;
    public string InteractibleNameText;
    private RectTransform UiDisplayRectTransform;

    [HideInInspector]
    public bool isInRange = false;

    public Rigidbody rb;

    private float angle;
    Quaternion targetRotation;
    public float turningSpeed = 1f;
    public Animator anim;
    public GameObject gfx;

    Vector3 movement;
    
    void Awake()
    {
        UiDisplayRectTransform = UIDisplay.GetComponent<RectTransform>();
        playerInputActions = new PlayeInputActions();
        playerInputActions.Player.Move.performed += context => Move(context.ReadValue<Vector2>());
        playerInputActions.Player.Move.canceled += context => Move(context.ReadValue<Vector2>());
        playerInputActions.Player.Fire.performed += context => CheckForNearbyNPC();
        playerInputActions.Player.Remember.performed += context => gameManager.Remember();
        playerInputActions.Player.Fire.performed += context => gameManager.StopRememberGameplay();
    }

    void Start()
    {
        dialogueRunner = dlgRunner.GetComponent<DialogueRunner>();
        dialogueUI = dlgRunner.GetComponent<DialogueUI>();
    }

    public void CheckForNearbyNPC()
    {
        if (dialogueRunner.IsDialogueRunning == true || gameManager.neutralEnding)
        {
            if (ContinueButton.IsActive() == true)
            {
                dialogueUI.MarkLineComplete();
            }
            return;
        }
        
        LeanTween.alpha(UiDisplayRectTransform, 0f, .1f).setOnComplete(eventa => {
            UIDisplay.SetActive(false);
        });
        Collider[] target = Physics.OverlapSphere(this.transform.position, 0.29f, dialogueLayer);
        foreach (Collider t in target)
        {
            dialogueRunner.StartDialogue(t.gameObject.GetComponentInParent<objDialogueScript>().talkToNode);
        }
    }

    void Move(Vector2 moveVector)
    {
        if (dialogueRunner.IsDialogueRunning == true || gameManager.neutralEnding)
        {
            anim.SetFloat("moving", 0f);
            movement = Vector3.zero;
            return;
        }
        movement =new Vector3(moveVector.x, 0f,moveVector.y);
        anim.SetFloat("moving",Mathf.Abs(movement.x)+Mathf.Abs(movement.z));
    }
    
    void CalculateDirection()
    {
        angle = Mathf.Atan2(movement.x, movement.z);
        angle = Mathf.Rad2Deg * angle;
    }

    void Rotate()
    {
        targetRotation = Quaternion.Euler(0, angle, 0);
        gfx.transform.rotation = Quaternion.Slerp(gfx.transform.rotation, targetRotation, turningSpeed * Time.deltaTime);
    }
    
    public void ShowInteractBtn()
    {
        UIDisplay.SetActive(true);
        LeanTween.alpha(UiDisplayRectTransform, 1f, .1f);
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "hasDialogue")
        {
            InteractibleNameText = col.gameObject.GetComponent<objDialogueScript>().characterName;
            TextDisplay.SetText(InteractibleNameText);
            UiDisplayRectTransform.LeanAlpha(0f, 0f);
            UIDisplay.SetActive(true);
            LeanTween.alpha(UiDisplayRectTransform, 1f, .1f);
            isInRange = true;
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.tag == "hasDialogue")
        {
            LeanTween.alpha(UiDisplayRectTransform, 0f, .1f).setOnComplete(eventa => {
                UIDisplay.SetActive(false);
            });
            isInRange = false;

        }
    }

    private void OnEnable()
    {
        playerInputActions.Enable();
    }
    
    private void OnDisable()
    {
        playerInputActions.Disable();
    }
    
    void Update()
    {

        if (gameManager.neutralEnding)
        {
            anim.SetBool("ending", true);
        }else
        CalculateDirection();
    }

    private void FixedUpdate()
    {
        if(movement != Vector3.zero)
        {
            Rotate();
        }
        if (dialogueRunner.IsDialogueRunning != true)
        {
            rb.MovePosition(rb.position + movement * walkSpeed * Time.fixedDeltaTime);
        }
    }
}
