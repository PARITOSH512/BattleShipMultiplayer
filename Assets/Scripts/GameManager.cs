using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;
using UnityEngine.SceneManagement;
//using Application.Modal;
using Applications.Utils;
using TMPro;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Inst;

    public string Myid;
    public string MyColor;
    public int MyteamCode;
    public string Lobby_id="0";

    public SocketIOComponent socketIOComponent;

    public JSONObject SaveData;

    public bool isSound = true;

    public bool isNotYetJoin;

    public GameMode gameMode;
    public string liveLink;

    public int bigIcelandLimit;
    public int smallIcelandLimit;

    private Rect _safeArea;
    //public int attackCounter;
    public bool isInbackground;//is in back ground while join game

    public string ClientID;

    public bool appispause = false;
    public bool allowupdate = false;
    private void Awake()
    {

        //UnityEngine.Application.runInBackground = true;

        if (_safeArea != Screen.safeArea)
        {
            _safeArea = Screen.safeArea;
            print("-=-=-=All is not good-=-=-=-=>" + Screen.safeArea.ToString());
        }
        else
        {
            print("-=-=-=All is good-=-=-=-=>");
        }
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Inst = this;

        //PlayerPrefs.DeleteAll();

        DontDestroyOnLoad(this.gameObject);
        socketIOComponent = GameObject.FindGameObjectWithTag("SocketIO").GetComponent<SocketIOComponent>();

        int S = PlayerPrefs.GetInt("Sound");
        if (S == 0)
        {
            isSound = true;
        }
        else
        {
            isSound = false;
        }
    }





    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(checkInternetConnection((isConnected) =>
        {
            // handle connection status here
            print("<Color><b>!♦ checkInternetConnection ♦!</b></Color>" + isConnected);
        }));

        socketIOComponent.On("startGame", (SocketIOEvent obj) =>
        {
            print("<Color><b>!♦ startGame ♦!</b></Color>" + obj);
            if (isNotYetJoin/*SceneManager.GetActiveScene().name.Equals("JoinGameScene") || SceneManager.GetActiveScene().name.Equals("CreateGameScene")*/)
            {
                if (GamePlayManagerMultiPlayer.inst == null && !isInbackground)
                {
                    SaveData = obj.data;
                    print("SAVEDATA♦ " + SaveData);
                    SceneManager.LoadScene("MultiPlayerGamePlayScene");
                }
                Debug.Log("obj: " + obj.data.ToString());
            }
        });

        socketIOComponent.On("register", (Eventio) =>
        {
            string id = Eventio.data["id"].ToString().RemoveQuotes();
            Debug.LogFormat("Our Client's Id ({0})", id);

            ClientID = id; // setting the Client id of the player
            if (Lobby_id != "0" && Lobby_id!=null)
            {
                Dictionary<string, string> pairs = new Dictionary<string, string>();
                pairs.Add("playerid", GameManager.Inst.ClientID);
                pairs.Add("lobby_id", GameManager.Inst.Lobby_id);
                socketIOComponent.Emit("Getplanedata", new JSONObject(pairs));
                //appispause = false;
                GameManager.Inst.allowupdate = true;
            }
        });

        #region
        /////////////////////////////////////////////////////////////////////////////////////////
        //socketIOComponent.On("playerJoinedRoom", (SocketIOEvent obj) =>
        //{
        //    print("playerJoinedRoom + data.ToString()" + obj.data.ToString());
        //    JoinGame joinGame = JsonUtility.FromJson<JoinGame>(obj.data.ToString());
        //    Debug.Log("obj: " + joinGame.status);

        //    //set joined players to rows
        //    for (int i = 1; i < joinGame.team.Length; i++)
        //    {
        //        CreateGameManager.inst.playerRowsText[i].GetComponentInChildren<TMP_Text>().text = joinGame.team[i].userName;

        //        string roleToSet = "friend";

        //        if (joinGame.team[i].role == "admin")
        //            roleToSet = "admin";

        //        if (joinGame.team[i].deviceId == SystemInfo.deviceUniqueIdentifier && joinGame.team[i].role == "friend")
        //            roleToSet = "you";

        //        CreateGameManager.inst.playerRowsText[i].transform.GetChild(2).gameObject.SetActive(false);
        //        CreateGameManager.inst.playerRowsText[i].transform.GetChild(1).gameObject.SetActive(true);
        //        CreateGameManager.inst.playerRowsText[i].transform.GetChild(1).gameObject.GetComponent<TMP_Text>().text = roleToSet;

        //        Dictionary<string, Dictionary<string, string>> playerDetails = new Dictionary<string, Dictionary<string, string>>();

        //        Dictionary<string, string> obj1 = new Dictionary<string, string>();
        //        obj1.Add((i + 1).ToString(), (i + 1).ToString());
        //        playerDetails.Add((i + 1).ToString(), obj1);
        //        PlayerDetails details = new PlayerDetails();

        //        print("playerJoinedRoom joinGame.team.Length" + joinGame.team.Length);

        //        details.allUsers = new PlayerData[joinGame.team.Length];

        //        int j = 0;

        //        foreach (string key in playerDetails.Keys)
        //        {
        //            Debug.Log("KEY: " + key);
        //            foreach (string innerKey in playerDetails[key].Keys)
        //            {
        //                //Debug.Log(innerKey + " INNER VALUE: " + playerDetails[key][innerKey]);
        //                PlayerData data = new PlayerData();
        //                data.name = key;
        //                data.type = innerKey;
        //                data.team = playerDetails[key][innerKey];
        //                print("playerJoinedRoom >>>>>>>>>>>>>>>>>>" + innerKey);
        //                Debug.Log("playerJoinedRoom ♥ KEY: " + key);
        //                if (innerKey != "none" && j <= joinGame.team.Length)
        //                {
        //                    switch (key)
        //                    {
        //                        case "1":
        //                            data.color = "red";
        //                            details.allUsers[j] = data;
        //                            break;
        //                        case "2":
        //                            data.color = "yellow";
        //                            details.allUsers[j] = data;
        //                            break;
        //                        case "3":
        //                            data.color = "green";
        //                            details.allUsers[j] = data;
        //                            break;
        //                        case "4":
        //                            data.color = "blue";
        //                            details.allUsers[j] = data;
        //                            break;
        //                    }
        //                    if (j != joinGame.team.Length)
        //                    {
        //                        j++;
        //                        print("j " + j);
        //                    }
        //                }
        //            }
        //        }
        //        print("♥♥♥" + details.allUsers[j]);
        //        PlayerPrefs.SetString("singlePlayerGame", JsonUtility.ToJson(details));
        //    }

        //    //SceneManager.LoadScene("MultiPlayerGamePlayScene");
        //});
        #endregion
    }
    IEnumerator checkInternetConnection(Action<bool> action)
    {
        WWW www = new WWW("http://google.com");
        yield return www;
        if (www.error != null)
        {
            action(false);
        }
        else
        {
            action(true);
        }
    }
    // Update is called once per frame
    void Update()
    {

    }

}


[System.Serializable]
public enum GameMode
{
    None,
    GamePlaying,
    GameOver
}