using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class objDialogueScript : MonoBehaviour
{

    public string characterName = "";

    public string talkToNode = "";

    public DialogueRunner dialogueRunner;

    [Header("Optional")]
    public YarnProgram scriptToLoad;

    void Start()
    {
        if (scriptToLoad != null)
        {
            dialogueRunner.Add(scriptToLoad);
        }
    }
}