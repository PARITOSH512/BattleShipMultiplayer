using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Application;

public class SplashHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ChangeScreen());
    }

    IEnumerator ChangeScreen()
    {
        yield return new WaitForSeconds(2.40f);
        Player player = new Player();
        string playerName = player.GetUserName();

        //Debug.Log("PLAYER_NAME: " + playerName);

        //playerName = "";

        if (playerName != "")
        {
            SceneManager.LoadScene(2);
        }
        else
        {
            SceneManager.LoadScene(1);
        }

    }
}
