using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPanelManager : MonoBehaviour
{
    public GameObject settingsPanel;
    public GameObject recordPanel;
    public GameObject sendMessagePanel;

    void Start()
    {
        settingsPanel.SetActive(false);
        recordPanel.SetActive(false);
        sendMessagePanel.SetActive(false);
    }

    public void ShowPanel(GameObject panel)
    {
        panel.SetActive(true);
    }

    public void HidePanel(GameObject panel)
    {
        panel.SetActive(false);
    }
}
