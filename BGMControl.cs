using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class BGMControl : MonoBehaviour
{
    [SerializeField]
    private AudioClip MainMenuBGM;
    [SerializeField]
    private AudioClip IngameBGM;
    AudioSource audioSource;

    private void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
        audioSource = gameObject.GetComponent<AudioSource>();

        // set and play music
        audioSource.clip = MainMenuBGM;
        audioSource.loop = true;
        audioSource.Play();
    }

    public void ChangeBGMToMainMenu()
    {
        if(audioSource.clip == MainMenuBGM)
        {
            return;
        }
        else
        {
            // stop current BGM
            audioSource.Stop();

            //Change and play MainMenu BGM
            audioSource.clip = MainMenuBGM;
            audioSource.Play();
        }
    }

    public void ChangeBGMToIngame()
    {
        if (audioSource.clip == IngameBGM)
        {
            return;
        }
        else
        {
            // stop current BGM
            audioSource.Stop();

            //Change and play Ingame BGM
            audioSource.clip = IngameBGM;
            audioSource.Play();
        }
    }
}
