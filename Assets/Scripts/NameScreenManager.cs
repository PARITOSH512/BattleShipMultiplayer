using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Application;

public class NameScreenManager : MonoBehaviour
{
    public Animator sliderAnimator;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void OnStartButtonClick(TMP_Text value)
    {

        string userName = value.text;

        if(userName.Length > 1)
        {
            Player player = new Player();
            player.SaveUserName(value.text);
            sliderAnimator.Play("CloseSliderAnimation");
            StartCoroutine(ChangeScreen());
        }
    }

    IEnumerator ChangeScreen()
    {
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene(2);
    }
}
