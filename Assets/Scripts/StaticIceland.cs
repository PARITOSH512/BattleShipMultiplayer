using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Applications.Utils;
using SocketIO;
using UnityEngine.SceneManagement;

public class StaticIceland : MonoBehaviour
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
    public bool isMyIcaland;

    public int myLimit;

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
        //
        //I am Small Iceland 
        //    MyLimit = GameManager.Inst.smallIcelandLimit;
        //}

    }
    // Start is called before the first frame update
    void Start()
    {

        if (GameObject.FindGameObjectWithTag("SocketIO") != null && socketIOComponent == null)
            socketIOComponent = GameObject.FindGameObjectWithTag("SocketIO").GetComponent<SocketIOComponent>();


        StartCoroutine(TeamIncrement(Random.Range(0.8f, 1.5f)));
        GetComponent<Button>().onClick.AddListener(AttackManager);
    }

    IEnumerator StartComputerBotAttack()
    {
        yield return new WaitForSeconds(Random.Range(3, 10));
        if (playerType == "computer" && totalPlane > Random.Range(25, 50))
        {
            int[] sortedIcelands = GameUtil.GetSortedListOfPlayers(totalPlane / 2);

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

            GameObject planeObject = Instantiate(planePrefab, GameObject.Find("AttackingPlaneTransform").transform/* GameUtil.staticSpawnContainer.gameObject.transform*/);

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
        }

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

    string selectedPlayerType;
    // will be click of iceland
    void AttackManager()
    {

        Debug.Log("icelandIndex: " + iceLandIndex + " playerType: " + playerType);

        print(GameUtil.isAttackModeOn + " form " + GameUtil.selectedIceLand[0] + " on iceLandIndex " + iceLandIndex);

        if (playerType == "human")
        {
            if (GameUtil.CheckIsIcelandSelected(iceLandIndex))
            {
                this.transform.GetChild(1).gameObject.SetActive(false);
                GameUtil.ResetAttackMode();
            }
            else
            {
                if (isMyIcaland)
                {
                    //if (this.transform.parent.gameObject.name == "SpawnPointsOne")
                    //{
                    //    selectedPlayerType = GameUtil.GetIcelandValueUsingKey<string>(this.transform.gameObject, GameUtil.selectedIceLand[0], "human");
                    //}
                    selectedPlayerType = "human";
                    Debug.Log(" if ♥ ♥ this.transform.parent.gameObject:  " + this.transform.parent.gameObject.name);
                    //selectedPlayerType = GameUtil.GetIcelandValueUsingKey<string>(this.transform.parent.gameObject, GameUtil.selectedIceLand[0], "human");
                }
                else
                {
                    Debug.Log("else ♠ ♠ this.transform.parent.gameObject:  " + this.transform.parent.gameObject.name);
                    selectedPlayerType = GameUtil.GetIcelandValueUsingKey<string>(this.transform.parent.gameObject, GameUtil.selectedIceLand[0], "playerType");
                }
                Debug.Log("selected iceland type: " + selectedPlayerType);

                if (selectedPlayerType == playerType && GameUtil.isAttackModeOn)
                {
                    if (GameUtil.isAttackModeOn)
                    {
                        GameUtil.attackIcelandIndex = iceLandIndex;
                        //reset timer if attacked on anyone
                        timeNotAttacked = 0;
                        Debug.Log("GameUtil.isAttackModeOn: " + GameUtil.isAttackModeOn);
                    }
                }
                else
                {
                    this.transform.GetChild(1).gameObject.SetActive(true);
                    GameUtil.selectedIceLand = new int[20];

                    GameUtil.selectedIceLand[0] = iceLandIndex;

                    Debug.Log("Iceland selection " + GameUtil.selectedIceLand[0]);

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


    void AttackManagerMulti()
    {

        Debug.Log("icelandIndex: " + iceLandIndex + " playerType: " + playerType);
        print(GameUtil.isAttackModeOn + " form " + GameUtil.selectedIceLand[0] + " on iceLandIndex " + iceLandIndex);

        if (color == GameManager.Inst.MyColor)
        {
            if (GameUtil.CheckIsIcelandSelected(iceLandIndex))
            {
                this.transform.GetChild(1).gameObject.SetActive(false);
                GameUtil.ResetAttackMode();
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
                    this.transform.GetChild(1).gameObject.SetActive(true);
                    GameUtil.selectedIceLand = new int[20];

                    GameUtil.selectedIceLand[0] = iceLandIndex;

                    Debug.Log("Iceland selection " + GameUtil.selectedIceLand[0]);

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

                if (totalPlane >= myLimit)
                {
                    //totalPlane = myLimit;
                }
                else
                {
                    totalPlane++;
                }
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
        if (totalPlane > 0 && !isStatic)
        {
            totalPlane++;
            if (totalPlane > myLimit)
            {
                totalPlane = myLimit;
            }
            GetComponentInChildren<TMP_Text>().text = "" + totalPlane;

            if (timeNotAttacked < 60)
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
}

