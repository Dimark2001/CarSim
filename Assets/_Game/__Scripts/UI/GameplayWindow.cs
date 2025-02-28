using TMPro;
using UnityEngine;

public class GameplayWindow : UIElement
{
    [SerializeField]
    private TextMeshProUGUI _infoText;

    public void Initialize()
    {
    }

    public void ShowSeeingInfo(IInteractable uiInfo)
    {
        _infoText.text = uiInfo.UiLabel;
        _infoText.gameObject.SetActive(true);
        _ = Show();
    }

    public void HideSeeingInfo()
    {
        _ = Hide();
        _infoText.gameObject.SetActive(false);
    }
}