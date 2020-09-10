using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioSource BgAs;
    public AudioSource ExpAs;
    public AudioSource BtnAs;

    public AudioClip bgClip;
    public AudioClip expClip;
    public AudioClip btnClip;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

    }

    private void Start()
    {
        BgSound();
    }
    public void BgSound()
    {
        if (GameManager.Inst.isSound)
        {
            print("<Color><b>!♦ Sound is Working Fine ♦!</b></Color>");
            BgAs.clip = bgClip;
            BgAs.Play();
            BgAs.playOnAwake = true;
            BgAs.loop = true;
            PlayerPrefs.SetInt("Sound", 0);
        }
        else
        {
            print("<Color><b>!♦ Mute ♦!</b></Color>");
            BgAs.Stop();
            PlayerPrefs.SetInt("Sound", 1);
        }
    }
    public void PlayExp()
    {
        if (GameManager.Inst.isSound)
        {
            ExpAs.clip = expClip;
            ExpAs.PlayOneShot(expClip);
        }
        else
        {
            ExpAs.Stop();
        }
    }
    public void Btn()
    {
        if (GameManager.Inst.isSound)
        {
            BtnAs.clip = btnClip;
            BtnAs.PlayOneShot(btnClip);
        }
        else
        {
            BtnAs.Stop();
        }
    }
}
