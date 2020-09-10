using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Application.Modal;
using Application;
using Applications.Utils;
using TMPro;
using UnityEngine.SceneManagement;

public class Tutorial : MonoBehaviour
{
    public static Tutorial inst;
    public GameObject[] spawnPointContainers;

    private GameObject mainSpawnContainer;

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

    public bool isSound;
    int activateThisIndex;
    private void Awake()
    {
        inst = this;

        PauseScreen = GameObject.Find("PauseScreen");
        SoundObject = GameObject.Find("SoundStatus");
        WinPlane = GameObject.Find("WinPlane").GetComponent<Image>();
        LostPlane = GameObject.Find("LostPlane").GetComponent<Image>();

        SoundOnOff();
    }


    // Start is called before the first frame update
    void Start()
    {

        GameUtil.selectedIceLand = new int[20];


        //set number of players
        numberOfPlayers = 2;

        //logic to select one spwan container
        int spawnContainerIndex = 0;
        spawnPointContainers[spawnContainerIndex].SetActive(true);

        mainSpawnContainer = spawnPointContainers[spawnContainerIndex];

        GameUtil.staticSpawnContainer = mainSpawnContainer;

        int totalChild = mainSpawnContainer.transform.childCount;
        int activated = 0;

        numberOfIcelandToGive = 5;
        Debug.Log("numberOfIcelandToGive: " + numberOfIcelandToGive);

        activatedIcelands = new int[numberOfIcelandToGive];

        do
        {
            int activateThisIndex = Random.Range(0, totalChild);

            if (!mainSpawnContainer.transform.GetChild(activateThisIndex).gameObject.activeInHierarchy)
            {
                int planeToBeSet = Random.Range(10, 30);

                mainSpawnContainer.transform.GetChild(activateThisIndex).gameObject.GetComponent<StaticIceland>().totalPlane = planeToBeSet;
                mainSpawnContainer.transform.GetChild(activateThisIndex).gameObject.GetComponent<StaticIceland>().isStatic = true;
                mainSpawnContainer.transform.GetChild(activateThisIndex).gameObject.GetComponent<StaticIceland>().isFree = true;
                mainSpawnContainer.transform.GetChild(activateThisIndex).gameObject.GetComponent<StaticIceland>().playerType = "none";
                mainSpawnContainer.transform.GetChild(activateThisIndex).gameObject.GetComponent<StaticIceland>().iceLandIndex = activateThisIndex;

                activatedIcelands[activated] = activateThisIndex;
                mainSpawnContainer.transform.GetChild(activateThisIndex).gameObject.SetActive(true);
                Allplanet.Add(mainSpawnContainer.transform.GetChild(activateThisIndex).gameObject);
                activated++;
            }
            //} while (activated < numberOfPlayers * numberOfIcelandToGive);
        } while (activated < numberOfIcelandToGive);

        if (activateThisIndex == 0)
        {
            int planeToBeSet = Random.Range(20, 30);
            mainSpawnContainer.transform.GetChild(activatedIcelands[activateThisIndex]).gameObject.GetComponent<StaticIceland>().playerCode = "username";
            mainSpawnContainer.transform.GetChild(activatedIcelands[activateThisIndex]).gameObject.GetComponent<StaticIceland>().teamCode = "1";
            mainSpawnContainer.transform.GetChild(activatedIcelands[activateThisIndex]).gameObject.GetComponent<StaticIceland>().playerType = "human";
            mainSpawnContainer.transform.GetChild(activatedIcelands[activateThisIndex]).gameObject.GetComponent<StaticIceland>().color = "red";
            mainSpawnContainer.transform.GetChild(activatedIcelands[activateThisIndex]).gameObject.GetComponent<StaticIceland>().totalPlane = planeToBeSet;
            mainSpawnContainer.transform.GetChild(activatedIcelands[activateThisIndex]).gameObject.GetComponent<StaticIceland>().isFree = false;
            mainSpawnContainer.transform.GetChild(activatedIcelands[activateThisIndex]).gameObject.GetComponent<StaticIceland>().isStatic = false;

            //enable top header player visibility
            if (redPlayer != null)
                redPlayer.SetActive(true);
        }
        if (activateThisIndex == 10)
        {
            int planeToBeSet = Random.Range(20, 30);
            mainSpawnContainer.transform.GetChild(activatedIcelands[activateThisIndex]).gameObject.GetComponent<StaticIceland>().playerCode = "username";
            mainSpawnContainer.transform.GetChild(activatedIcelands[activateThisIndex]).gameObject.GetComponent<StaticIceland>().teamCode = "2";
            mainSpawnContainer.transform.GetChild(activatedIcelands[activateThisIndex]).gameObject.GetComponent<StaticIceland>().playerType = "computer";
            mainSpawnContainer.transform.GetChild(activatedIcelands[activateThisIndex]).gameObject.GetComponent<StaticIceland>().color = "yellow";
            mainSpawnContainer.transform.GetChild(activatedIcelands[activateThisIndex]).gameObject.GetComponent<StaticIceland>().totalPlane = planeToBeSet;
            mainSpawnContainer.transform.GetChild(activatedIcelands[activateThisIndex]).gameObject.GetComponent<StaticIceland>().isFree = false;
            mainSpawnContainer.transform.GetChild(activatedIcelands[activateThisIndex]).gameObject.GetComponent<StaticIceland>().isStatic = false;
            if (yellowPlayer != null)
                yellowPlayer.SetActive(true);
        }
        int i = 0;

        do
        {
            int activateThisIndex = Random.Range(0, activatedIcelands.Length);

            if (mainSpawnContainer.transform.GetChild(activatedIcelands[activateThisIndex]).gameObject.GetComponent<StaticIceland>().playerCode.Length == 0)
            {
                int planeToBeSet = Random.Range(20, 30);

                if (i == 0)
                {
                    mainSpawnContainer.transform.GetChild(activatedIcelands[activateThisIndex]).gameObject.GetComponent<StaticIceland>().playerCode = "username";
                    mainSpawnContainer.transform.GetChild(activatedIcelands[activateThisIndex]).gameObject.GetComponent<StaticIceland>().teamCode = "1";
                    mainSpawnContainer.transform.GetChild(activatedIcelands[activateThisIndex]).gameObject.GetComponent<StaticIceland>().playerType = "human";
                    mainSpawnContainer.transform.GetChild(activatedIcelands[activateThisIndex]).gameObject.GetComponent<StaticIceland>().color = "red";
                    mainSpawnContainer.transform.GetChild(activatedIcelands[activateThisIndex]).gameObject.GetComponent<StaticIceland>().totalPlane = planeToBeSet;
                    mainSpawnContainer.transform.GetChild(activatedIcelands[activateThisIndex]).gameObject.GetComponent<StaticIceland>().isFree = false;
                    mainSpawnContainer.transform.GetChild(activatedIcelands[activateThisIndex]).gameObject.GetComponent<StaticIceland>().isStatic = false;
                    mainSpawnContainer.transform.GetChild(activatedIcelands[activateThisIndex]).gameObject.GetComponent<StaticIceland>().isMyIcaland = true;
                    //enable top header player visibility
                    if (redPlayer != null)
                        redPlayer.SetActive(true);
                }
                if (i == 1)
                {
                    mainSpawnContainer.transform.GetChild(activatedIcelands[activateThisIndex]).gameObject.GetComponent<StaticIceland>().playerCode = "username";
                    mainSpawnContainer.transform.GetChild(activatedIcelands[activateThisIndex]).gameObject.GetComponent<StaticIceland>().teamCode = "2";
                    mainSpawnContainer.transform.GetChild(activatedIcelands[activateThisIndex]).gameObject.GetComponent<StaticIceland>().playerType = "computer";
                    mainSpawnContainer.transform.GetChild(activatedIcelands[activateThisIndex]).gameObject.GetComponent<StaticIceland>().color = "yellow";
                    mainSpawnContainer.transform.GetChild(activatedIcelands[activateThisIndex]).gameObject.GetComponent<StaticIceland>().totalPlane = planeToBeSet;
                    mainSpawnContainer.transform.GetChild(activatedIcelands[activateThisIndex]).gameObject.GetComponent<StaticIceland>().isFree = false;
                    mainSpawnContainer.transform.GetChild(activatedIcelands[activateThisIndex]).gameObject.GetComponent<StaticIceland>().isStatic = false;
                    if (yellowPlayer != null)
                        yellowPlayer.SetActive(true);
                }

                i++;
            }
        } while (i < numberOfPlayers);
        GameUtil.ResetAttackMode();
        GameUtil.activatedIcelands = activatedIcelands;

        for (int j = 0; j < Allplanet.Count; j++)
        {
            //print(Allplanet[i].GetComponent<Iceland>().playerType);
            if (Allplanet[j].GetComponent<StaticIceland>().playerType.Equals("computer"))
            {
                ComputerPlayerList.Add(1);
            }
            else if (Allplanet[j].GetComponent<StaticIceland>().playerType.Equals("human"))
            {
                Allplanet[j].GetComponent<StaticIceland>().isMyIcaland = true;
            }

        }
        GameObject.Find("instraction").transform.localScale = Vector3.one;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            BackButtonClick();
        }

        if (GameUtil.isAttackModeOn && GameUtil.attackIcelandIndex != -1 && GameUtil.CheckIsIcelandSelected(GameUtil.selectedIceLand[0]))
        {
            //get both object and store into one object
            GameObject attackOn = mainSpawnContainer.transform.GetChild(GameUtil.attackIcelandIndex).gameObject;
            GameObject attackFrom = mainSpawnContainer.transform.GetChild(GameUtil.selectedIceLand[0]).gameObject;


            #region Send Attacking planes
            //get number of planes to be removed
            int removePlane = 0;
            if (attackFrom.GetComponent<StaticIceland>().totalPlane > 1)
            {
                //deactivated activated round iceland border of attackFrom iceland
                attackFrom.transform.GetChild(1).gameObject.SetActive(false);

                removePlane = Mathf.RoundToInt(attackFrom.GetComponent<StaticIceland>().totalPlane / 2);
                attackFrom.GetComponent<StaticIceland>().totalPlane = removePlane;    //deduct plane to e removed from attackFrom iceland

                int attackerTotalPlane = attackOn.GetComponent<StaticIceland>().totalPlane;

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
                planeObject.GetComponent<AttackingPlane>().playerCode = attackFrom.GetComponent<StaticIceland>().playerCode;
                planeObject.GetComponent<AttackingPlane>().teamCode = attackFrom.GetComponent<StaticIceland>().teamCode;
                planeObject.GetComponent<AttackingPlane>().playerType = attackFrom.GetComponent<StaticIceland>().playerType;
                planeObject.GetComponent<AttackingPlane>().color = attackFrom.GetComponent<StaticIceland>().color;
                planeObject.GetComponent<AttackingPlane>().isFree = false;
                planeObject.GetComponent<AttackingPlane>().isStatic = false;

                //planeObject.transform.SetSiblingIndex();

                //start attacking
                GameUtil.GenerateAttackPlane(planeObject, attackFrom.transform, attackOn.transform);

                //reset attack
                GameUtil.ResetAttackMode();
            }
            #endregion
        }
    }

    public void BackButtonClick()
    {
        AudioManager.instance.Btn();
        //if game is not joined and clicked back
        ChangeScene(0);
    }
    void ChangeScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
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
            if (Allplanet[i].GetComponent<StaticIceland>().teamCode.Equals("1"))
            {
                TeamOne.Add(1);
            }
            else if (Allplanet[i].GetComponent<StaticIceland>().teamCode.Equals("2"))
            {
                TeamTwo.Add(1);
            }
            else if (Allplanet[i].GetComponent<StaticIceland>().teamCode.Equals("3"))
            {
                TeamThree.Add(1);
            }
            else if (Allplanet[i].GetComponent<StaticIceland>().teamCode.Equals("4"))
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

            if (ComputerPlayerList.Count == 1)
            {
                print("<color><b> ♥ Rank 2 ♥ </b></color>");
                GameObject.Find("Rank").GetComponent<TMP_Text>().text = "YOUR RANK 2";
            }
            else if (ComputerPlayerList.Count == 2)
            {
                print("<color><b> ♥ Rank 3 ♥ </b></color>");
                GameObject.Find("Rank").GetComponent<TMP_Text>().text = "YOUR RANK 3";
            }
            else if (ComputerPlayerList.Count == 3)
            {
                print("<color><b> ♥ Rank 4 ♥ </b></color>");
                GameObject.Find("Rank").GetComponent<TMP_Text>().text = "YOUR RANK 4";
            }
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
            GameObject.Find("Rank").GetComponent<TMP_Text>().text = "YOUR RANK 1";

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
            GameObject.Find("Rank").GetComponent<TMP_Text>().text = "YOUR RANK 1";
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
            GameObject.Find("Rank").GetComponent<TMP_Text>().text = "YOUR RANK 1";
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
        SceneManager.LoadScene("Tutorial");
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


}