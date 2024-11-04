using System;
using UnityEngine;
using UnityEngine.UI;

public class UITimeManager : MonoBehaviour
{
    public Text timeText;

    void Start()
    {
        var timeNow = DateTime.Now.ToString("hh:mm tt");
        timeText.text = timeNow;
    }

    void Update()
    {
        var timeNow = DateTime.Now.ToString("hh:mm tt");
        timeText.text = timeNow;
    }
}
