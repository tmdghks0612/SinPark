using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialButton : MonoBehaviour
{
    // button action to deactivate current image
    public void HideImage()
    {
        gameObject.SetActive(false);
    }
    // button action to end tutorial and start ingame
    public void EndTutorial()
    {
        LoadingSceneManager.LoadScene("DefaultIngame");
    }
}
