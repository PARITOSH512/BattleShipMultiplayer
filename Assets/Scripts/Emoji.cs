using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using SocketIO;
using System;

public class Emoji : MonoBehaviour
{
    public static Emoji Inst;

    public ScrollRect ScrollView;
    public GameObject EmojiContent;
    private SocketIOComponent socketIOComponent;

    public bool isMyEmoji;
    private void Awake()
    {
        Inst = this;
        ScrollView = GameObject.Find("EmojiScrollView").GetComponent<ScrollRect>();
        ScrollView.enabled = false;
        EmojiContent = GameObject.Find("EmojiContent");

    }
    // Start is called before the first frame update
    void Start()
    {

        ScrollView.enabled = true;
        socketIOComponent = GameObject.FindGameObjectWithTag("SocketIO").GetComponent<SocketIOComponent>();

        socketIOComponent.On("Emoji", (SocketIOEvent obj) =>
        {
            Debug.Log("obj: " + obj.data.ToString());
            print("index " + obj.data.GetField("index"));
            print("Did" + obj.data.GetField("Did").ToString().Trim('"'));
            if (SystemInfo.deviceUniqueIdentifier == obj.data.GetField("Did").ToString().Trim('"'))
            {
                isMyEmoji = true;
            }
            else 
            {
                isMyEmoji = false;
            }
            string str = obj.data.GetField("index").ToString().Trim('"');
            print("index str >" + str);

            try
            {
                int a = Convert.ToInt32(str);// int.Parse(str);
                print("index  " + a);
                SendData(a);

            }
            catch (System.Exception e)
            {
                print("Exception " + e);
                throw;
            }
        });

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SendEmoji(int i)  //Send Data
    {
        print("SEND EMOJI NUMBER" + i);
        if (SceneManager.GetActiveScene().name.Equals("MultiPlayerGamePlayScene"))
        {
            Dictionary<string, string> pairs = new Dictionary<string, string>();
            pairs.Add("index", i.ToString());
            pairs.Add("Did", SystemInfo.deviceUniqueIdentifier);
            socketIOComponent.Emit("Emoji", new JSONObject(pairs), (JSONObject obj) =>
            {
                Debug.Log("NUmber index Send" + pairs.ToString());
            });
        }
    }

    public void SendData(int i)
    {
        if (isMyEmoji)
        {
            GameObject Clone = Instantiate(Resources.Load("EmojiPrefab", typeof(GameObject)) as GameObject, GameObject.Find("From").transform);
            print(Resources.Load("Emoji/" + "emj" + i) as Sprite);
            Clone.GetComponent<MyEmoji>().MyImage.sprite = Resources.Load("Emoji/" + "emj" + i, typeof(Sprite)) as Sprite;
            Clone.GetComponent<MyEmoji>().Speed = 1f;
            Destroy(Clone, 1f);
        }
        else
        {
            GameObject Clone = Instantiate(Resources.Load("EmojiPrefab", typeof(GameObject)) as GameObject, GameObject.Find("ToHereUser (1)").transform);
            print(Resources.Load("Emoji/" + "emj" + i) as Sprite);
            Clone.GetComponent<MyEmoji>().MyImage.sprite = Resources.Load("Emoji/" + "emj" + i, typeof(Sprite)) as Sprite;
            Clone.GetComponent<MyEmoji>().Speed = -1f;
            Destroy(Clone, 1f);
        }
        //Invoke("FlagReset", 1f);
    }
    void FlagReset()
    {
        isMyEmoji = false;
    }

}
