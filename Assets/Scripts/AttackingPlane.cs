using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AttackingPlane : MonoBehaviour
{

    public string AttackingPlane_ID="null";
    public string playerCode;
    public bool isFree;
    public bool isStatic;
    public bool placeAtInitial = true;
    public string teamCode;
    public string playerType;
    public string color;
    public int totalPlane;
    public int iceLandIndex;
    public float speed;
    public GameObject toObject;

    // Start is called before the first frame update
    void Start()
    {
        if (color == "red")
            GetComponent<Image>().color = Color.red;

        if (color == "yellow")
            GetComponent<Image>().color = Color.yellow;

        if (color == "green")
            GetComponent<Image>().color = Color.green;

        if (color == "blue")
            GetComponent<Image>().color = Color.blue;

        GetComponentInChildren<TMP_Text>().text = "" + totalPlane;
    }

    // Update is called once per frame
    void Update()
    {

        if (GameManager.Inst.gameMode == GameMode.GamePlaying)//           
        {
            Dictionary<string, string> pairs = new Dictionary<string, string>();
            pairs.Add("plane_id", AttackingPlane_ID);
            //will animate plane from to to destination iceland
            if (placeAtInitial)
            {
                float step = speed * Time.deltaTime;
                


                toObject.transform.GetChild(1).gameObject.SetActive(false);

                transform.position = Vector3.MoveTowards(transform.position, toObject.transform.position, 0.5f * Time.deltaTime);

                // will be performed while getting same place matching with destination iceland
                if (transform.position == toObject.transform.position)
                {
                    placeAtInitial = false;
                    int temptotalplane = 0;
                    string tempplayercode="";
                    string tempcolor="";
                    string tempplayerType="";
                    bool tempisFree = false;
                    bool tempisStatic = false;
                    string tempteamcode="";
                    // if reached to the destination place, perfom attack logic on that iceland


                    if (SceneManager.GetActiveScene().name.Equals("Tutorial"))
                    {
                        int attackerTotalPlane = toObject.GetComponent<StaticIceland>().totalPlane;
                        if (/*toObject.GetComponent<Iceland>().playerType != playerType &&*/ (toObject.GetComponent<StaticIceland>().teamCode != teamCode || toObject.GetComponent<StaticIceland>().teamCode == "0"))
                        {
                            AudioManager.instance.PlayExp();
                            if (toObject.GetComponent<StaticIceland>().totalPlane > totalPlane)
                            {
                                
                                //can not own iceland still
                                print("<Color=cyan><b> can not own iceland still </b></Color>");
                                toObject.GetComponent<StaticIceland>().totalPlane -= totalPlane;

                            }
                            else if ((attackerTotalPlane - totalPlane) == 0)
                            {
                                //iceland will get empty
                                toObject.GetComponent<StaticIceland>().totalPlane = Mathf.Abs(attackerTotalPlane - totalPlane);
                                toObject.GetComponent<StaticIceland>().playerCode = "";
                                toObject.GetComponent<StaticIceland>().teamCode = "";
                                toObject.GetComponent<StaticIceland>().playerType = "none";
                                toObject.GetComponent<StaticIceland>().color = "";
                                toObject.GetComponent<StaticIceland>().isFree = true;
                                toObject.GetComponent<StaticIceland>().isStatic = true;
                            }
                            else
                            {
                                //iceland will be owned as attacking plane have more planes that destination iceland
                                print("<Color><b> NAME IS HERE ::: </b></Color>" + playerCode);
                                int temp = Mathf.Abs(attackerTotalPlane - totalPlane);

                                //   toObject.GetComponent<Iceland>().totalPlane = Mathf.Abs(attackerTotalPlane - totalPlane);
                                if (temp >= toObject.GetComponent<StaticIceland>().myLimit)
                                {
                                    temp = toObject.GetComponent<StaticIceland>().myLimit;
                                }
                                toObject.GetComponent<StaticIceland>().totalPlane = temp;
                                toObject.GetComponent<StaticIceland>().playerCode = playerCode;
                                toObject.GetComponent<StaticIceland>().teamCode = teamCode;
                                toObject.GetComponent<StaticIceland>().playerType = playerType;
                                toObject.GetComponent<StaticIceland>().color = color;
                                toObject.GetComponent<StaticIceland>().isFree = false;
                                toObject.GetComponent<StaticIceland>().isStatic = false;
                                toObject.GetComponent<StaticIceland>().isMyIcaland = true;

                            }
                        }
                        else
                        {
                            // planes will be incremented as it's friendly plane placement.
                            print("<Color=cyan><b> planes will be incremented as it's friendly plane placement. </b></Color>");
                            // toObject.GetComponent<StaticIceland>().totalPlane = Mathf.Abs(attackerTotalPlane + totalPlane);
                            int checkLimit = Mathf.Abs(attackerTotalPlane + totalPlane);
                            if (checkLimit >= toObject.GetComponent<StaticIceland>().myLimit)
                            {
                                //checkLimit = toObject.GetComponent<StaticIceland>().myLimit;
                            }
                            toObject.GetComponent<StaticIceland>().totalPlane = checkLimit;
                        }
                    }
                    else
                    {
                        int attackerTotalPlane = toObject.GetComponent<Iceland>().totalPlane;
                        

                        if ((toObject.GetComponent<Iceland>().teamCode != teamCode || toObject.GetComponent<Iceland>().teamCode == "0"))
                        {
                            AudioManager.instance.PlayExp();
                            if (toObject.GetComponent<Iceland>().totalPlane > totalPlane)
                            {
                                //can not own iceland still
                               
                                //toObject.GetComponent<Iceland>().totalPlane -= totalPlane;

                                temptotalplane = toObject.GetComponent<Iceland>().totalPlane - totalPlane;
                                tempplayercode = toObject.GetComponent<Iceland>().playerCode;
                                tempteamcode = toObject.GetComponent<Iceland>().teamCode;
                                tempplayerType = toObject.GetComponent<Iceland>().playerType;
                                tempcolor = toObject.GetComponent<Iceland>().color;
                                tempisFree = toObject.GetComponent<Iceland>().isFree;
                                tempisStatic = toObject.GetComponent<Iceland>().isStatic;
                              //  print("<Color=cyan><b> can not own iceland still </b></Color>"+tempplayerType+);


                            }
                            else if ((attackerTotalPlane - totalPlane) == 0)
                            {
                                //iceland will get empty
                                //toObject.GetComponent<Iceland>().totalPlane = Mathf.Abs(attackerTotalPlane - totalPlane);
                                //toObject.GetComponent<Iceland>().playerCode = "";
                                //toObject.GetComponent<Iceland>().teamCode = "";
                                //toObject.GetComponent<Iceland>().playerType = "none";
                                //toObject.GetComponent<Iceland>().color = "";
                                //toObject.GetComponent<Iceland>().isFree = true;
                                //toObject.GetComponent<Iceland>().isStatic = true;
                                temptotalplane = 0;
                                tempplayercode = "";
                                tempteamcode = "";
                                tempplayerType = "none";
                                tempcolor = "";
                                tempisFree = true;
                                tempisStatic = true;

                            }
                            else
                            {
                                //iceland will be owned as attacking plane have more planes that destination iceland
                                print("<Color><b> NAME IS HERE ::: </b></Color>" + playerCode);
                                //toObject.GetComponent<Iceland>().totalPlane = Mathf.Abs(attackerTotalPlane - totalPlane);
                                //toObject.GetComponent<Iceland>().playerCode = playerCode;
                                //toObject.GetComponent<Iceland>().teamCode = teamCode;
                                //toObject.GetComponent<Iceland>().playerType = playerType;
                                //toObject.GetComponent<Iceland>().color = color;
                                //toObject.GetComponent<Iceland>().isFree = false;
                                //toObject.GetComponent<Iceland>().isStatic = false;

                                temptotalplane =  Mathf.Abs(attackerTotalPlane - totalPlane);
                                tempplayercode = playerCode;
                                tempteamcode = teamCode;
                                tempplayerType = playerType;
                                tempcolor = color;
                                tempisFree = false;
                                tempisStatic = false;
                                //GameManager.Inst.attackCounter = 0;
                            }
                        }
                        else
                        {
                            // planes will be incremented as it's friendly plane placement.
                            print("<Color=cyan><b> planes will be incremented as it's friendly plane placement. </b></Color>");
                            // toObject.GetComponent<Iceland>().totalPlane = Mathf.Abs(attackerTotalPlane + totalPlane);
                            int checklimit = Mathf.Abs(attackerTotalPlane + totalPlane);
                            if (checklimit >= toObject.GetComponent<Iceland>().MyLimit)
                            {
                                //checklimit = toObject.GetComponent<Iceland>().MyLimit;
                            }
                            // toObject.GetComponent<Iceland>().totalPlane = checklimit;
                            temptotalplane = checklimit;
                            tempplayercode = toObject.GetComponent<Iceland>().playerCode;
                            tempteamcode = toObject.GetComponent<Iceland>().teamCode;
                            tempplayerType = toObject.GetComponent<Iceland>().playerType;
                            tempcolor = toObject.GetComponent<Iceland>().color;
                            tempisFree = toObject.GetComponent<Iceland>().isFree;
                            tempisStatic = toObject.GetComponent<Iceland>().isStatic;
                        }

                        //adding the data to send to the server
                        pairs.Add("icelandindex", toObject.GetComponent<Iceland>().iceLandIndex.ToString());
                        pairs.Add("totalPlane", temptotalplane.ToString());
                        pairs.Add("playerCode", tempplayercode.ToString());
                        pairs.Add("teamCode", tempteamcode.ToString());
                        pairs.Add("playerType", tempplayerType.ToString());
                        pairs.Add("color", tempcolor.ToString());
                        pairs.Add("isFree", tempisFree.ToString());
                        pairs.Add("isStatic", tempisStatic.ToString());
                    }

                    if (SceneManager.GetActiveScene().name.Equals("MultiPlayerGamePlayScene"))
                    {
                        //GamePlayManagerMultiPlayer.inst.AddData();
                        GamePlayManagerMultiPlayer.inst.serverObject.Remove(AttackingPlane_ID);
                        print("data" + new JSONObject(pairs));
                        GamePlayManagerMultiPlayer.inst.socketIOComponent.Emit("AttackingPlaneReach", new JSONObject(pairs));
                    }
                    else if (SceneManager.GetActiveScene().name.Equals("Tutorial"))
                    {

                        Tutorial.inst.AddData();
                    }
                    else
                    {
                        GamePlayManagerSingle.inst.AddData();
                        toObject.GetComponent<Iceland>().totalPlane = temptotalplane;
                        toObject.GetComponent<Iceland>().playerCode = tempplayercode;
                        toObject.GetComponent<Iceland>().teamCode = tempteamcode;
                        toObject.GetComponent<Iceland>().playerType = tempplayerType;
                        toObject.GetComponent<Iceland>().color = tempcolor;
                        toObject.GetComponent<Iceland>().isFree = tempisFree;
                        toObject.GetComponent<Iceland>().isStatic = tempisStatic;
                    }
                    //GamePlayManagerSingle.inst.CheckWinner();
                    //player 

                    Destroy(this.gameObject);
                }
            }
        }
    }
}
