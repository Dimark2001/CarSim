using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public abstract class UIElement : MonoBehaviour
{
    [SerializeField]
    protected CanvasGroup _canvasGroup;

    protected virtual async void Show()
    {
        _canvasGroup.alpha = 1f;
        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.interactable = true;
    }
    
    protected virtual async void Hide()
    {
        _canvasGroup.alpha = 0f;
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.interactable = false;
    }
}