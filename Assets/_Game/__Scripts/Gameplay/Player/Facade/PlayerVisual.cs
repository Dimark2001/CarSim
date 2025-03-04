using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    [SerializeField]
    private GameObject _visual;

    public void Show()
    {
        _visual.SetActive(true);
    }
    
    public void Hide()
    {
        _visual.SetActive(false);
    }
}