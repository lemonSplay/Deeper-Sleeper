using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuSCript : MonoBehaviour
{

    public RectTransform IMAGE;

    public void ExitACtion()
    {
        Application.Quit();
    }

    public void StartAction()
    {
        IMAGE.gameObject.SetActive(true);
        LeanTween.alpha(IMAGE, 1F, 0.5F).setOnComplete(acac => {
            SceneManager.LoadScene(1);

        });

    }
}
