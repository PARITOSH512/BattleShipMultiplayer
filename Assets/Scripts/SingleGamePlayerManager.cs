using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Application.Modal;

public class SingleGamePlayerManager : MonoBehaviour
{


    [Header("All player button will be covered in this")]
    public Button[] playerButtons;

    [Header("All team button will be covered in this")]
    public Button[] teamButtons;

    [Header("All rows for animation purpose")]
    public GameObject[] rowsToAnimate;

    [Space(10)]

    public GameObject sliderGroup;
    public Button startButton;

    private int selectedNonePlayers;
    private int selectedHumanPlayers;
    public int selectedComputerPlayers;

    private void Awake()
    {

    }
    // Start is called before the first frame update
    void Start()
    {

        StartCoroutine(ShowAllButtons(0));      //start animation to show rows animatically
        CountPlayers();
    }
    public void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            BackButtonClick();
        }
    }
    public void BackButtonClick()
    {
        AudioManager.instance.Btn();
        //if game is not joined and clicked back
        StartCoroutine(ChangeScene(2));
    }
    /**
     * used to show all rows animatically  
     */
    IEnumerator ShowAllButtons(int index)
    {
        if (index != 0)
            yield return new WaitForSeconds(0.05f);  // wait for half second and start animation of button
        else
            yield return new WaitForSeconds(1.5f);

        Animation animation = rowsToAnimate[index].GetComponent<Animation>();
        animation.Play();

        if ((index + 1) < rowsToAnimate.Length)
        {
            StartCoroutine(ShowAllButtons(index + 1));
        }
    }

    /**
     * Will be called when user will tap on center button
     * displayed for 4 players
     **/
    public void CenterButtonClicked(int clickedIndex)
    {
        try
        {


            string textToSet = playerButtons[clickedIndex].GetComponentInChildren<TMP_Text>().text;
            string teamToSet = teamButtons[clickedIndex].GetComponentInChildren<TMP_Text>().text;

            switch (textToSet)
            {
                case "human":
                    textToSet = "computer";
                    teamToSet = teamToSet != "0" ? teamToSet : "1";
                    break;
                case "computer":
                    textToSet = "none";
                    teamToSet = "0";
                    break;
                case "none":
                    if (selectedHumanPlayers > 0)
                    {
                        textToSet = "computer";
                        teamToSet = "1";
                        if (selectedHumanPlayers > 0)
                        {
                            textToSet = "computer";
                            teamToSet = "2";

                        }
                    }
                    else
                    {
                        textToSet = "human";
                        teamToSet = "1";
                        //teamToSet = "0";
                    }
                    break;
            }

            playerButtons[clickedIndex].GetComponentInChildren<TMP_Text>().text = textToSet;
            teamButtons[clickedIndex].GetComponentInChildren<TMP_Text>().text = teamToSet;

            print("<color=green><b>textToSet </b></color>" + " <color>" + textToSet + "</color>");

            CountPlayers();
        }
        catch (System.Exception ex)
        {
            print("<color>Exception </color>" + " <color>" + ex.ToString() + "</color>");
        }
    }

    /**
     * will handle click of all right team arrow buttons  
     **/
    public void RightButtonClicked(int clickedIndex)
    {
        AudioManager.instance.Btn();
        string textToSet = teamButtons[clickedIndex].GetComponentInChildren<TMP_Text>().text;
        string playerText = playerButtons[clickedIndex].GetComponentInChildren<TMP_Text>().text;

        int current = int.Parse(textToSet);

        if (current == 4)
            current = 0;
        else
            current += 1;

        textToSet = current.ToString();

        if (playerText == "none" && current == 1)
        {
            playerButtons[clickedIndex].GetComponentInChildren<TMP_Text>().text = "computer";
        }
        if (current == 0)
        {
            playerButtons[clickedIndex].GetComponentInChildren<TMP_Text>().text = "none";
        }

        //if (clickedIndex == 0)
        //{
        //    playerButtons[clickedIndex].GetComponentInChildren<TMP_Text>().text = "human";
        //    textToSet = "1";
        //}

        if (selectedHumanPlayers > 0 && selectedComputerPlayers < 2)
        {
            if (playerButtons[clickedIndex].GetComponentInChildren<TMP_Text>().text == "computer")
            {
                textToSet = "2";
            }
        }

        teamButtons[clickedIndex].GetComponentInChildren<TMP_Text>().text = textToSet;

        CountPlayers();
    }

    /**
     * will check validation to start game
     * and will enable and disable start button
     * accordingly   
     **/
    private void CountPlayers()
    {
        selectedNonePlayers = 0;
        selectedHumanPlayers = 0;
        selectedComputerPlayers = 0;

        bool isAllInOneTeam = true;
        bool isAllSamePlayer = true;
        bool isAllTeamAsZero = true;

        int previousTeam = int.Parse(teamButtons[0].GetComponentInChildren<TMP_Text>().text);
        string previousPlayer = playerButtons[0].GetComponentInChildren<TMP_Text>().text;

        for (int i = 0; i < playerButtons.Length; i++)
        {
            string textToSet = playerButtons[i].GetComponentInChildren<TMP_Text>().text;
            string teamNumber = teamButtons[i].GetComponentInChildren<TMP_Text>().text;

            //print("<color=red>previousTeam </color>" + " <color>" + previousTeam + "</color>");
            //print("<color=red>previousPlayer </color>" + " <color>" + previousPlayer + "</color>");
            //print("<color=red>teamNumber </color>" + " <color>" + teamNumber + "</color>");

            if (previousTeam != int.Parse(teamNumber) && textToSet != "none")
                isAllInOneTeam = false;

            if (0 != int.Parse(teamNumber) && textToSet != "none")
                isAllTeamAsZero = false;


            if (previousPlayer != textToSet && textToSet != "none")
                isAllSamePlayer = false;


            switch (textToSet)
            {
                case "human":
                    selectedHumanPlayers++;
                    //print("<color=red>selectedHumanPlayers </color>" + " <color>" + selectedHumanPlayers + "</color>");
                    break;
                case "computer":
                    selectedComputerPlayers++;
                    //print("<color>selectedComputerPlayers </color>" + " <color>" + selectedComputerPlayers + "</color>");
                    break;
                case "none":
                    selectedNonePlayers++;
                    //print("<color>selectedNonePlayers </color>" + " <color>" + selectedNonePlayers + "</color>");
                    break;
            }
        }

        print("<color=green><b>isAllInOneTeam </b></color>" + " <color>" + isAllInOneTeam + "</color>");
        print("<color=green><b>isAllSamePlayer </b></color>" + " <color>" + isAllSamePlayer + "</color>");
        print("<color=green><b>isAllTeamAsZero </b></color>" + " <color>" + isAllTeamAsZero + "</color>");
        print("<color=green><b>selectedHumanPlayers </b></color>" + " <color>" + selectedHumanPlayers + "</color>");
        print("<color=red><b>selectedNonePlayers </b></color>" + " <color>" + selectedNonePlayers + "</color>");
        print("<color=yellow><b>selectedComputerPlayers </b></color>" + " <color>" + selectedComputerPlayers + "</color>");

        for (int i = 0; i < playerButtons.Length; i++)
        {
            string teamNumber = teamButtons[i].GetComponentInChildren<TMP_Text>().text;
            //if (teamButtons[i].GetComponentInChildren<TMP_Text>().text)
            //{

            //}
        }

        if (isAllInOneTeam || isAllSamePlayer || (selectedHumanPlayers < 1) || isAllTeamAsZero || selectedComputerPlayers < 1)
            startButton.interactable = false;
        else
            startButton.interactable = true;
    }

    /**
     * will handle click of start button  
     **/
    public void StartButtonClicked()
    {
        AudioManager.instance.Btn();
        Dictionary<string, Dictionary<string, string>> playerDetails = new Dictionary<string, Dictionary<string, string>>();

        Dictionary<string, string> obj = new Dictionary<string, string>();
        obj.Add(playerButtons[0].GetComponentInChildren<TMP_Text>().text, teamButtons[0].GetComponentInChildren<TMP_Text>().text);
        playerDetails.Add("1", obj);

        Dictionary<string, string> obj1 = new Dictionary<string, string>();
        obj1.Add(playerButtons[1].GetComponentInChildren<TMP_Text>().text, teamButtons[1].GetComponentInChildren<TMP_Text>().text);
        playerDetails.Add("2", obj1);

        Dictionary<string, string> obj2 = new Dictionary<string, string>();
        obj2.Add(playerButtons[2].GetComponentInChildren<TMP_Text>().text, teamButtons[2].GetComponentInChildren<TMP_Text>().text);
        playerDetails.Add("3", obj2);

        Dictionary<string, string> obj3 = new Dictionary<string, string>();
        obj3.Add(playerButtons[3].GetComponentInChildren<TMP_Text>().text, teamButtons[3].GetComponentInChildren<TMP_Text>().text);
        playerDetails.Add("4", obj3);

        //Debug.Log("PLAYERDETAILS: " + playerDetails);

        PlayerDetails details = new PlayerDetails();

        details.allUsers = new PlayerData[selectedComputerPlayers + selectedHumanPlayers];

        int i = 0;

        foreach (string key in playerDetails.Keys)
        {
            //Debug.Log("KEY: " + key);
            foreach (string innerKey in playerDetails[key].Keys)
            {
                //Debug.Log(innerKey + " INNER VALUE: " + playerDetails[key][innerKey]);
                PlayerData data = new PlayerData();
                data.name = key;
                data.type = innerKey;
                data.team = playerDetails[key][innerKey];

                if (innerKey != "none")
                {
                    switch (key)
                    {
                        case "1":
                            data.color = "red";
                            details.allUsers[i] = data;
                            break;
                        case "2":
                            data.color = "yellow";
                            details.allUsers[i] = data;
                            break;
                        case "3":
                            data.color = "green";
                            details.allUsers[i] = data;
                            break;
                        case "4":
                            data.color = "blue";
                            details.allUsers[i] = data;
                            break;
                    }
                    i++;
                }
            }
        }

        PlayerPrefs.SetString("singlePlayerGame", JsonUtility.ToJson(details));
        StartCoroutine(ChangeScene(4));
    }

    /**
     * will perform slider close animation and
     * then will change the scene
     **/
    IEnumerator ChangeScene(int sceneIndex)
    {
        Animator animator = sliderGroup.GetComponent<Animator>();
        animator.Play("CloseSliderAnimation");

        yield return new WaitForSeconds(1);

        SceneManager.LoadScene(sceneIndex);
    }

    
}
