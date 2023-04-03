using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManeger : MonoBehaviour
{
    public static SoundManeger Instance;
    public enum PlayList{
        Pacman_StartSound = 0,
        Pacman_WonSound = 1,
        Pacman_Chomp = 2,
        Pacman_Death = 3,
        Pacman_EatGhost = 4,
        Pacman_Power = 5,
    }
    public List<AudioClip> audioLists;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource effectsSource;

    public Sprite audioOn_Image;
    public Sprite audioOFF_Image;
    public Image audioButtonImage;
    
    bool mute = false;

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else if(Instance != this)
        {
            Destroy(this);
        }
        DontDestroyOnLoad(this);
    }
    public void PlaySound(PlayList soundType)
    {
        effectsSource.PlayOneShot(audioLists[(int)soundType]);
    }

    public void PlayMusic(PlayList soundType)
    {
        musicSource.clip = audioLists[(int)soundType];
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void PauseMusic()
    {
        musicSource.Pause();
    }

    public void ContinueMusic()
    {
        musicSource.Play();
    }

    public void Audio()
    {
        mute = !mute;
        musicSource.mute = mute;
        effectsSource.mute = mute;
        if(mute == true)
        {
            audioButtonImage.sprite = audioOFF_Image;
        }
        else
        {
            audioButtonImage.sprite = audioOn_Image;   
        }
    }
}
