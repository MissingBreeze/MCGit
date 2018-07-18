using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class TipUIControl : MonoBehaviour {

    public Text tipText;

    public Button ConfirmBtn;

    public Button CancleBtn;

    public delegate void callback(bool operate);


    void Start ()
    {
        gameObject.SetActive(false);
    }	
	
	void Update ()
    {
		
	}

    /// <summary>
    /// 打开提示界面
    /// </summary>
    /// <param name="message">提示</param>
    /// <param name="call">回调</param>
    public void SetTip(string message,callback call)
    {
        gameObject.SetActive(true);
        if (!string.IsNullOrEmpty(message))
            tipText.text = message;
        ConfirmBtn.onClick.AddListener(() =>
        {
            call(true);
            gameObject.SetActive(false);
        });

        CancleBtn.onClick.AddListener(() =>
        {
            call(false);
            gameObject.SetActive(false);
        });
    }

}
