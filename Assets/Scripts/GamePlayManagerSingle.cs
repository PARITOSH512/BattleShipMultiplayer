using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Application.Modal;
using Application;
using Applications.Utils;
using TMPro;
using UnityEngine.SceneManagement;

public class GamePlayManagerSingle : MonoBehaviour
{
    public static GamePlayManagerSingle inst;

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

    public Image WinPlane;
    public Image LostPlane;
    public GameObject Emoji;



    private void Awake()
    {
        inst = this;
        PauseScreen = GameObject.Find("PauseScreen");
        SoundObject = GameObject.Find("SoundStatus");
        WinPlane = GameObject.Find("WinPlane").GetComponent<Image>();
        LostPlane = GameObject.Find("LostPlane").GetComponent<Image>();

    }

    // Start is called before the first frame update
    void Start()
    {
        SoundOnOff();

        GameUtil.selectedIceLand = new int[20];

        //get player data
        string playerDataString = PlayerPrefs.GetString("singlePlayerGame");

        PlayerDetails playerDetails = JsonUtility.FromJson<PlayerDetails>(playerDataString);

        //set number of players
        numberOfPlayers = playerDetails.allUsers.Length;

        //logic to select one spwan container
        int spawnContainerIndex = Random.Range(0, spawnPointContainers.Length);

        spawnPointContainers[spawnContainerIndex].SetActive(true);

        mainSpawnContainer = spawnPointContainers[spawnContainerIndex];

        GameUtil.staticSpawnContainer = mainSpawnContainer;

        int totalChild = mainSpawnContainer.transform.childCount;
        int activated = 0;

        numberOfIcelandToGive = Random.Range(10, 20);
        Debug.Log("numberOfIcelandToGive: " + numberOfIcelandToGive);

        activatedIcelands = new int[numberOfIcelandToGive];

        do
        {
            int activateThisIndex = Random.Range(0, totalChild);

            if (!mainSpawnContainer.transform.GetChild(activateThisIndex).gameObject.activeInHierarchy)
            {
                int planeToBeSet = Random.Range(10, 30);

                mainSpawnContainer.transform.GetChild(activateThisIndex).gameObject.GetComponent<Iceland>().totalPlane = planeToBeSet;
                mainSpawnContainer.transform.GetChild(activateThisIndex).gameObject.GetComponent<Iceland>().isStatic = true;
                mainSpawnContainer.transform.GetChild(activateThisIndex).gameObject.GetComponent<Iceland>().isFree = true;
                mainSpawnContainer.transform.GetChild(activateThisIndex).gameObject.GetComponent<Iceland>().playerType = "none";
                mainSpawnContainer.transform.GetChild(activateThisIndex).gameObject.GetComponent<Iceland>().iceLandIndex = activateThisIndex;

                activatedIcelands[activated] = activateThisIndex;
                mainSpawnContainer.transform.GetChild(activateThisIndex).gameObject.SetActive(true);
                Allplanet.Add(mainSpawnContainer.transform.GetChild(activateThisIndex).gameObject);
                activated++;
            }
            //} while (activated < numberOfPlayers * numberOfIcelandToGive);
        } while (activated < numberOfIcelandToGive);

        int i = 0;

        do
        {
            int activateThisIndex = Random.Range(0, activatedIcelands.Length);

            if (mainSpawnContainer.transform.GetChild(activatedIcelands[activateThisIndex]).gameObject.GetComponent<Iceland>().playerCode.Length == 0)
            {
                int planeToBeSet = Random.Range(20, 30);

                mainSpawnContainer.transform.GetChild(activatedIcelands[activateThisIndex]).gameObject.GetComponent<Iceland>().playerCode = playerDetails.allUsers[i].name;
                mainSpawnContainer.transform.GetChild(activatedIcelands[activateThisIndex]).gameObject.GetComponent<Iceland>().teamCode = playerDetails.allUsers[i].team;
                mainSpawnContainer.transform.GetChild(activatedIcelands[activateThisIndex]).gameObject.GetComponent<Iceland>().playerType = playerDetails.allUsers[i].type;
                mainSpawnContainer.transform.GetChild(activatedIcelands[activateThisIndex]).gameObject.GetComponent<Iceland>().color = playerDetails.allUsers[i].color;
                mainSpawnContainer.transform.GetChild(activatedIcelands[activateThisIndex]).gameObject.GetComponent<Iceland>().totalPlane = planeToBeSet;
                mainSpawnContainer.transform.GetChild(activatedIcelands[activateThisIndex]).gameObject.GetComponent<Iceland>().isFree = false;
                mainSpawnContainer.transform.GetChild(activatedIcelands[activateThisIndex]).gameObject.GetComponent<Iceland>().isStatic = false;

                //human
                if (mainSpawnContainer.transform.GetChild(activatedIcelands[activateThisIndex]).gameObject.GetComponent<Iceland>().playerType == "human")
                {
                    mainSpawnContainer.transform.GetChild(activatedIcelands[activateThisIndex]).gameObject.GetComponent<Iceland>().totalPlane = Random.Range(25, 30);
                }

                //enable top header player visibility
                if (playerDetails.allUsers[i].color == "red")
                    if (redPlayer != null)
                        redPlayer.SetActive(true);

                if (playerDetails.allUsers[i].color == "yellow")
                    if (yellowPlayer != null)
                        yellowPlayer.SetActive(true);

                if (playerDetails.allUsers[i].color == "green")
                    if (greenPlayer != null)
                        greenPlayer.SetActive(true);

                if (playerDetails.allUsers[i].color == "blue")
                    if (bluePlayer != null)
                        bluePlayer.SetActive(true);

                i++;
            }
        } while (i < numberOfPlayers);

        GameUtil.ResetAttackMode();
        GameUtil.activatedIcelands = activatedIcelands;

        for (int j = 0; j < Allplanet.Count; j++)
        {
            //print(Allplanet[i].GetComponent<Iceland>().playerType);
            if (Allplanet[j].GetComponent<Iceland>().playerType.Equals("computer"))
            {
                ComputerPlayerList.Add(1);
            }
        }

        GameManager.Inst.gameMode = GameMode.GamePlaying;
    }

    // Update is called once per frame
    void Update()
    {

        //if (Input.GetMouseButtonDown(0))
        //{
        //    for (int i = 0; i < mainSpawnContainer.transform.childCount; i++)
        //    {
        //        print("<color> ♥ ♥ ♣ ♣ ♦ ♦</color>" + mainSpawnContainer.transform.childCount);
        //        mainSpawnContainer.transform.GetChild(i).transform.GetChild(1).gameObject.SetActive(false);
        //        //GameUtil.ResetAttackMode();
        //    }
        //}

        if (GameUtil.isAttackModeOn && GameUtil.attackIcelandIndex != -1 )
        {
            for (int i = 0; i < GameUtil.numberofselected; i++)
            {
                if (GameUtil.CheckIsIcelandSelected(GameUtil.selectedIceLand[i]))
                {
                    print("-=-=-=-GameUtil.attackIcelandIndex=-=->" + GameUtil.attackIcelandIndex);
                    //get both object and store into one object
                    GameObject attackOn = mainSpawnContainer.transform.GetChild(GameUtil.attackIcelandIndex).gameObject;
                    GameObject attackFrom = mainSpawnContainer.transform.GetChild(GameUtil.selectedIceLand[i]).gameObject;

                    #region Send Attacking planes
                    //get number of planes to be removed
                    int removePlane = 0;
                    if (attackFrom.GetComponent<Iceland>().totalPlane > 1 && attackFrom.GetComponent<Iceland>().playerType == "human")
                    {
                        //deactivated activated round iceland border of attackFrom iceland
                        attackFrom.transform.GetChild(1).gameObject.GetComponent<Image>().fillAmount=0;

                        removePlane = Mathf.RoundToInt(attackFrom.GetComponent<Iceland>().totalPlane / 2);
                        attackFrom.GetComponent<Iceland>().totalPlane = removePlane;    //deduct plane to e removed from attackFrom iceland

                        int attackerTotalPlane = attackOn.GetComponent<Iceland>().totalPlane;

                        //generate one plane object and assign to object
                        GameObject planeObject = Instantiate(planePrefab, GameObject.Find("AttackingPlaneTransform").transform/*mainSpawnContainer.gameObject.transform*/);

                        //set position of plane to attackFrom's position
                        planeObject.transform.position = attackFrom.transform.position;

                        //assign plane to be remove from attackTo iceland and allow to start animation moving towards attackTo object
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

                        //planeObject.transform.SetSiblingIndex();

                        //start attacking
                        GameUtil.GenerateAttackPlane(planeObject, attackFrom.transform, attackOn.transform);

                    }
                }
            }
            //reset attack
            GameUtil.ResetAttackMode();

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

        if (TeamOne.Count == 0)
        {
            print("<Color=red><b>!♦ TeamOne lost ♦!</b></Color>");
            GameUtil.isAttackModeOn = false;
            GameObject.Find("Winner").transform.localScale = Vector3.one;
            GameObject.Find("WinnerTxt").GetComponent<TMP_Text>().text = "You lose";
            GameObject.Find("WinnerTxt").GetComponent<TMP_Text>().color = Color.red;
            LostPlane.transform.localScale = Vector3.one;
            WinPlane.transform.localScale = Vector3.zero;

            GameObject.Find("Percentage").GetComponent<Text>().text = "0%";
            GameObject.Find("PercentageSlider").GetComponent<Image>().fillAmount = 0f;

            if (ComputerPlayerList.Count == 1)
            {
                print("<color><b> ♥ Rank 2 ♥ </b></color>");
                GameObject.Find("Rank").GetComponent<Text>().text = "YOUR RANK 2";


            }
            else if (ComputerPlayerList.Count == 2)
            {
                print("<color><b> ♥ Rank 3 ♥ </b></color>");
                GameObject.Find("Rank").GetComponent<Text>().text = "YOUR RANK 3";
            }
            else if (ComputerPlayerList.Count == 3)
            {
                print("<color><b> ♥ Rank 4 ♥ </b></color>");
                GameObject.Find("Rank").GetComponent<Text>().text = "YOUR RANK 4";
            }
            GameManager.Inst.gameMode = GameMode.GameOver;
        }

        if (ComputerPlayerList.Count < 2 && TeamTwo.Count == 0 && yellowPlayer.activeSelf)
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
        else if (ComputerPlayerList.Count < 3 && TeamTwo.Count == 0 && TeamThree.Count == 0 && yellowPlayer.activeSelf)
        {
            print("<Color=green><b>!♦ TeamThree Player lost ♦!</b></Color>");
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
        else if (ComputerPlayerList.Count < 4 && TeamTwo.Count == 0 && TeamThree.Count == 0 && TeamFour.Count == 0 && bluePlayer.activeSelf)
        {
            print("<Color=blue><b>!♦ TeamFour Player lost ♦!</b></Color>");
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

    public void PauseBtn()
    {
        Time.timeScale = 0;
        GameObject.Find("PlayPauseStatus").GetComponent<Image>().sprite = PasueSp;
        PauseScreen.transform.localScale = Vector3.one;
        AudioManager.instance.Btn();
    }

    public void Resume()
    {
        Time.timeScale = 1;
        GameObject.Find("PlayPauseStatus").GetComponent<Image>().sprite = PlaySp;
        PauseScreen.transform.localScale = Vector3.zero;
        AudioManager.instance.Btn();
    }

    public void QuitGame()
    {
        print("<Color><b> Quit Game Call </b></Color>");
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenuScreen");
        AudioManager.instance.Btn();
    }

    public void Replay()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("SingleGamePlayScene");
        AudioManager.instance.Btn();
    }

    public void NewGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("SingleGameScene");
        AudioManager.instance.Btn();
    }

    public void ExitGame()
    {
        Time.timeScale = 1;
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
    bool pause1;
    int count = 0;
    private void OnApplicationPause(bool pause)
    {

        pause1 = pause;
        running = appause();
        if(pause1)
        StartCoroutine(appause());
        //lol();

        //if (pause)
        //{

        //    print("-=-=-=-Apppplication Pause" + pause1 + count + gameObject.name);
        //    count++;
        //}
        //else
        //{
        //    print("-=-=-=-Apppplication Pause" + pause1 + count + gameObject.name);
        //}
        //while (pause1)
        //{
        //    print("-=-=-=-Apppplication Pause" + pause);
        //}
    }
    public IEnumerator running;
    IEnumerator appause()
    {
        yield return new WaitForEndOfFrame();
        print("-=-=-=-Apppplication Pause" + pause1 + count + gameObject.name);
        StartCoroutine(appause());

    }
   
    private void OnApplicationFocus(bool focus)
    {
        pause1 = false;
        StopAllCoroutines();
        print("-=-=-=-Apppplication focus"+focus);
    }
}



