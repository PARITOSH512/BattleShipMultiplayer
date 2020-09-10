using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Application;


public class MainMenuManager : MonoBehaviour
{

    public static MainMenuManager Inst;

    public Button[] menuButtons;
    public GameObject menuBackground;
    public GameObject sliderGroup;
    public GameObject socketIOPrefab;
    public GameObject SoundObject;
    public Sprite On, Off;

    [Header("Planel")]
    public GameObject Removeads;

    private bool isAnimationOver = true;

    private void Awake()
    {
        if (Inst == null)
        {
            Inst = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        Removeads.SetActive(false);
        SoundOnOff();
        isAnimationOver = false;

        StartCoroutine(ShowMenuPanel());

        Debug.Log("OBJ: " + GameObject.FindGameObjectWithTag("SocketIO"));

        if (GameObject.FindGameObjectWithTag("SocketIO") == null)
        {
            Instantiate(socketIOPrefab);
            print("-==-OBJ=-null=>");
            DontDestroyOnLoad(GameObject.FindGameObjectWithTag("SocketIO"));
        }

    }

    IEnumerator ShowMenuPanel()
    {
        yield return new WaitForSeconds(1);     // wait for slider animation completion
        Animation animation = menuBackground.GetComponent<Animation>();
        animation.Play();

        yield return new WaitForSeconds(0.2f);     // wait for menu background animation completion
        StartCoroutine(ShowAllButtons(0));      // start after menu background animation

        yield return new WaitForSeconds(0.5f * menuButtons.Length);
        isAnimationOver = true;
    }

    IEnumerator ShowAllButtons(int index)
    {
        yield return new WaitForSeconds(0.05f);  // wait for half second and start animation of button

        Animation animation = menuButtons[index].GetComponent<Animation>();
        animation.Play();

        if ((index + 1) < menuButtons.Length)
        {
            StartCoroutine(ShowAllButtons(index + 1));
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnCreateGameClicked()
    {
       // GoogleMobileAdsDemoScript.instance.ShowInterstitial(ChangeScene(6).ToString());
        StartCoroutine(ChangeScene(6));
        AudioManager.instance.Btn();
    }

    public void OnJoinGameClicked()
    {
       // GoogleMobileAdsDemoScript.instance.ShowInterstitial(ChangeScene(5).ToString());
        StartCoroutine(ChangeScene(5));
        AudioManager.instance.Btn();
    }

    public void OnSingleGameClicked()
    {
       // GoogleMobileAdsDemoScript.instance.ShowInterstitial(ChangeScene(3).ToString());
        StartCoroutine(ChangeScene(3));      // Loads SingleGameScene
        AudioManager.instance.Btn();
    }

    public void OnTutorialClicked()
    {
     //   GoogleMobileAdsDemoScript.instance.ShowInterstitial(ChangeScene(8).ToString());
        StartCoroutine(ChangeScene(8));     //Tutorial
        GameManager.Inst.gameMode = GameMode.GamePlaying;
        AudioManager.instance.Btn();
    }

    //Will exit app
    public void OnExitGameClicked()
    {
        AudioManager.instance.Btn();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        UnityEngine.Application.Quit();
#endif
    }

    IEnumerator ChangeScene(int sceneIndex)
    {
        Animator animator = sliderGroup.GetComponent<Animator>();
        animator.Play("CloseSliderAnimation");

        yield return new WaitForSeconds(1);

        SceneManager.LoadScene(sceneIndex);
    }
    public void Sound()
    {
        if (GameManager.Inst.isSound)
        {
            GameManager.Inst.isSound = false;
        }
        else
        {
            GameManager.Inst.isSound = true;
        }
        SoundOnOff();
        AudioManager.instance.BgSound();
    }
    internal void SoundOnOff()
    {
        if (GameManager.Inst.isSound)
        {
            SoundObject.GetComponent<Image>().sprite = On;

        }
        else
        {
            SoundObject.GetComponent<Image>().sprite = Off;

        }
    }
    public void Buy_Remove_Ads_Click()
    {
       // IAPManager.instance.BuyRemoveAds();
    }
    public void Click_Remove_Add()
    {
        Removeads.SetActive(true);
    }

}
