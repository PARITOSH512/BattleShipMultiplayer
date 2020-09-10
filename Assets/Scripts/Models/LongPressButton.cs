using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Applications.Utils;

public class LongPressButton : MonoBehaviour, IPointerDownHandler,IPointerUpHandler
{
    private bool pointerDown;
    private float pointerDownTimer;
    [SerializeField]private bool longpresscomplete;

    public float requiretimeHold;

    public UnityEvent OnLongClick;

    [SerializeField] private Iceland iceland;
    [SerializeField] private Image fillimage; 

    public void OnPointerDown(PointerEventData eventData)
    {
        fillimage.gameObject.SetActive(true);
        fillimage.fillAmount = 0;
        pointerDown = true;
        Debug.Log("OnpointerDown" + gameObject.name+GameUtil.isAttackModeOn);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Reset();
        //if(!longpresscomplete)
//{
            if (SceneManager.GetActiveScene().name == ("MultiPlayerGamePlayScene"))
                iceland.AttackManagerMulti();
            else
                iceland.AttackManager();
     //   }
       // longpresscomplete = false;
        Debug.Log("OnpointerUp" + gameObject.name);
    }

    private void Reset()
    {
        pointerDown = false;
        pointerDownTimer = 0;
        fillimage.fillAmount = pointerDownTimer / requiretimeHold;
        
    }

    private void Awake()
    {
        iceland = GetComponent<Iceland>();
        fillimage = transform.GetChild(1).GetComponent<Image>();

    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().name == ("MultiPlayerGamePlayScene"))
        {
            if(GameManager.Inst.MyColor == iceland.color)
            {
                if (pointerDown)
                {
                    pointerDownTimer += Time.deltaTime;
                    if (pointerDownTimer >= requiretimeHold)
                    {
                        print("-=-pointerDownTimer=->" + pointerDownTimer);
                        GameUtil.isAttackModeOn = false;
                        //if (OnLongClick != null)
                        //{
                        //    OnLongClick.Invoke();

                        //    longpresscomplete = true;
                        //}
                        // Reset();
                    }
                    fillimage.fillAmount = pointerDownTimer / requiretimeHold;
                }
            }

        }
        else
        {
            if (iceland.playerType == "human")
            {
                if (pointerDown)
                {
                    pointerDownTimer += Time.deltaTime;
                    if (pointerDownTimer >= requiretimeHold)
                    {
                        print("-=-pointerDownTimer=->" + pointerDownTimer);
                        GameUtil.isAttackModeOn = false;
                        //if (OnLongClick != null)
                        //{
                        //    OnLongClick.Invoke();

                        //    longpresscomplete = true;
                        //}
                        // Reset();
                    }
                    fillimage.fillAmount = pointerDownTimer / requiretimeHold;
                }
            }
        }
    }
}
