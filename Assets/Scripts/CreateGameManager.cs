using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using Application;
using UnityEngine.SceneManagement;
using Application.Modal;
using Applications.Utils;
using SocketIO;

public class CreateGameManager : MonoBehaviour
{
    public static CreateGameManager inst;

    [Header("Objects to be animated")]
    public GameObject joinGameHeader;
    public GameObject enterCodeTitle;
    public GameObject enterCodeField;
    public GameObject playerSectionTitle;

    public string ShareUrl;
    public GameObject[] playerRows;

    [Header("All rows of player")]
    public Button[] playerRowsText;

    [Space(15)]

    public GameObject sliderGroup;
    public GameObject confirmPanel;
    public TMP_Text confirmText;
    public SocketIOComponent socketIOComponent;

    private bool isAnimationOver = true;    // will be managed to complete animation and then continue other things
    private bool isJoinedGame = false;
    private string myGameCode = "";

    private void Awake()
    {
        inst = this;

        isAnimationOver = false;


        GameManager.Inst.isNotYetJoin = false;
        GameManager.Inst.isInbackground = false;

        if (GameObject.FindGameObjectWithTag("SocketIO") != null && socketIOComponent == null)
            socketIOComponent = GameObject.FindGameObjectWithTag("SocketIO").GetComponent<SocketIOComponent>();
        else
            print("-=-=socketIO-=error-=->");
        Debug.Log("socketIO: " + socketIOComponent);

        //enterCodeField.GetComponent<TMP_InputField>().onEndEdit.AddListener(EndEditing);
        enterCodeField.GetComponent<TMP_InputField>().interactable = false;

        //get code from server
        GetCodeFromServer();

        //start animation after slider gets open
        StartCoroutine(StartAnimation());

        //get current user name (who will be admin) and set on first row
        Player player = new Player();

        playerRowsText[0].GetComponentInChildren<TMP_Text>().text = player.GetUserName();
        playerRowsText[0].interactable = false;

        //deactive confirmation panel and change message of that
        confirmPanel.SetActive(false);
        confirmText.text = "Your friends will not be able join team using this code. Are you sure you want to leave ?";

        //Socket setup
        socketIOComponent.On("playerJoinedRoom", (SocketIOEvent obj) =>
        {
            JoinGame joinGame = JsonUtility.FromJson<JoinGame>(obj.data.ToString());
            print("playerJoinedRoom + data.ToString()" + obj.data.ToString());
            Debug.Log("obj: " + joinGame.status);
            GameManager.Inst.isNotYetJoin = true;
            //set joined players to rows
            for (int i = 1; i < joinGame.team.Length; i++)
            {
                playerRowsText[i].GetComponentInChildren<TMP_Text>().text = joinGame.team[i].userName;
                GameManager.Inst.Lobby_id = joinGame.team[i].lobby_id;
                string roleToSet = "friend";

                if (joinGame.team[i].role == "admin")
                    roleToSet = "admin";

                if (joinGame.team[i].deviceId == SystemInfo.deviceUniqueIdentifier && joinGame.team[i].role == "friend")
                    roleToSet = "you";

                playerRowsText[i].transform.GetChild(2).gameObject.SetActive(false);
                playerRowsText[i].transform.GetChild(1).gameObject.SetActive(true);
                playerRowsText[i].transform.GetChild(1).gameObject.GetComponent<TMP_Text>().text = roleToSet;

                Dictionary<string, Dictionary<string, string>> playerDetails = new Dictionary<string, Dictionary<string, string>>();

                Dictionary<string, string> obj1 = new Dictionary<string, string>();
                obj1.Add((i + 1).ToString(), (i + 1).ToString());
                playerDetails.Add((i + 1).ToString(), obj1);
                PlayerDetails details = new PlayerDetails();

                print("playerJoinedRoom joinGame.team.Length" + joinGame.team.Length);

                details.allUsers = new PlayerData[joinGame.team.Length];

                int j = 0;

                foreach (string key in playerDetails.Keys)
                {
                    Debug.Log("KEY: " + key);
                    foreach (string innerKey in playerDetails[key].Keys)
                    {
                        //Debug.Log(innerKey + " INNER VALUE: " + playerDetails[key][innerKey]);
                        PlayerData data = new PlayerData();
                        data.name = key;
                        data.type = innerKey;
                        data.team = playerDetails[key][innerKey];
                        print("playerJoinedRoom >>>>>>>>>>>>>>>>>>" + innerKey);
                        Debug.Log("playerJoinedRoom ♥ KEY: " + key);
                        if (innerKey != "none" && j <= joinGame.team.Length)
                        {
                            switch (key)
                            {
                                case "1":
                                    data.color = "red";
                                    details.allUsers[j] = data;
                                    break;
                                case "2":
                                    data.color = "yellow";
                                    details.allUsers[j] = data;
                                    break;
                                case "3":
                                    data.color = "green";
                                    details.allUsers[j] = data;
                                    break;
                                case "4":
                                    data.color = "blue";
                                    details.allUsers[j] = data;
                                    break;
                            }
                            if (j != joinGame.team.Length)
                            {
                                j++;
                                print("j " + j);
                            }
                        }
                    }
                }
                print("♥♥♥" + details.allUsers[j]);
                PlayerPrefs.SetString("singlePlayerGame", JsonUtility.ToJson(details));
            }
            // here timer 
            //timer = 15;
            //InvokeRepeating("Starttimer", 1f, 1f);
            //SceneManager.LoadScene("MultiPlayerGamePlayScene");

            GameObject.Find("StartButtonText").GetComponent<TMP_Text>().text = "You can Start Game";
        });

        socketIOComponent.On("playerLeftGame", (SocketIOEvent obj) =>
        {
            JoinGame joinGame = JsonUtility.FromJson<JoinGame>(obj.data.ToString());
            Debug.Log("obj: " + joinGame.status);

            //set joined players to rows
            for (int i = joinGame.team.Length; i < playerRowsText.Length; i++)
            {
                playerRowsText[i].GetComponentInChildren<TMP_Text>().text = "add friend";
                GameManager.Inst.isNotYetJoin = false;

                playerRowsText[i].transform.GetChild(2).gameObject.SetActive(true);
                playerRowsText[i].transform.GetChild(1).gameObject.SetActive(false);
            }

        });

        //make socket not destroy
        //DontDestroyOnLoad(socketIOComponent);

        //get code from server
        //GetCodeFromServer();      from here to Awake top

    }
    // Start is called before the first frame update
    void Start()
    {

    }

    public void Update()
    {
        if (Input.GetKey(KeyCode.Escape) && isAnimationOver)
        {
            BackButtonClick();
        }
    }

    public void BackButtonClick()
    {


        AudioManager.instance.Btn();
        //if game is not joined and clicked back
        if (!isJoinedGame)
            StartCoroutine(ChangeScene(2));

        //game is created and clicked back
        if (isJoinedGame && !confirmPanel.activeInHierarchy && isAnimationOver)
            StartCoroutine(StartConfirmPanelAnimation());

        //popup is visible and clicked back (for android)
        if (isJoinedGame && confirmPanel.activeInHierarchy && isAnimationOver)
            StartCoroutine(CloseConfirmPanelAccount(false));


    }

    public void ClosePopupAndGoBack()
    {
        AudioManager.instance.Btn();
        Dictionary<string, string> pairs = new Dictionary<string, string>();
        pairs.Add("code", myGameCode);

        //socketIOComponent.Emit("adminDidLeft");

        socketIOComponent.Emit("adminDidLeft", new JSONObject(pairs), (JSONObject obj) =>
        {
            Debug.Log(obj.ToString());
        });
        StartCoroutine(CloseConfirmPanelAccount(true));
    }

    //will start animation to show exit popup
    IEnumerator StartConfirmPanelAnimation()
    {
        isAnimationOver = false;
        confirmPanel.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        isAnimationOver = true;
    }

    //Will start animation to close popup
    IEnumerator CloseConfirmPanelAccount(bool isGoBack)
    {
        isAnimationOver = false;
        confirmPanel.GetComponent<Animator>().Play("ConfirmPanelAnimationOut");

        yield return new WaitForSeconds(0.5f);

        confirmPanel.SetActive(false);

        if (isGoBack)
            StartCoroutine(ChangeScene(2));
        else
            isAnimationOver = true;
    }

    //will start animation to show all required views
    IEnumerator StartAnimation()
    {
        yield return new WaitForSeconds(0.5f);

        joinGameHeader.GetComponent<Animation>().Play();
        enterCodeTitle.GetComponent<Animation>().Play();
        enterCodeField.GetComponent<Animation>().Play();

        yield return new WaitForSeconds(0.4f);

        StartCoroutine(ShowAllButtons(0));

        yield return new WaitForSeconds(0.3f * playerRows.Length);
        isAnimationOver = true;
    }

    //will animate buttons
    IEnumerator ShowAllButtons(int index)
    {
        yield return new WaitForSeconds(0.05f);  // wait for half second and start animation of button

        Animation animation = playerRows[index].GetComponent<Animation>();
        animation.Play();

        if ((index + 1) < playerRows.Length)
        {
            StartCoroutine(ShowAllButtons(index + 1));
        }
    }

    //will allow to show share popup
    public void AddFriend(int index)
    {
        //playerRowsText[index].interactable = false;
        //playerRowsText[index].GetComponentInChildren<TMP_Text>().text = "invited...";

        new NativeShare().SetTitle("Invite friends").SetText("Hello there!!\nInviting you to play an amazing game \"Battle Warship\" " + ShareUrl + " with me. Here is the code using which you can join the game " + myGameCode).Share();
    }

    void GetCodeFromServer()
    {
        Debug.Log("CONNECTED: " + socketIOComponent.IsConnected);
        Player player = new Player();

        Dictionary<string, string> inputData = new Dictionary<string, string>();
        inputData.Add("userName", player.GetUserName());
        inputData.Add("deviceId", SystemInfo.deviceUniqueIdentifier);

        socketIOComponent.Emit("getUniqueCode", new JSONObject(inputData), (JSONObject obj) =>
        {
            string jsonString = obj[0].ToString();
            print("-=-=-=jsonString-=-=>" + jsonString);
            MyCodeModel myCode = JsonUtility.FromJson<MyCodeModel>(jsonString);
            enterCodeField.GetComponent<TMP_InputField>().text = myCode.myCode;

            isJoinedGame = true;

            myGameCode = myCode.myCode;
            GameManager.Inst.Lobby_id = myGameCode;

        });
    }

    //will get unique code from server (not used)
    IEnumerator GetCodeFromServer1()
    {
        Player player = new Player();

        UnityWebRequest www = UnityWebRequest.Get(ApiUtil.BASE_URL + "getUniqueCode/" + player.GetUserName() + "/" + SystemInfo.deviceUniqueIdentifier);
        //Debug.Log("URL: " + ApiUtil.BASE_URL + "getUniqueCode/" + player.GetUserName());

        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            string jsonString = www.downloadHandler.text;
            MyCodeModel myCode = JsonUtility.FromJson<MyCodeModel>(jsonString);
            enterCodeField.GetComponent<TMP_InputField>().text = myCode.myCode;

            isJoinedGame = true;

            myGameCode = myCode.myCode;
        }
    }

    //will start close animation of scene and will change the scene
    IEnumerator ChangeScene(int sceneIndex)
    {
        isAnimationOver = false;

        Animator animator = sliderGroup.GetComponent<Animator>();
        animator.Play("CloseSliderAnimation");

        yield return new WaitForSeconds(1);

        SceneManager.LoadScene(sceneIndex);
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


    public void StartGame()
    {
        if (GameManager.Inst.isNotYetJoin)
        {
            print("<Color><b> startGame </b></Color>");
            AudioManager.instance.Btn();
            socketIOComponent.Emit("startGame", (JSONObject obj) =>
            {
                Debug.Log("startGame" + obj.ToString());
            });
        }
    }


}
