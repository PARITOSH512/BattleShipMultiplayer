using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyEmoji : MonoBehaviour
{
    public Image MyImage;
    public float Speed;

    private void Awake()
    {
        MyImage = this.GetComponent<Image>();
    }
    // Start is called before the first frame update
    void Start()
    {

    }
    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.up * Speed * Time.deltaTime);
    }
}
