using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Applications.Utils;
using SocketIO;
using UnityEngine.SceneManagement;

public class Iceland : MonoBehaviour
{

    public string playerCode;
    public bool isFree;
    public bool isStatic;
    public string teamCode;
    public string playerType;
    public string color;
    public int totalPlane;
    public int iceLandIndex;
    public int timeNotAttacked = 0;


    public GameObject planePrefab;

    public SocketIOComponent socketIOComponent;

    public int MyLimit;

    public bool isBigIcelandOrSmall;
    private void Awake()
    {
        //if (isBigIcelandOrSmall)
        //{
        //    //I am Big Iceland 
        //    MyLimit = GameManager.Inst.bigIcelandLimit;
        //}
        //else 
        //{
        //    //I am Small Iceland 
        //    MyLimit = GameManager.Inst.smallIcelandLimit;
        //}
    }

    // Start is called before the first frame update
    void Start()
    {

        if (GameObject.FindGameObjectWithTag("SocketIO") != null && socketIOComponent == null)
            socketIOComponent = GameObject.FindGameObjectWithTag("SocketIO").GetComponent<SocketIOComponent>();


        //update Iceland owner according to data
        //UpdateIceLand();



        //Debug.Log("icelandIndex: " + iceLandIndex);
        if (SceneManager.GetActiveScene().name != ("MultiPlayerGamePlayScene"))
        {
            //add button click
            GetComponent<Button>().onClick.AddListener(AttackManager);
            StartCoroutine(StartComputerBotAttack());    //HERE

            if (playerType == "human")
            {
                StartCoroutine(TeamIncrement(1f));
                //StartCoroutine(TeamIncrement(Random.Range(0.5f, 1f)));
            }
            else
            {
                StartCoroutine(TeamIncrement(1f));
                //StartCoroutine(TeamIncrement(Random.Range(1.5f, 2f)));
            }

        }
        else
        {
            //add button click for multiplayer
            GetComponent<Button>().onClick.AddListener(AttackManagerMulti);

        }

        socketIOComponent.On("startTimer", (SocketIOEvent obj) =>
        {
               // StartCoroutine(TeamIncrementMulti(1f));      //here time increment is fix bcoz both side same increment
            print("<Color><b>!♦ startTimer ♦!</b></Color>" + obj);
        });

    }

    IEnumerator StartComputerBotAttack()
    {
        yield return new WaitForSeconds(Random.Range(20, 30));
        if (playerType == "computer" && totalPlane > Random.Range(MyLimit / 2, MyLimit) /*&& GameManager.Inst.attackCounter == 0*/)
        {
            int[] sortedIcelands = GameUtil.GetSortedListOfPlayers(totalPlane / 2);
            for (int i = 0; i < sortedIcelands.Length; i++)
            {
                print("<Color><b>!♦ sortedIcelands ♦!</b></Color>" + sortedIcelands[i]);
            }


            sortedIcelands = GameUtil.GetOtherPlayerIndexes(teamCode, sortedIcelands);

            int middleRange = sortedIcelands.Length / 2;

            //for (int i = 0; i < middleRange; i++)
            //{
            //}
            Debug.Log("middleRange: " + middleRange);

            int attackOn = Random.Range(0, middleRange);

            //attackOn = 0;

            GameObject attackOnObject = GameUtil.staticSpawnContainer.transform.GetChild(sortedIcelands[attackOn]).gameObject;

            int removePlane = Mathf.RoundToInt(totalPlane / 2);
            totalPlane -= removePlane;

            GameObject planeObject = Instantiate(planePrefab, GameObject.Find("AttackingPlaneTransform").transform/*GameUtil.staticSpawnContainer.gameObject.transform*/);

            //set position of plane to attackFrom's position
            planeObject.transform.position = transform.position;

            //assign plane to be remove from attackTo iceland and allow to start animation moving towards attackTo object
            planeObject.GetComponent<AttackingPlane>().totalPlane = removePlane;
            planeObject.GetComponent<AttackingPlane>().placeAtInitial = true;

            //assign details that needs to be changed after plane reach to the destination iceland
            Color attackerColor = GetComponentInChildren<Image>().color;
            planeObject.transform.GetChild(0).GetComponent<Image>().color = new Color(attackerColor.r, attackerColor.g, attackerColor.b, 0.7f);
            planeObject.GetComponent<AttackingPlane>().playerCode = GetComponent<Iceland>().name;
            planeObject.GetComponent<AttackingPlane>().teamCode = GetComponent<Iceland>().teamCode;
            planeObject.GetComponent<AttackingPlane>().playerType = GetComponent<Iceland>().playerType;
            planeObject.GetComponent<AttackingPlane>().color = GetComponent<Iceland>().color;
            planeObject.GetComponent<AttackingPlane>().isFree = false;
            planeObject.GetComponent<AttackingPlane>().isStatic = false;

            //planeObject.transform.SetSiblingIndex();

            //start attacking
            GameUtil.GenerateAttackPlane(planeObject, transform, attackOnObject.transform);
            //GameManager.Inst.attackCounter++;
        }

        StartCoroutine(StartComputerBotAttack());
    }

    // will update iceland according to set data
    void UpdateIceLand()
    {
        //Debug.Log("SELECTED INDEX: " + GameUtil.selectedIceLand[0]);
        if (color == "red")
            GetComponent<Image>().color = Color.red;

        if (color == "yellow")
            GetComponent<Image>().color = Color.yellow;

        if (color == "green")
            GetComponent<Image>().color = Color.green;

        if (color == "blue")
            GetComponent<Image>().color = Color.blue;

        // change font color if icland is of no one
        if (isStatic)
        {
            GetComponentInChildren<TMP_Text>().color = Color.blue;
            GetComponent<Image>().color = Color.white;
        }
        else
        {
            GetComponentInChildren<TMP_Text>().color = Color.white;
        }

        GetComponentInChildren<TMP_Text>().text = "" + totalPlane;
    }

    // will be click of iceland
    public void AttackManager()
    {

        Debug.Log("icelandIndex: " + iceLandIndex + " playerType: " + playerType);

        print(GameUtil.isAttackModeOn + " form " + GameUtil.selectedIceLand[0] + " on iceLandIndex " + iceLandIndex);

        if (SceneManager.GetActiveScene().name.Equals("MultiPlayerGamePlayScene"))
        {
            if (GameUtil.isAttackModeOn)
            {
                Dictionary<string, string> pairs = new Dictionary<string, string>();
                pairs.Add("isAttackModeOn", GameUtil.isAttackModeOn.ToString());
                pairs.Add("selectedIceLand", GameUtil.selectedIceLand[0].ToString());
                pairs.Add("iceLandIndex", iceLandIndex.ToString());
                pairs.Add("deviceId", SystemInfo.deviceUniqueIdentifier);
                socketIOComponent.Emit("Attack", new JSONObject(pairs));
            }
        }

        if (playerType == "human")
        {
            if (GameUtil.CheckIsIcelandSelected(iceLandIndex))
            {

                print("check" + GameUtil.CheckIsIcelandSelected(iceLandIndex));
                Singleplayerreset();
                //this.transform.GetChild(1).gameObject.SetActive(false);
                    //print("-=-=-=->Attack111111-=-=->" + gameObject.name);
                    //this.transform.GetChild(1).gameObject.GetComponent<Image>().fillAmount = 0;
                    //GameUtil.ResetAttackMode(); 
            }
            else
            {

                string selectedPlayerType = GameUtil.GetIcelandValueUsingKey<string>(this.transform.parent.gameObject, GameUtil.selectedIceLand[0], "playerType");
                Debug.Log("selected iceland type: " + selectedPlayerType);

                if (selectedPlayerType == playerType && GameUtil.isAttackModeOn)
                {
                    if (GameUtil.isAttackModeOn)
                    {
                        print("-=-=-=->Attack22222222-=-=->"+gameObject.name);
                        GameUtil.attackIcelandIndex = iceLandIndex;
                        //reset timer if attacked on anyone
                        timeNotAttacked = 0;
                    }
                }
                else
                {
                    print("-=-=-=->Attack3333333-=-=->"+gameObject.name);
                    this.transform.GetChild(1).gameObject.SetActive(true);
                    this.transform.GetChild(1).gameObject.GetComponent<Image>().fillAmount = 1;
                    // GameUtil.selectedIceLand = new int[20];

                    GameUtil.numberofselected++;
                    GameUtil.selectedIceLand[GameUtil.numberofselected-1] = iceLandIndex;
                    

                    Debug.Log("Iceland selection " + GameUtil.selectedIceLand[0]+"-=-=-=->"+ GameUtil.numberofselected);

                    GameUtil.isAttackModeOn = true;
                        
                }
            }
        }
        else /*if (playerType == "computer")*/
        {
            if (GameUtil.isAttackModeOn)
            {
                this.transform.GetChild(1).gameObject.SetActive(true);
                this.transform.GetChild(1).gameObject.GetComponent<Image>().fillAmount = 0;
                print("-=-=-=->Attack4444444-=-=->"+gameObject.name);
                GameUtil.attackIcelandIndex = iceLandIndex;

                //reset timer if attacked on anyone
                timeNotAttacked = 0;
            }
        }

    }


    public void AttackManagerMulti()
    {
        Debug.Log("icelandIndex: " + iceLandIndex + " playerType: " + playerType);
        print(GameUtil.isAttackModeOn + " form " + GameUtil.selectedIceLand[0] + " on iceLandIndex " + iceLandIndex);

        if (SceneManager.GetActiveScene().name.Equals("MultiPlayerGamePlayScene"))
        {
            for (int i = 0; i < GameUtil.numberofselected; i++)
            {
                Iceland attack = GamePlayManagerMultiPlayer.inst.mainSpawnContainer.transform.GetChild(GameUtil.selectedIceLand[i]).gameObject.GetComponent<Iceland>();
                if (GameUtil.isAttackModeOn && GameUtil.selectedIceLand[0].ToString() != iceLandIndex.ToString() && attack.color == GameManager.Inst.MyColor)
                {
                    print("<Color><b> totalPlane ♥  </b></Color>>> " + GamePlayManagerMultiPlayer.inst.mainSpawnContainer.transform.GetChild(GameUtil.selectedIceLand[i]).gameObject.GetComponent<Iceland>().totalPlane);

                    Dictionary<string, string> pairs = new Dictionary<string, string>();
                    pairs.Add("isAttackModeOn", GameUtil.isAttackModeOn.ToString());
                    pairs.Add("selectedIceLand", GameUtil.selectedIceLand[i].ToString());
                    pairs.Add("iceLandIndex", iceLandIndex.ToString());
                    pairs.Add("totalPlane", GamePlayManagerMultiPlayer.inst.mainSpawnContainer.transform.GetChild(GameUtil.selectedIceLand[i]).gameObject.GetComponent<Iceland>().totalPlane.ToString());
                    pairs.Add("deviceId", SystemInfo.deviceUniqueIdentifier);
                    socketIOComponent.Emit("Attack", new JSONObject(pairs));
                }
            }
        }

        if (color == GameManager.Inst.MyColor)
        {
            if (GameUtil.CheckIsIcelandSelected(iceLandIndex))
            {
                Multiplayerrest();
                //this.transform.GetChild(1).gameObject.SetActive(false);
                //GameUtil.ResetAttackMode();
            }
            else
            {
                string selectedPlayerType = GameUtil.GetIcelandValueUsingKey<string>(this.transform.parent.gameObject, GameUtil.selectedIceLand[0], "playerType");
                Debug.Log("selected iceland type: " + selectedPlayerType);

                if (selectedPlayerType == playerType && GameUtil.isAttackModeOn)
                {
                    if (GameUtil.isAttackModeOn)
                    {
                        GameUtil.attackIcelandIndex = iceLandIndex;
                        //reset timer if attacked on anyone
                        timeNotAttacked = 0;
                    }
                }
                else
                {
                    //this.transform.GetChild(1).gameObject.SetActive(true);
                    //GameUtil.selectedIceLand = new int[20];

                    //GameUtil.selectedIceLand[0] = iceLandIndex;
                    print("-=-=-=->Attack3333333-=-=->" + gameObject.name);
                    this.transform.GetChild(1).gameObject.SetActive(true);
                    this.transform.GetChild(1).gameObject.GetComponent<Image>().fillAmount = 1;
                    // GameUtil.selectedIceLand = new int[20];

                    GameUtil.numberofselected++;
                    GameUtil.selectedIceLand[GameUtil.numberofselected - 1] = iceLandIndex;

                    Debug.Log("Iceland selection " + GameUtil.selectedIceLand[0]+GameUtil.selectedIceLand.Length);

                    GameUtil.isAttackModeOn = true;

                }
            }
        }
        else /*if (playerType == "computer")*/
        {
            if (GameUtil.isAttackModeOn)
            {
                GameUtil.attackIcelandIndex = iceLandIndex;

                //reset timer if attacked on anyone
                timeNotAttacked = 0;
            }
        }
    }


    // will be used to increment planes automatically
    IEnumerator TeamIncrement(float waitForTime)
    {
        yield return new WaitForSeconds(waitForTime);
        if (totalPlane > 0 && !isStatic)
        {

            //if (timeNotAttacked < 60)
            {
                if (totalPlane >= MyLimit)
                {
                    //totalPlane = MyLimit;
                }
                else
                {
                    totalPlane++;
                }
                //print("MyLimit " + MyLimit);
                GetComponentInChildren<TMP_Text>().text = "" + totalPlane;

                //increment this to keep tracj of how much time player was idle
                timeNotAttacked++;
            }
        }
        StartCoroutine(TeamIncrement(Random.Range(0.8f, 1.5f)));
    }

    IEnumerator TeamIncrementMulti(float waitForTime)
    {
        yield return new WaitForSeconds(waitForTime);
        if (totalPlane >= 0 && !isStatic)
        {
            //print("TeamIncrementMulti ");

            if (totalPlane >= MyLimit)
            {
                //totalPlane = MyLimit;
            }
            else
            {
                totalPlane++;
            }
            GetComponentInChildren<TMP_Text>().text = "" + totalPlane;

            //if (timeNotAttacked < 60)
            {
                //increment this to keep tracj of how much time player was idle
                timeNotAttacked++;
            }
        }
        StartCoroutine(TeamIncrementMulti(1));
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateIceLand();
    }

    //Update the text and the totalplane
    public void Updatetotalplane(int Totalplane , bool isstatic,bool isfree,string TeamCode,string color1,string PlayerType)
    {
        GetComponentInChildren<TMP_Text>().text = "" + Totalplane;
        totalPlane = Totalplane;
        isStatic = isstatic;
        isFree = isfree;
        teamCode = TeamCode;
        color = color1;
        playerType = PlayerType;

    }

    //It there is a multipule selected iceland and that it selected to attack on itself
    //So than every thing get reset
    void Singleplayerreset()
    {
        for (int i = 0; i < GameUtil.numberofselected; i++)
        {
            GameObject attackFrom = GamePlayManagerSingle.inst.mainSpawnContainer.transform.GetChild(GameUtil.selectedIceLand[i]).gameObject;
            attackFrom.transform.GetChild(1).gameObject.SetActive(false);
            print("-=-=-=->Attack111111-=-=->" + gameObject.name);
            attackFrom.transform.GetChild(1).gameObject.GetComponent<Image>().fillAmount = 0;
        }
        GameUtil.ResetAttackMode();
    }

    void Multiplayerrest()
    {
        for (int i = 0; i < GameUtil.numberofselected; i++)
        {
            GameObject attackFrom = GamePlayManagerMultiPlayer.inst.mainSpawnContainer.transform.GetChild(GameUtil.selectedIceLand[i]).gameObject;
            attackFrom.transform.GetChild(1).gameObject.SetActive(false);
            print("-=-=-=->Attack111111-=-=->" + gameObject.name);
            attackFrom.transform.GetChild(1).gameObject.GetComponent<Image>().fillAmount = 0;
        }
        GameUtil.ResetAttackMode();
    }
    
}

