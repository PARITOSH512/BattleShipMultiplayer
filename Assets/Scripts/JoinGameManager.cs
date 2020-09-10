using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Networking;
using Application;
using Application.Modal;
using Applications.Utils;
using SocketIO;

public class JoinGameManager : MonoBehaviour
{
    public static JoinGameManager instance;

    [Header("Objects to be animated")]
    public GameObject joinGameHeader;
    public GameObject enterCodeTitle;
    public GameObject enterCodeField;

    [Header("Objects to be animated later")]
    public GameObject[] playerRows;

    [Header("Objects to be hide till code entered")]
    public GameObject playerSectionTitle;
    public GameObject playerSection;

    [Space(15)]

    public GameObject sliderGroup;
    public GameObject errorPanel;
    public TMP_Text errorText;
    public GameObject startButton;
    public GameObject confirmPanel;
    public TMP_Text confirmText;
    public TMP_Text confirmOkButton;
    public SocketIOComponent socketIOComponent;

    [Header("All rows of player")]
    public GameObject[] playerRowsText;

    private bool isAnimationOver = true;    // will be managed to complete animation and then continue other things
    private string myGameCode = "";
    private bool isErrorVisible = false;
    private bool joinedTeam = false;
    private bool isAdminExited = false;

    private void Awake()
    {


    }
    // Start is called before the first frame update
    void Start()
    {
        GameManager.Inst.isInbackground = false;

        if (GameObject.FindGameObjectWithTag("SocketIO") != null && socketIOComponent == null)
            socketIOComponent = GameObject.FindGameObjectWithTag("SocketIO").GetComponent<SocketIOComponent>();

        isAnimationOver = false;
        GameManager.Inst.isNotYetJoin = false;
        //hide player showing section and title first
        foreach (GameObject game in playerRows)
            game.SetActive(false);

        //start animation after slider gets open
        StartCoroutine(StartAnimation());

        enterCodeField.GetComponent<TMP_InputField>().onEndEdit.AddListener(EndEditing);
        errorPanel.SetActive(false);

        confirmPanel.SetActive(false);

        //socket setup
        socketIOComponent.On("leftGame", (SocketIOEvent obj) =>
        {
            confirmText.text = obj.data.ToDictionary()["message"];
            confirmOkButton.text = "OK";

            isAdminExited = true;
            StartCoroutine(StartConfirmPanelAnimation());
        });

        socketIOComponent.On("playerJoinedRoom", (SocketIOEvent obj) =>
         {
             JoinGame joinGame = JsonUtility.FromJson<JoinGame>(obj.data.ToString());

             for (int i = 0; i < joinGame.team.Length; i++)
             {
                 playerRowsText[i].GetComponentInChildren<TMP_Text>().text = joinGame.team[i].userName;
                 GameManager.Inst.Lobby_id = joinGame.team[i].lobby_id;
                 string roleToSet = "friend";

                 if (joinGame.team[i].role == "admin")
                     roleToSet = "admin";

                 if (joinGame.team[i].deviceId == SystemInfo.deviceUniqueIdentifier && joinGame.team[i].role == "friend")
                     roleToSet = "you";

                 playerRowsText[i].transform.GetChild(1).gameObject.SetActive(true);
                 playerRowsText[i].transform.GetChild(1).gameObject.GetComponent<TMP_Text>().text = roleToSet;

                 print("<Color><b> !♦ playerJoinedRoom ♦! </b></Color>");
             }
         });

        socketIOComponent.On("playerLeftGame", (SocketIOEvent obj) =>
        {
            JoinGame joinGame = JsonUtility.FromJson<JoinGame>(obj.data.ToString());
            Debug.Log("obj: " + joinGame.status);

            //set joined players to rows
            for (int i = joinGame.team.Length; i < playerRowsText.Length; i++)
            {
                playerRowsText[i].GetComponentInChildren<TMP_Text>().text = "pending...";
                GameManager.Inst.isNotYetJoin = false;
                playerRowsText[i].transform.GetChild(1).gameObject.SetActive(false);
            }

        });

    }

    IEnumerator StartAnimation()
    {
        yield return new WaitForSeconds(0.5f);

        joinGameHeader.GetComponent<Animation>().Play();
        enterCodeTitle.GetComponent<Animation>().Play();
        enterCodeField.GetComponent<Animation>().Play();

        yield return new WaitForSeconds(0.5f);

        isAnimationOver = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            BackButtonClicked();
        }

        //socketIOComponent.On("StratGame", (SocketIOEvent obj) =>
        //{
        //    Debug.Log("obj: " + obj.data.ToString());
        //    SceneManager.LoadScene("MultiPlayerGamePlayScene");
        //});
    }

    public void BackButtonClicked()
    {
        AudioManager.instance.Btn();
        if (isAnimationOver && !isErrorVisible)
        {
            //will detect back button click (android)
            if (!joinedTeam)
                StartCoroutine(ChangeScene(2));
        }

        //game is joined and press back make confirmation popup visible
        if (joinedTeam && !confirmPanel.activeInHierarchy && isAnimationOver)
            StartCoroutine(StartConfirmPanelAnimation());

        //game is joined and confirmation popup is visible, hide that popup
        ConfirmPanelCloseButton();

        //if error popup is visible then hide it.
        if (isErrorVisible)
            CloseButton();


    }

    public void ConfirmPanelCloseButton()
    {
        if (confirmPanel.activeInHierarchy && isAnimationOver && !isAdminExited)
            StartCoroutine(CloseConfirmPanelAnimation(false));

        //if admin exited, popup shown, and press back, change scene
        if (confirmPanel.activeInHierarchy && isAnimationOver && isAdminExited)
            LeaveEvenIfJoined();
    }

    public void LeaveEvenIfJoined()
    {
        Dictionary<string, string> pairs = new Dictionary<string, string>();
        pairs.Add("code", myGameCode);
        pairs.Add("deviceId", SystemInfo.deviceUniqueIdentifier);
        GameManager.Inst.isNotYetJoin = false;
        socketIOComponent.Emit("leftGame", new JSONObject(pairs));
        StartCoroutine(CloseConfirmPanelAnimation(true));       //close confirm panel
    }

    IEnumerator StartConfirmPanelAnimation()
    {
        isAnimationOver = false;

        confirmPanel.SetActive(true);
        yield return new WaitForSeconds(0.6f);
        isAnimationOver = true;
    }

    IEnumerator CloseConfirmPanelAnimation(bool isGoBack)
    {
        isAnimationOver = false;

        confirmPanel.GetComponent<Animator>().Play("ConfirmPanelAnimationOut");

        yield return new WaitForSeconds(0.6f);

        confirmPanel.SetActive(false);
        isAnimationOver = true;

        if (isGoBack)
        {
            StartCoroutine(ChangeScene(2));
        }
    }

    //will be called when user end to edit
    public void EndEditing(string text)
    {
        if (text.Length > 1)
        {
            StartCoroutine(CheckCodeAndJoin());
            CheckAndJoinGame();
        }
        else
        {
            //Show Error PopUP Here
            if (text.Length != 0)
            {
                GameObject.Find("ErrorPopUp").transform.localScale = Vector3.one;
            }
        }
    }
    public void CloseErrorPopup()
    {
        GameObject.Find("ErrorPopUp").transform.localScale = Vector3.zero;
        enterCodeField.GetComponent<TMP_InputField>().text = "";
    }
    //will send code to server and will fetch team for same using socket (it's used)
    public void CheckAndJoinGame()
    {
        myGameCode = enterCodeField.GetComponent<TMP_InputField>().text;

        Player player = new Player();

        Dictionary<string, string> inputData = new Dictionary<string, string>();
        inputData.Add("code", myGameCode);
        inputData.Add("userName", player.GetUserName());
        inputData.Add("deviceId", SystemInfo.deviceUniqueIdentifier);

        socketIOComponent.Emit("joinGame", new JSONObject(inputData), (JSONObject obj) =>
        {
            string jsonString = obj[0].ToString();
            Debug.Log("joinGame  CHECKING 11: " + jsonString);
            JoinGame joinGame = JsonUtility.FromJson<JoinGame>(jsonString);

            if (joinGame.status == true)
            {
                GameManager.Inst.isNotYetJoin = true;

                joinedTeam = true;

                enterCodeField.GetComponent<TMP_InputField>().interactable = false;

                //set joined players to rows
                for (int i = 0; i < joinGame.team.Length; i++)
                {
                    playerRowsText[i].GetComponentInChildren<TMP_Text>().text = joinGame.team[i].userName;

                    string roleToSet = "friend";

                    if (joinGame.team[i].role == "admin")
                        roleToSet = "admin";

                    if (joinGame.team[i].deviceId == SystemInfo.deviceUniqueIdentifier && joinGame.team[i].role == "friend")
                        roleToSet = "you";

                    playerRowsText[i].transform.GetChild(1).gameObject.SetActive(true);
                    playerRowsText[i].transform.GetChild(1).gameObject.GetComponent<TMP_Text>().text = roleToSet;

                    //show player showing section and title first
                    foreach (GameObject game in playerRows)
                        game.SetActive(true);

                    StartCoroutine(AnimateRows(0));

                }

                // here timer 
                //timer = 15;
                //InvokeRepeating("Starttimer", 1f, 1f);
            }
            else
            {
                GameManager.Inst.isNotYetJoin = false;
                //if api sends error, show error panel
                isErrorVisible = true;
                errorPanel.SetActive(true);
                errorPanel.GetComponent<Animator>().Play("ErrorAnimationIn");
                errorText.text = joinGame.message;
            }

        });
    }

    //will send code to server and will fetch team for same (not used)
    IEnumerator CheckCodeAndJoin()
    {
        myGameCode = enterCodeField.GetComponent<TMP_InputField>().text;

        Player player = new Player();

        string apiUrl = ApiUtil.BASE_URL + "checkCode/" + myGameCode + "/" + player.GetUserName() + "/" + SystemInfo.deviceUniqueIdentifier;

        UnityWebRequest www = UnityWebRequest.Get(apiUrl);

        yield return www.SendWebRequest();      // hold for API response

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            string jsonString = www.downloadHandler.text;
            Debug.Log("CHECKING 11: " + jsonString);
            JoinGame joinGame = JsonUtility.FromJson<JoinGame>(jsonString);

            if (joinGame.status == true)
            {
                joinedTeam = true;

                enterCodeField.GetComponent<TMP_InputField>().interactable = false;

                //set joined players to rows
                for (int i = 0; i < joinGame.team.Length; i++)
                {
                    playerRowsText[i].GetComponentInChildren<TMP_Text>().text = joinGame.team[i].userName;

                    string roleToSet = "friend";

                    if (joinGame.team[i].role == "admin")
                        roleToSet = "admin";

                    if (joinGame.team[i].deviceId == SystemInfo.deviceUniqueIdentifier && joinGame.team[i].role == "friend")
                        roleToSet = "you";

                    playerRowsText[i].transform.GetChild(1).gameObject.SetActive(true);
                    playerRowsText[i].transform.GetChild(1).gameObject.GetComponent<TMP_Text>().text = roleToSet;
                }

                //show player showing section and title first
                foreach (GameObject game in playerRows)
                    game.SetActive(true);

                StartCoroutine(AnimateRows(0));
            }
            else
            {
                //if api sends error, show error panel
                isErrorVisible = true;
                errorPanel.SetActive(true);
                errorPanel.GetComponent<Animator>().Play("ErrorAnimationIn");
                errorText.text = joinGame.message;
            }
        }
    }

    IEnumerator AnimateRows(int index)
    {
        Animation animation = playerRows[index].GetComponent<Animation>();
        animation.Play();

        yield return new WaitForSeconds(0.05f);

        if ((index + 1) < playerRows.Length)
            StartCoroutine(AnimateRows(index + 1));
    }

    //will hide error panel on click of close button
    public void CloseButton()
    {
        isAnimationOver = false;
        enterCodeField.GetComponent<TMP_InputField>().text = "";
        errorPanel.GetComponent<Animator>().Play("ErrorAnimationOut");
        StartCoroutine(DeactivateErrorPanel());

        if (GameManager.Inst.isInbackground)
        {
            SceneManager.LoadScene("MainMenuScreen");
            AudioManager.instance.Btn();
        }

    }

    //after .5 sec, disable panel else you won't be able to click any buttons
    IEnumerator DeactivateErrorPanel()
    {
        yield return new WaitForSeconds(0.5f);
        errorPanel.SetActive(false);
        isAnimationOver = true;
        isErrorVisible = false;
    }

    IEnumerator ChangeScene(int sceneIndex)
    {
        isAnimationOver = false;
        Animator animator = sliderGroup.GetComponent<Animator>();
        animator.Play("CloseSliderAnimation");

        yield return new WaitForSeconds(1);

        SceneManager.LoadScene(sceneIndex);
    }

    public void StartGame()
    {
        print("<Color><b> startGame </b></Color>");
        AudioManager.instance.Btn();
        //socketIOComponent.Emit("startGame", (JSONObject obj) =>
        //{
        //    Debug.Log("startGame" + obj.ToString());
        //});
    }

    float timer = 15f;
    void Starttimer()
    {
        timer--;
        GameObject.Find("StartButtonText").GetComponent<TMP_Text>().text = "Game Start in " + timer + " seconds";
        if (timer < 0)
        {
            GameObject.Find("StartButtonText").GetComponent<TMP_Text>().text = "Game Start in few seconds";
        }
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            //Application on pause mode
            print("<Color><b> Application on pause mode </b></Color>");
        }
        else
        {
            //Application foreground
            GameManager.Inst.isInbackground = true;
            print("<Color><b> Application foreground </b></Color>");
            isErrorVisible = true;
            errorPanel.SetActive(true);
            errorPanel.GetComponent<Animator>().Play("ErrorAnimationIn");
            errorText.text = "you lost the game or left in between battle";
        }

    }
}
