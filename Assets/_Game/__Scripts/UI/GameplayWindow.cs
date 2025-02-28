using System;
using TMPro;
using UnityEngine;

public class GameplayWindow : UIElement
{
    [SerializeField]
    private TextMeshProUGUI _infoText;

    public void ShowSeeingInfo(IInteractable uiInfo)
    {
        Show();
        _infoText.text = uiInfo.UiLabel;
        _infoText.gameObject.SetActive(true);
    }
    
    public void HideSeeingInfo()
    {
        Hide();
        _infoText.gameObject.SetActive(false);
    }
}