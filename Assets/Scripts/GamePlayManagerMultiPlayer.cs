using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Application.Modal;
using Application;
using Applications.Utils;
using TMPro;
using UnityEngine.SceneManagement;
using SocketIO;

[System.Serializable]
public class PlanetData
{
    public List<int> Mylimit = new List<int>();
}

[System.Serializable]
public class planeposdata
{
    public string playerid;
    public List<Pos> pos = new List<Pos>();
}

[System.Serializable]
public class Pos
{
    public string id;
    public float X;
    public float Y;
    public float Z;
}

public class GamePlayManagerMultiPlayer : MonoBehaviour
{
    public static GamePlayManagerMultiPlayer inst;
    public GameObject[] spawnPointContainers;

    public GameObject mainSpawnContainer;

    private int numberOfPlayers;
    private int numberOfIcelandToGive;

    private int[] activatedIcelands;

    [Header("Pause Screen")]
    public GameObject PauseScreen;
    public GameObject SoundObject;

    [Header("Sprite")]
    public Sprite On, Off;
    public Sprite PasueSp, PlaySp;

    [Header("Player header objects")]
    public GameObject redPlayer;
    public GameObject yellowPlayer;
    public GameObject greenPlayer;
    public GameObject bluePlayer;

    public GameObject planePrefab;

    [Header("WinnerCount List Of Object")]
    public List<GameObject> Allplanet = new List<GameObject>();

    public List<int> TeamOne = new List<int>();
    public List<int> TeamTwo = new List<int>();
    public List<int> TeamThree = new List<int>();
    public List<int> TeamFour = new List<int>();
    public List<int> ComputerPlayerList = new List<int>();

    [Header("Attacking Plane")]
    public Dictionary<string, AttackingPlane> serverObject = new Dictionary<string, AttackingPlane>();

    public int TotalPlayer;
    public SocketIOComponent socketIOComponent;

    public Image WinPlane;
    public Image LostPlane;

    public bool StartGamebool = false;

    public List<int> Mylimt = new List<int>();

    

    private void Awake()
    {
        inst = this;
        PauseScreen = GameObject.Find("PauseScreen");
        SoundObject = GameObject.Find("SoundStatus");

        if (GameObject.FindGameObjectWithTag("SocketIO") != null && socketIOComponent == null)
            socketIOComponent = GameObject.FindGameObjectWithTag("SocketIO").GetComponent<SocketIOComponent>();
    }

    // Start is called before the first frame update
    void Start()
    {
        SoundOnOff();

        GameUtil.selectedIceLand = new int[20];

        socketIOComponent.On("Attack", (SocketIOEvent obj) =>
        {
            Debug.Log("<Color=green>Receiver obj: </Color>" + obj.data.ToString());
            JSONObject DataAttack = new JSONObject(obj.data.ToString());

            print("DataAttack  " + DataAttack);
            print("isAttackModeOn  " + DataAttack.GetField("isAttackModeOn").ToString());
            print("selectedIceLand  " + DataAttack.GetField("selectedIceLand").ToString());
            print("iceLandIndex  " + DataAttack.GetField("iceLandIndex").ToString());

            try
            {
                string str1 = DataAttack.GetField("isAttackModeOn").ToString().Trim('"');
                bool isA = bool.Parse(str1);
                string str2 = DataAttack.GetField("selectedIceLand").ToString().Trim('"');
                int Si = int.Parse(str2);
                string str3 = DataAttack.GetField("iceLandIndex").ToString().Trim('"');
                int ii = int.Parse(str3);
                string str4 = DataAttack.GetField("totalPlane").ToString().Trim('"');
                int ToatalPlane = int.Parse(str4);
                string str5 = DataAttack.GetField("plane_id").ToString().Trim('"');

                print("<Color><b> ♥ totalPlane ♥ </b></Color>" + ToatalPlane);

                print("isA ♦" + isA + " Si ♦" + Si + " ii ♦" + ii);

                ServerData(isA, Si, ii, ToatalPlane ,str5 , Vector3.zero);
            }
            catch (System.Exception e)
            {
                print("EE" + e);
                throw;
            }

        });

        socketIOComponent.On("updateiceland", (SocketIOEvent obj) =>
        {

            JSONObject Newdata = new JSONObject(obj.data.GetField("Newplanedata1").ToString());
            //print("-=-NewData=-=->" + Newdata);

            for (int i = 0; i < Allplanet.Count; i++)
            {
                //int planeToBeSet = int.Parse(obj.data.GetField("Newplanedata")[i].ToString());
                //int plane1 = int.Parse(obj.data.GetField("Newplanedata")[i].GetField("totalPlane").ToString());
                //print("-=-=-dataaaa=-=>" + Newdata[i]);
                try
                {
                    string str1 = Newdata[i].GetField("isFree").ToString().Trim('"');
                    bool isF = bool.Parse(str1);
                    string str2 = Newdata[i].GetField("isStatic").ToString().Trim('"');
                    bool isS = bool.Parse(str2);
                    string Tc = Newdata[i].GetField("TeamCode").ToString().Trim('"');
                    //int  Tc = int.Parse(str3);
                    string str4 = Newdata[i].GetField("totalPlane").ToString().Trim('"');
                    int ToatalPlane = int.Parse(str4);
                    string color = Newdata[i].GetField("Color").ToString().Trim('"');
                    string playertype = Newdata[i].GetField("PlayerType").ToString().Trim('"');
                    //string str5 = Newdata[i].GetField("plane_id").ToString().Trim('"');
                    Allplanet[i].GetComponent<Iceland>().Updatetotalplane(ToatalPlane,isS,isF,Tc,color,playertype);

                }
                catch (System.Exception e)
                {
                    print("EE" + e);
                    throw;
                }

                //Allplanet[i].GetComponent<Iceland>().Updatetotalplane(planeToBeSet);
                
            }
            
        });

        socketIOComponent.On("Checkwin", (SocketIOEvent obj) =>
         {
             AddData();
         });

        socketIOComponent.On("Updateplanedata", (SocketIOEvent obj)=>
        {
            print("<Color><b>-=-=-=-=-=-=-=-=> </b></Color>" + obj.data.ToString());

            if (GameManager.Inst.allowupdate)
            {
                print("<Color><b>-=-=-=-=-=-=-=-=> </b></Color>" + obj.data.GetField("NewPlanedata1").ToString());
                JSONObject Newdata = new JSONObject(obj.data.GetField("NewPlanedata1").ToString());
                
                UpdatePlanedata(Newdata);
                GameManager.Inst.allowupdate = false;
            }
        });

        socketIOComponent.On("Getplanedata", (SocketIOEvent obj) =>
         {
             print("-=player data were we have to send the data" + obj.data.GetField("playerid").ToString());
             tranfromdata(obj.data.GetField("playerid").ToString());
         });

        JSONObject Data = GameManager.Inst.SaveData;
        print("SAVE DATA IS HERE ♥☺♦♣ " + Data);
        int index = int.Parse(Data.GetField("spawncontainerindex").ToString());
        JSONObject PlayingData = new JSONObject(Data.GetField("playingData").ToString());
        JSONObject PlayersData = new JSONObject(Data.GetField("playerData").ToString());
        print("PlayingData +++" + PlayingData);
        print("PlayersData +++" + PlayersData);
        StartGame(index, PlayingData, PlayersData);

       // print("Allplanet" + Allplanet.Count);

        PlanetData planetData = new PlanetData();
        for (int i = 0; i < Allplanet.Count; i++)
        {
            planetData.Mylimit.Add(Allplanet[i].GetComponent<Iceland>().MyLimit);
        }
        socketIOComponent.Emit("MyLimit", new JSONObject(JsonUtility.ToJson(planetData)));
          print("MYLIMIT -=-=->" + new JSONObject(JsonUtility.ToJson(planetData)));

    }


    public void StartGame(int spawncontainerindex, JSONObject PlayingData, JSONObject PlayerData)
    {
        GameManager.Inst.gameMode = GameMode.GamePlaying;
        print("STARTGAME CALL");
        print("spawncontainerindex =>" + spawncontainerindex);
        spawnPointContainers[spawncontainerindex].SetActive(true);

        mainSpawnContainer = spawnPointContainers[spawncontainerindex];

        GameUtil.staticSpawnContainer = mainSpawnContainer;

        int totalChild = mainSpawnContainer.transform.childCount;


        for (int k = 0; k < PlayingData[0].GetField("activethisindex").Count; k++)
        {
            int a = int.Parse(PlayingData[0].GetField("activethisindex")[k].ToString());
            //print("a" + a);
            int planeToBeSet = int.Parse(PlayingData[0].GetField("planevalue")[k].ToString());
            //print("planeToBeSet" + planeToBeSet);

            if (!mainSpawnContainer.transform.GetChild(a).gameObject.activeInHierarchy)
            {
                if (planeToBeSet >= mainSpawnContainer.transform.GetChild(a).gameObject.GetComponent<Iceland>().MyLimit)
                {
                    planeToBeSet = mainSpawnContainer.transform.GetChild(a).gameObject.GetComponent<Iceland>().MyLimit;
                }
                mainSpawnContainer.transform.GetChild(a).gameObject.GetComponent<Iceland>().totalPlane = planeToBeSet;
                mainSpawnContainer.transform.GetChild(a).gameObject.GetComponent<Iceland>().isStatic = true;
                mainSpawnContainer.transform.GetChild(a).gameObject.GetComponent<Iceland>().isFree = true;
                mainSpawnContainer.transform.GetChild(a).gameObject.GetComponent<Iceland>().playerType = "none";
                mainSpawnContainer.transform.GetChild(a).gameObject.GetComponent<Iceland>().iceLandIndex = a;
                mainSpawnContainer.transform.GetChild(a).gameObject.SetActive(true);
                Allplanet.Add(mainSpawnContainer.transform.GetChild(a).gameObject);
            }
        }

        for (int l = 0; l < PlayerData.Count; l++)
        {
            TotalPlayer = PlayerData.Count;

            print("<color><b> Player Data  </b></color>" + PlayerData[l].ToString());

            print("<Color><b> My id is </b></Color>" + PlayerData[l].GetField("_id").ToString().Trim('"'));

            GameManager.Inst.Myid = PlayerData[l].GetField("_id").ToString().Trim('"');

            int playerindex = int.Parse(PlayerData[l].GetField("playerindex").ToString());
            print("Playerindex  " + playerindex);

            int planeToBeSet = int.Parse(PlayerData[l].GetField("Playerplane").ToString());
            print("planeToBeSet " + planeToBeSet);

            string teamCode = PlayerData[l].GetField("teamCode").ToString();
            print("team code " + teamCode);

            string playerCode = PlayerData[l].GetField("playerCode").ToString().Trim('"');
            print("player code " + playerCode);

            string PlayerColor = PlayerData[l].GetField("teamColor").ToString().Trim('"');
            print("color " + PlayerColor);

            //print("mainSpawnContainer.transform.GetChild(playerindex).gameObject.activeInHierarchy" + mainSpawnContainer.transform.GetChild(playerindex).gameObject.activeInHierarchy);
            if (mainSpawnContainer.transform.GetChild(playerindex).gameObject.activeInHierarchy)
            {
                mainSpawnContainer.transform.GetChild(playerindex).gameObject.GetComponent<Iceland>().playerType = "human";//playerDetails.allUsers[i].type;
                mainSpawnContainer.transform.GetChild(playerindex).gameObject.GetComponent<Iceland>().playerCode = playerCode;// playerDetails.allUsers[i].name;
                mainSpawnContainer.transform.GetChild(playerindex).gameObject.GetComponent<Iceland>().teamCode = teamCode;//playerDetails.allUsers[i].team;
                mainSpawnContainer.transform.GetChild(playerindex).gameObject.GetComponent<Iceland>().color = PlayerColor;// playerDetails.allUsers[i].color;
                if (planeToBeSet >= mainSpawnContainer.transform.GetChild(playerindex).gameObject.GetComponent<Iceland>().MyLimit)
                {  
                    planeToBeSet = mainSpawnContainer.transform.GetChild(playerindex).gameObject.GetComponent<Iceland>().MyLimit;
                }
                mainSpawnContainer.transform.GetChild(playerindex).gameObject.GetComponent<Iceland>().totalPlane = planeToBeSet;
                mainSpawnContainer.transform.GetChild(playerindex).gameObject.GetComponent<Iceland>().isFree = false;
                mainSpawnContainer.transform.GetChild(playerindex).gameObject.GetComponent<Iceland>().isStatic = false;

                //enable top header player visibility
                if (PlayerColor == "red")
                {
                    if (redPlayer != null)
                    {
                        redPlayer.SetActive(true);
                    }
                    if (GameManager.Inst.Myid == GameManager.Inst.ClientID)
                    {
                        GameManager.Inst.MyColor = "red";
                        GameManager.Inst.MyteamCode = 1;
                        print("I Am Red Team");
                    }
                }

                if (PlayerColor == "yellow")
                {
                    if (yellowPlayer != null)
                    {
                        yellowPlayer.SetActive(true);
                    }
                    if (GameManager.Inst.Myid == GameManager.Inst.ClientID)
                    {
                        GameManager.Inst.MyColor = "yellow";
                        GameManager.Inst.MyteamCode = 2;
                        print("I Am yellow Team");
                    }
                }

                if (PlayerColor == "green")
                {
                    if (greenPlayer != null)
                    {
                        greenPlayer.SetActive(true);
                    }
                    if (GameManager.Inst.Myid == GameManager.Inst.ClientID)
                    {
                        GameManager.Inst.MyColor = "green";
                        GameManager.Inst.MyteamCode = 3;
                        print("I Am green Team");
                    }
                }

                if (PlayerColor == "blue")
                {
                    if (bluePlayer != null)
                    {
                        bluePlayer.SetActive(true);
                    }
                    if (GameManager.Inst.Myid == GameManager.Inst.ClientID)
                    {
                        GameManager.Inst.MyColor = "blue";
                        GameManager.Inst.MyteamCode = 4;
                        print("I Am blue Team");
                    }
                }
            }
        }

        GameUtil.ResetAttackMode();

        GameUtil.activatedIcelands = activatedIcelands;
        StartGamebool = true;
    }
    // Update is called once per frame
    private void Update()
    {

    }

    void ServerData(bool isAttack, int attackIndex, int selectediceland, int totalPlaneServer, string plane_id, Vector3 pos )
    {
        //print("attack on " + mainSpawnContainer.transform.GetChild(attackIndex).gameObject.name);
        //print("attack from " + mainSpawnContainer.transform.GetChild(selectediceland).gameObject.name);

        print("Server Call but not forther" + GameUtil.CheckIsIcelandSelected(selectediceland));

        if (isAttack && attackIndex != -1 && GameUtil.CheckIsIcelandSelected(GameUtil.selectedIceLand[selectediceland]))
        {
            //get both object and store into one object
            GameObject attackOn = mainSpawnContainer.transform.GetChild(selectediceland).gameObject;
            GameObject attackFrom = mainSpawnContainer.transform.GetChild(attackIndex).gameObject;
            print("attack on " + attackOn.name);
            print("attack from " + attackFrom.name);
            #region Send Attacking planes
            //get number of planes to be removed
            int removePlane = 0;
            if (attackFrom.GetComponent<Iceland>().totalPlane > 1)
            {
                //deactivated activated round iceland border of attackFrom iceland
                //attackFrom.transform.GetChild(1).gameObject.SetActive(false);
                attackFrom.transform.GetChild(1).gameObject.GetComponent<Image>().fillAmount = 0;

                removePlane = Mathf.RoundToInt(totalPlaneServer / 2);
                attackFrom.GetComponent<Iceland>().totalPlane = removePlane;    //deduct plane to e removed from attackFrom iceland

                int attackerTotalPlane = attackOn.GetComponent<Iceland>().totalPlane;

                //generate one plane object and assign to object
                GameObject planeObject = Instantiate(planePrefab, GameObject.Find("AttackingPlaneTransform").transform/*mainSpawnContainer.gameObject.transform*/);

                //set position of plane to attackFrom's position
                if (pos == Vector3.zero)
                {
                    planeObject.transform.position = attackFrom.transform.position;
                }
                else
                {
                    planeObject.transform.position = pos;
                }
                //assign plane to be remove from attackTo iceland and allow to start animation moving towards attackTo object
                planeObject.GetComponent<AttackingPlane>().AttackingPlane_ID = plane_id;
                planeObject.GetComponent<AttackingPlane>().totalPlane = removePlane;
                planeObject.GetComponent<AttackingPlane>().placeAtInitial = true;

                //assign details that needs to be changed after plane reach to the destination iceland
                Color attackerColor = attackFrom.GetComponentInChildren<Image>().color;
                planeObject.transform.GetChild(0).GetComponent<Image>().color = new Color(attackerColor.r, attackerColor.g, attackerColor.b, 0.7f);
                planeObject.GetComponent<AttackingPlane>().playerCode = attackFrom.GetComponent<Iceland>().playerCode;
                planeObject.GetComponent<AttackingPlane>().teamCode = attackFrom.GetComponent<Iceland>().teamCode;
                planeObject.GetComponent<AttackingPlane>().playerType = attackFrom.GetComponent<Iceland>().playerType;
                planeObject.GetComponent<AttackingPlane>().color = attackFrom.GetComponent<Iceland>().color;
                planeObject.GetComponent<AttackingPlane>().isFree = false;
                planeObject.GetComponent<AttackingPlane>().isStatic = false;

                //Adding the Attacking plane in the Dictionary
                serverObject.Add(plane_id, planeObject.GetComponent<AttackingPlane>());

                //planeObject.transform.SetSiblingIndex();

                //start attacking
                GameUtil.GenerateAttackPlane(planeObject, attackFrom.transform, attackOn.transform);

                print("Attck from to here => " + planeObject.GetComponent<AttackingPlane>().totalPlane);
                print("Attck from to here => " + planeObject.GetComponent<AttackingPlane>().color);
                print("Attck from to here => " + planeObject.GetComponent<AttackingPlane>().playerCode);
                print("Attck from to here => " + planeObject.GetComponent<AttackingPlane>().teamCode);
                print("Attck from to here => " + planeObject.GetComponent<AttackingPlane>().isFree);
                print("Attck from to here => " + planeObject.GetComponent<AttackingPlane>().isStatic);
                print("Attck from to here => " + attackOn.name);

                //reset attack
                GameUtil.ResetAttackMode();
            }
            #endregion
        }
    }




    public void AddData()
    {
        TeamOne.Clear();
        TeamTwo.Clear();
        TeamThree.Clear();
        TeamFour.Clear();

        for (int i = 0; i < Allplanet.Count; i++)
        {
            //print(Allplanet[i].GetComponent<Iceland>().playerType);
            if (Allplanet[i].GetComponent<Iceland>().teamCode.Equals("1"))
            {
                TeamOne.Add(1);
            }
            else if (Allplanet[i].GetComponent<Iceland>().teamCode.Equals("2"))
            {
                TeamTwo.Add(1);
            }
            else if (Allplanet[i].GetComponent<Iceland>().teamCode.Equals("3"))
            {
                TeamThree.Add(1);
            }
            else if (Allplanet[i].GetComponent<Iceland>().teamCode.Equals("4"))
            {
                TeamFour.Add(1);
            }
        }

        CheckWinner();
    }

    public void CheckWinner()
    {
        print(" CheckWinner ");

        if (TeamOne.Count == 0 && GameManager.Inst.MyteamCode == 1)
        {
            print("<Color=red><b>!♦ TeamOne lost ♦!</b></Color>");
            GameUtil.isAttackModeOn = false;
            GameObject.Find("Winner").transform.localScale = Vector3.one;
            GameObject.Find("WinnerTxt").GetComponent<TMP_Text>().text = "You lose";
            GameObject.Find("WinnerTxt").GetComponent<TMP_Text>().color = Color.red;
            LostPlane.transform.localScale = Vector3.one;
            WinPlane.transform.localScale = Vector3.zero;

            if (TotalPlayer == 2)
            {
                GameObject.Find("Rank").GetComponent<Text>().text = "YOUR RANK 2";
                GameObject.Find("Percentage").GetComponent<Text>().text = "0%";
                GameObject.Find("PercentageSlider").GetComponent<Image>().fillAmount = 0f;
            }
            else if (TotalPlayer == 3)
            {
                GameObject.Find("Rank").GetComponent<Text>().text = "YOUR RANK 3";
                GameObject.Find("Percentage").GetComponent<Text>().text = "0%";
                GameObject.Find("PercentageSlider").GetComponent<Image>().fillAmount = 0f;
            }
            else if (TotalPlayer == 4)
            {
                GameObject.Find("Rank").GetComponent<Text>().text = "YOUR RANK 4";
                GameObject.Find("Percentage").GetComponent<Text>().text = "0%";
                GameObject.Find("PercentageSlider").GetComponent<Image>().fillAmount = 0f;
            }
            GameManager.Inst.gameMode = GameMode.GameOver;
        }
        if (TeamTwo.Count == 0 && GameManager.Inst.MyteamCode == 2)
        {
            print("<Color=red><b>!♦ TeamOne lost ♦!</b></Color>");
            GameUtil.isAttackModeOn = false;
            GameObject.Find("Winner").transform.localScale = Vector3.one;
            GameObject.Find("WinnerTxt").GetComponent<TMP_Text>().text = "You lose";
            GameObject.Find("WinnerTxt").GetComponent<TMP_Text>().color = Color.red;
            LostPlane.transform.localScale = Vector3.one;
            WinPlane.transform.localScale = Vector3.zero;

            if (TotalPlayer == 2)
            {
                GameObject.Find("Rank").GetComponent<Text>().text = "YOUR RANK 2";
                GameObject.Find("Percentage").GetComponent<Text>().text = "0%";
                GameObject.Find("PercentageSlider").GetComponent<Image>().fillAmount = 0f;
            }
            else if (TotalPlayer == 3)
            {
                GameObject.Find("Rank").GetComponent<Text>().text = "YOUR RANK 3";
                GameObject.Find("Percentage").GetComponent<Text>().text = "0%";
                GameObject.Find("PercentageSlider").GetComponent<Image>().fillAmount = 0f;
            }
            else if (TotalPlayer == 4)
            {
                GameObject.Find("Rank").GetComponent<Text>().text = "YOUR RANK 4";
                GameObject.Find("Percentage").GetComponent<Text>().text = "0%";
                GameObject.Find("PercentageSlider").GetComponent<Image>().fillAmount = 0f;
            }
            GameManager.Inst.gameMode = GameMode.GameOver;
        }
        if (TeamThree.Count == 0 && GameManager.Inst.MyteamCode == 3)
        {
            print("<Color=red><b>!♦ TeamOne lost ♦!</b></Color>");
            GameUtil.isAttackModeOn = false;
            GameObject.Find("Winner").transform.localScale = Vector3.one;
            GameObject.Find("WinnerTxt").GetComponent<TMP_Text>().text = "You lose";
            GameObject.Find("WinnerTxt").GetComponent<TMP_Text>().color = Color.red;
            LostPlane.transform.localScale = Vector3.one;
            WinPlane.transform.localScale = Vector3.zero;
            if (TotalPlayer == 2)
            {
                GameObject.Find("Rank").GetComponent<Text>().text = "YOUR RANK 2";
                GameObject.Find("Percentage").GetComponent<Text>().text = "0%";
                GameObject.Find("PercentageSlider").GetComponent<Image>().fillAmount = 0f;
            }
            else if (TotalPlayer == 3)
            {
                GameObject.Find("Rank").GetComponent<Text>().text = "YOUR RANK 3";
                GameObject.Find("Percentage").GetComponent<Text>().text = "0%";
                GameObject.Find("PercentageSlider").GetComponent<Image>().fillAmount = 0f;
            }
            else if (TotalPlayer == 4)
            {
                GameObject.Find("Rank").GetComponent<Text>().text = "YOUR RANK 4";
                GameObject.Find("Percentage").GetComponent<Text>().text = "0%";
                GameObject.Find("PercentageSlider").GetComponent<Image>().fillAmount = 0f;
            }
            GameManager.Inst.gameMode = GameMode.GameOver;
        }
        if (TeamFour.Count == 0 && GameManager.Inst.MyteamCode == 4)
        {
            print("<Color=red><b>!♦ TeamOne lost ♦!</b></Color>");
            GameUtil.isAttackModeOn = false;
            GameObject.Find("Winner").transform.localScale = Vector3.one;
            GameObject.Find("WinnerTxt").GetComponent<TMP_Text>().text = "You lose";
            GameObject.Find("WinnerTxt").GetComponent<TMP_Text>().color = Color.red;
            LostPlane.transform.localScale = Vector3.one;
            WinPlane.transform.localScale = Vector3.zero;
            if (TotalPlayer == 2)
            {
                GameObject.Find("Rank").GetComponent<Text>().text = "YOUR RANK 2";
                GameObject.Find("Percentage").GetComponent<Text>().text = "0%";
                GameObject.Find("PercentageSlider").GetComponent<Image>().fillAmount = 0f;
            }
            else if (TotalPlayer == 3)
            {
                GameObject.Find("Rank").GetComponent<Text>().text = "YOUR RANK 3";
                GameObject.Find("Percentage").GetComponent<Text>().text = "0%";
                GameObject.Find("PercentageSlider").GetComponent<Image>().fillAmount = 0f;
            }
            else if (TotalPlayer == 4)
            {
                GameObject.Find("Rank").GetComponent<Text>().text = "YOUR RANK 4";
                GameObject.Find("Percentage").GetComponent<Text>().text = "0%";
                GameObject.Find("PercentageSlider").GetComponent<Image>().fillAmount = 0f;
            }
            GameManager.Inst.gameMode = GameMode.GameOver;
        }

        try
        {
            if (GameManager.Inst.MyteamCode == 1)
            {
                if (TotalPlayer == 2 && TeamTwo.Count == 0)
                {
                    print("<Color=yellow><b>!♦ TeamTwo Player lost ♦!</b></Color>");
                    GameUtil.isAttackModeOn = false;
                    GameObject.Find("Winner").transform.localScale = Vector3.one;
                    GameObject.Find("WinnerTxt").GetComponent<TMP_Text>().text = "You win";
                    GameObject.Find("WinnerTxt").GetComponent<TMP_Text>().color = Color.green;
                    LostPlane.transform.localScale = Vector3.zero;
                    WinPlane.transform.localScale = Vector3.one;

                    GameObject.Find("Rank").GetComponent<Text>().text = "YOUR RANK 1";
                    GameObject.Find("Percentage").GetComponent<Text>().text = "100%";
                    GameObject.Find("PercentageSlider").GetComponent<Image>().fillAmount = 1f;
                    GameManager.Inst.gameMode = GameMode.GameOver;
                }
                else if (TotalPlayer == 3 && TeamTwo.Count == 0 && TeamThree.Count == 0)
                {
                    print("<Color=yellow><b>!♦ TeamTwo Player lost ♦!</b></Color>");
                    GameUtil.isAttackModeOn = false;
                    GameObject.Find("Winner").transform.localScale = Vector3.one;
                    GameObject.Find("WinnerTxt").GetComponent<TMP_Text>().text = "You win";
                    GameObject.Find("WinnerTxt").GetComponent<TMP_Text>().color = Color.green;
                    LostPlane.transform.localScale = Vector3.zero;
                    WinPlane.transform.localScale = Vector3.one;

                    GameObject.Find("Rank").GetComponent<Text>().text = "YOUR RANK 1";
                    GameObject.Find("Percentage").GetComponent<Text>().text = "100%";
                    GameObject.Find("PercentageSlider").GetComponent<Image>().fillAmount = 1f;
                    GameManager.Inst.gameMode = GameMode.GameOver;
                }
                else if (TotalPlayer == 4 && TeamTwo.Count == 0 && TeamThree.Count == 0 && TeamFour.Count == 0)
                {
                    print("<Color=yellow><b>!♦ TeamTwo Player lost ♦!</b></Color>");
                    GameUtil.isAttackModeOn = false;
                    GameObject.Find("Winner").transform.localScale = Vector3.one;
                    GameObject.Find("WinnerTxt").GetComponent<TMP_Text>().text = "You win";
                    GameObject.Find("WinnerTxt").GetComponent<TMP_Text>().color = Color.green;
                    LostPlane.transform.localScale = Vector3.zero;
                    WinPlane.transform.localScale = Vector3.one;

                    GameObject.Find("Rank").GetComponent<Text>().text = "YOUR RANK 1";
                    GameObject.Find("Percentage").GetComponent<Text>().text = "100%";
                    GameObject.Find("PercentageSlider").GetComponent<Image>().fillAmount = 1f;
                    GameManager.Inst.gameMode = GameMode.GameOver;
                }
            }

            if (GameManager.Inst.MyteamCode == 2)
            {
                if (TotalPlayer == 2 && TeamOne.Count == 0)
                {
                    print("<Color=yellow><b>!♦ TeamTwo Player lost ♦!</b></Color>");
                    GameUtil.isAttackModeOn = false;
                    GameObject.Find("Winner").transform.localScale = Vector3.one;
                    GameObject.Find("WinnerTxt").GetComponent<TMP_Text>().text = "You win";
                    GameObject.Find("WinnerTxt").GetComponent<TMP_Text>().color = Color.green;
                    LostPlane.transform.localScale = Vector3.zero;
                    WinPlane.transform.localScale = Vector3.one;

                    GameObject.Find("Rank").GetComponent<Text>().text = "YOUR RANK 1";
                    GameObject.Find("Percentage").GetComponent<Text>().text = "100%";
                    GameObject.Find("PercentageSlider").GetComponent<Image>().fillAmount = 1f;
                    GameManager.Inst.gameMode = GameMode.GameOver;
                }
                else if (TotalPlayer == 3 && TeamOne.Count == 0 && TeamThree.Count == 0)
                {
                    print("<Color=yellow><b>!♦ TeamTwo Player lost ♦!</b></Color>");
                    GameUtil.isAttackModeOn = false;
                    GameObject.Find("Winner").transform.localScale = Vector3.one;
                    GameObject.Find("WinnerTxt").GetComponent<TMP_Text>().text = "You win";
                    GameObject.Find("WinnerTxt").GetComponent<TMP_Text>().color = Color.green;
                    LostPlane.transform.localScale = Vector3.zero;
                    WinPlane.transform.localScale = Vector3.one;

                    GameObject.Find("Rank").GetComponent<Text>().text = "YOUR RANK 1";
                    GameObject.Find("Percentage").GetComponent<Text>().text = "100%";
                    GameObject.Find("PercentageSlider").GetComponent<Image>().fillAmount = 1f;
                    GameManager.Inst.gameMode = GameMode.GameOver;
                }
                else if (TotalPlayer == 4 && TeamOne.Count == 0 && TeamThree.Count == 0 && TeamFour.Count == 0)
                {
                    print("<Color=yellow><b>!♦ TeamTwo Player lost ♦!</b></Color>");
                    GameUtil.isAttackModeOn = false;
                    GameObject.Find("Winner").transform.localScale = Vector3.one;
                    GameObject.Find("WinnerTxt").GetComponent<TMP_Text>().text = "You win";
                    GameObject.Find("WinnerTxt").GetComponent<TMP_Text>().color = Color.green;
                    LostPlane.transform.localScale = Vector3.zero;
                    WinPlane.transform.localScale = Vector3.one;

                    GameObject.Find("Rank").GetComponent<Text>().text = "YOUR RANK 1";
                    GameObject.Find("Percentage").GetComponent<Text>().text = "100%";
                    GameObject.Find("PercentageSlider").GetComponent<Image>().fillAmount = 1f;
                    GameManager.Inst.gameMode = GameMode.GameOver;
                }
            }

            if (GameManager.Inst.MyteamCode == 3)
            {
                if (TotalPlayer == 2 && TeamOne.Count == 0)
                {
                    print("<Color=yellow><b>!♦ TeamTwo Player lost ♦!</b></Color>");
                    GameUtil.isAttackModeOn = false;
                    GameObject.Find("Winner").transform.localScale = Vector3.one;
                    GameObject.Find("WinnerTxt").GetComponent<TMP_Text>().text = "You win";
                    GameObject.Find("WinnerTxt").GetComponent<TMP_Text>().color = Color.green;
                    LostPlane.transform.localScale = Vector3.zero;
                    WinPlane.transform.localScale = Vector3.one;

                    GameObject.Find("Rank").GetComponent<Text>().text = "YOUR RANK 1";
                    GameObject.Find("Percentage").GetComponent<Text>().text = "100%";
                    GameObject.Find("PercentageSlider").GetComponent<Image>().fillAmount = 1f;
                    GameManager.Inst.gameMode = GameMode.GameOver;
                }
                else if (TotalPlayer == 3 && TeamOne.Count == 0 && TeamTwo.Count == 0)
                {
                    print("<Color=yellow><b>!♦ TeamTwo Player lost ♦!</b></Color>");
                    GameUtil.isAttackModeOn = false;
                    GameObject.Find("Winner").transform.localScale = Vector3.one;
                    GameObject.Find("WinnerTxt").GetComponent<TMP_Text>().text = "You win";
                    GameObject.Find("WinnerTxt").GetComponent<TMP_Text>().color = Color.green;
                    LostPlane.transform.localScale = Vector3.zero;
                    WinPlane.transform.localScale = Vector3.one;

                    GameObject.Find("Rank").GetComponent<Text>().text = "YOUR RANK 1";
                    GameObject.Find("Percentage").GetComponent<Text>().text = "100%";
                    GameObject.Find("PercentageSlider").GetComponent<Image>().fillAmount = 1f;
                    GameManager.Inst.gameMode = GameMode.GameOver;
                }
                else if (TotalPlayer == 4 && TeamOne.Count == 0 && TeamTwo.Count == 0 && TeamFour.Count == 0)
                {
                    print("<Color=yellow><b>!♦ TeamTwo Player lost ♦!</b></Color>");
                    GameUtil.isAttackModeOn = false;
                    GameObject.Find("Winner").transform.localScale = Vector3.one;
                    GameObject.Find("WinnerTxt").GetComponent<TMP_Text>().text = "You win";
                    GameObject.Find("WinnerTxt").GetComponent<TMP_Text>().color = Color.green;
                    LostPlane.transform.localScale = Vector3.zero;
                    WinPlane.transform.localScale = Vector3.one;

                    GameObject.Find("Rank").GetComponent<Text>().text = "YOUR RANK 1";
                    GameObject.Find("Percentage").GetComponent<Text>().text = "100%";
                    GameObject.Find("PercentageSlider").GetComponent<Image>().fillAmount = 1f;
                    GameManager.Inst.gameMode = GameMode.GameOver;
                }
            }

            if (GameManager.Inst.MyteamCode == 4)
            {
                if (TotalPlayer == 2 && TeamOne.Count == 0)
                {
                    print("<Color=yellow><b>!♦ TeamTwo Player lost ♦!</b></Color>");
                    GameUtil.isAttackModeOn = false;
                    GameObject.Find("Winner").transform.localScale = Vector3.one;
                    GameObject.Find("WinnerTxt").GetComponent<TMP_Text>().text = "You win";
                    GameObject.Find("WinnerTxt").GetComponent<TMP_Text>().color = Color.green;
                    LostPlane.transform.localScale = Vector3.zero;
                    WinPlane.transform.localScale = Vector3.one;

                    GameObject.Find("Rank").GetComponent<Text>().text = "YOUR RANK 1";
                    GameObject.Find("Percentage").GetComponent<Text>().text = "100%";
                    GameObject.Find("PercentageSlider").GetComponent<Image>().fillAmount = 1f;
                    GameManager.Inst.gameMode = GameMode.GameOver;
                }
                else if (TotalPlayer == 3 && TeamOne.Count == 0 && TeamTwo.Count == 0)
                {
                    print("<Color=yellow><b>!♦ TeamTwo Player lost ♦!</b></Color>");
                    GameUtil.isAttackModeOn = false;
                    GameObject.Find("Winner").transform.localScale = Vector3.one;
                    GameObject.Find("WinnerTxt").GetComponent<TMP_Text>().text = "You win";
                    GameObject.Find("WinnerTxt").GetComponent<TMP_Text>().color = Color.green;
                    LostPlane.transform.localScale = Vector3.zero;
                    WinPlane.transform.localScale = Vector3.one;

                    GameObject.Find("Rank").GetComponent<Text>().text = "YOUR RANK 1";
                    GameObject.Find("Percentage").GetComponent<Text>().text = "100%";
                    GameObject.Find("PercentageSlider").GetComponent<Image>().fillAmount = 1f;
                    GameManager.Inst.gameMode = GameMode.GameOver;
                }
                else if (TotalPlayer == 4 && TeamOne.Count == 0 && TeamTwo.Count == 0 && TeamThree.Count == 0)
                {
                    print("<Color=yellow><b>!♦ TeamTwo Player lost ♦!</b></Color>");
                    GameUtil.isAttackModeOn = false;
                    GameObject.Find("Winner").transform.localScale = Vector3.one;
                    GameObject.Find("WinnerTxt").GetComponent<TMP_Text>().text = "You win";
                    GameObject.Find("WinnerTxt").GetComponent<TMP_Text>().color = Color.green;
                    LostPlane.transform.localScale = Vector3.zero;
                    WinPlane.transform.localScale = Vector3.one;

                    GameObject.Find("Rank").GetComponent<Text>().text = "YOUR RANK 1";
                    GameObject.Find("Percentage").GetComponent<Text>().text = "100%";
                    GameObject.Find("PercentageSlider").GetComponent<Image>().fillAmount = 1f;
                    GameManager.Inst.gameMode = GameMode.GameOver;
                }
            }

            #region
            //if (TotalPlayer == 2 && GameManager.Inst.MyteamCode == 1 && TeamTwo.Count == 0)
            //{
            //    print("<Color=yellow><b>!♦ TeamTwo Player lost ♦!</b></Color>");
            //    GameUtil.isAttackModeOn = false;
            //    GameObject.Find("Winner").transform.localScale = Vector3.one;
            //    GameObject.Find("WinnerTxt").GetComponent<TMP_Text>().text = "You win";
            //    GameObject.Find("WinnerTxt").GetComponent<TMP_Text>().color = Color.green;
            //    LostPlane.transform.localScale = Vector3.zero;
            //    WinPlane.transform.localScale = Vector3.one;

            //    GameObject.Find("Rank").GetComponent<Text>().text = "YOUR RANK 1";
            //    GameObject.Find("Percentage").GetComponent<Text>().text = "100%";
            //    GameObject.Find("PercentageSlider").GetComponent<Image>().fillAmount = 1f;
            //    GameManager.Inst.gameMode = GameMode.GameOver;
            //}
            //if (TotalPlayer == 2 && GameManager.Inst.MyteamCode == 2 && TeamOne.Count == 0)
            //{
            //    print("<Color=yellow><b>!♦ TeamTwo Player lost ♦!</b></Color>");
            //    GameUtil.isAttackModeOn = false;
            //    GameObject.Find("Winner").transform.localScale = Vector3.one;
            //    GameObject.Find("WinnerTxt").GetComponent<TMP_Text>().text = "You win";
            //    GameObject.Find("WinnerTxt").GetComponent<TMP_Text>().color = Color.green;
            //    LostPlane.transform.localScale = Vector3.zero;
            //    WinPlane.transform.localScale = Vector3.one;

            //    GameObject.Find("Rank").GetComponent<Text>().text = "YOUR RANK 1";
            //    GameObject.Find("Percentage").GetComponent<Text>().text = "100%";
            //    GameObject.Find("PercentageSlider").GetComponent<Image>().fillAmount = 1f;
            //    GameManager.Inst.gameMode = GameMode.GameOver;
            //}
            //else if (TotalPlayer == 3 && GameManager.Inst.MyteamCode == 3 && TeamTwo.Count == 0 && TeamThree.Count == 0)
            //{
            //    print("<Color=green><b>!♦ TeamThree Player lost ♦!</b></Color>");
            //    GameUtil.isAttackModeOn = false;
            //    GameObject.Find("Winner").transform.localScale = Vector3.one;
            //    GameObject.Find("WinnerTxt").GetComponent<TMP_Text>().text = "You win";
            //    GameObject.Find("WinnerTxt").GetComponent<TMP_Text>().color = Color.green;
            //    LostPlane.transform.localScale = Vector3.zero;
            //    WinPlane.transform.localScale = Vector3.one;

            //    GameObject.Find("Rank").GetComponent<Text>().text = "YOUR RANK 1";
            //    GameObject.Find("Percentage").GetComponent<Text>().text = "100%";
            //    GameObject.Find("PercentageSlider").GetComponent<Image>().fillAmount = 1f;
            //    GameManager.Inst.gameMode = GameMode.GameOver;
            //}
            //else if (TotalPlayer == 4 && GameManager.Inst.MyteamCode == 4 && TeamTwo.Count == 0 && TeamThree.Count == 0 && TeamFour.Count == 0)
            //{
            //    print("<Color=blue><b>!♦ TeamFour Player lost ♦!</b></Color>");
            //    GameUtil.isAttackModeOn = false;
            //    GameObject.Find("Winner").transform.localScale = Vector3.one;
            //    GameObject.Find("WinnerTxt").GetComponent<TMP_Text>().text = "You win";
            //    GameObject.Find("WinnerTxt").GetComponent<TMP_Text>().color = Color.green;
            //    LostPlane.transform.localScale = Vector3.zero;
            //    WinPlane.transform.localScale = Vector3.one;

            //    GameObject.Find("Rank").GetComponent<Text>().text = "YOUR RANK 1";
            //    GameObject.Find("Percentage").GetComponent<Text>().text = "100%";
            //    GameObject.Find("PercentageSlider").GetComponent<Image>().fillAmount = 1f;
            //    GameManager.Inst.gameMode = GameMode.GameOver;
            //}
            #endregion
        }
        catch (System.Exception ex)
        {
            print("Exception " + ex);
        }

    }

    public void PauseBtn()
    {
        //Time.timeScale = 0;
        GameObject.Find("PlayPauseStatus").GetComponent<Image>().sprite = PasueSp;
        PauseScreen.transform.localScale = Vector3.one;
        AudioManager.instance.Btn();
    }

    public void Resume()
    {
        GameObject.Find("PlayPauseStatus").GetComponent<Image>().sprite = PlaySp;
        //Time.timeScale = 1;
        PauseScreen.transform.localScale = Vector3.zero;
        AudioManager.instance.Btn();
    }

    public void QuitGame()
    {
        print("<Color><b> Quit Game Call </b></Color>");
        GameManager.Inst.Lobby_id = null;
        Time.timeScale = 1;
        Dictionary<string, string> pairs = new Dictionary<string, string>();
        pairs.Add("playerid", GameManager.Inst.ClientID);
        socketIOComponent.Emit("exitlobby", new JSONObject(pairs));
        SceneManager.LoadScene("MainMenuScreen");
        AudioManager.instance.Btn();
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
    public void ExitGame()
    {
        GameManager.Inst.Lobby_id = null;
        Dictionary<string, string> pairs = new Dictionary<string, string>();
        pairs.Add("playerid", GameManager.Inst.ClientID);
        socketIOComponent.Emit("exitlobby", new JSONObject(pairs));
        SceneManager.LoadScene("MainMenuScreen");
        AudioManager.instance.Btn();

    }

    private void OnApplicationFocus(bool focus)
    {

        print("OnApplicationFocus-=>" + GameManager.Inst.allowupdate);
        if (focus && StartGamebool)
        {
            Dictionary<string, string> pairs = new Dictionary<string, string>();
            pairs.Add("playerid", GameManager.Inst.ClientID);
            pairs.Add("lobby_id", GameManager.Inst.Lobby_id);
            socketIOComponent.Emit("Getplanedata", new JSONObject(pairs));
            //appispause = false;
            GameManager.Inst.allowupdate = true;
        }
        else
        {
            GameManager.Inst.allowupdate = true;
        }
    }
    //private void OnApplicationPause(bool pause)
    //{
    //    //Update();
    //    if (pause)
    //    {
    //        ////Application on pause mode
    //        //while (pause)
    //        //{
    //        //    print("<Color><b> Application on pause mode </b></Color>");
    //        //}
    //        appispause = true;
    //    }
    //    else
    //    {
    //        ////Application foreground
    //        //print("<Color><b> Application foreground </b></Color>");
    //        //if (SceneManager.GetActiveScene().name == ("MultiPlayerGamePlayScene"))
    //        //{
    //        //    GameObject.Find("ErrorPanel").transform.localScale = Vector3.one;
    //        //}
    //        socketIOComponent.Emit("Getplanedata");
    //        appispause = false;
    //        allowupdate = true;
    //    }
    //}

    private void UpdatePlanedata(JSONObject data)
    {

        //sprint("-=-NewData=-=->" + data.ToString());
        //JSONObject Newdata = new JSONObject(data.GetField("Newplanedata1").ToString());
        JSONObject Newdata = data;
        print("-=-NewData=-=->" + Newdata.ToString());
        //JSONObject Newdata = new JSONObject(Newdata1.GetField("NewPlanedata1").ToString());
        print("serverObjectCount-=-=-=->" + Newdata.Count);
        destoryPlanes();
        
        for (int i = 0; i < Newdata.Count; i++)
        {
            //int planeToBeSet = int.Parse(obj.data.GetField("Newplanedata")[i].ToString());
            //int plane1 = int.Parse(obj.data.GetField("Newplanedata")[i].GetField("totalPlane").ToString());
            print("-=-=-dataaaa=-=>" + Newdata[i]);
            try
            {
                string plane_id = Newdata[i].GetField("plane_id").ToString().Trim('"');
                string strx = Newdata[i].GetField("X").ToString().Trim('"');
                float X =float.Parse(strx);
                string stry = Newdata[i].GetField("Y").ToString().Trim('"');
                float Y = float.Parse(stry);
                string strz = Newdata[i].GetField("Z").ToString().Trim('"');
                float Z = float.Parse(strz);
                string str2 = Newdata[i].GetField("selectedIceLand").ToString().Trim('"');
                int Si = int.Parse(str2);
                string str3 = Newdata[i].GetField("iceLandIndex").ToString().Trim('"');
                int ii = int.Parse(str3);
                string str4 = Newdata[i].GetField("TotalPlane").ToString().Trim('"');
                int ToatalPlane = int.Parse(str4);


                print("parsed" + plane_id + strx + stry + strz + str2 + str3 + str4);


                //string str5 = Newdata[i].GetField("plane_id").ToString().Trim('"');
                ServerData(true, Si, ii, ToatalPlane, plane_id, new Vector3(X, Y, Z));


            }
            catch (System.Exception e)
            {
                print("EE" + e);
                throw;
            }

            //Allplanet[i].GetComponent<Iceland>().Updatetotalplane(planeToBeSet);

        }
    }

    private void destoryPlanes()
    {
        foreach(var item in serverObject)
        {
            Destroy(item.Value.gameObject);
        }
        serverObject.Clear();
        
    }





    private void tranfromdata(string playerid)
    {
        planeposdata planeposdata = new planeposdata();
        planeposdata.playerid = playerid.RemoveQuotes();
        foreach (var item in serverObject)
        {
            //Vector3 pos = item.Value.transform.position;
            Pos pos1 = new Pos();
            pos1.id = item.Key;
            pos1.X = item.Value.transform.position.x.TwoDecimals();
            pos1.Y = item.Value.transform.position.y.TwoDecimals();
            pos1.Z = item.Value.transform.position.z.TwoDecimals();
            planeposdata.pos.Add(pos1);
             print("-=-=-=-=->" + planeposdata.pos.Count);
        }
        print("planedata-=-=-=> " + new JSONObject(JsonUtility.ToJson(planeposdata)));
        socketIOComponent.Emit("Updatedataplane", new JSONObject(JsonUtility.ToJson(planeposdata)));
    }

    public void ClosePopUP()
    {
        GameObject.Find("ErrorPanel").transform.localScale = Vector3.zero;
        SceneManager.LoadScene("MainMenuScreen");
        AudioManager.instance.Btn();
    }
    private void OnDestroy()
    {

    }

}