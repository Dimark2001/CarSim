using Cysharp.Threading.Tasks;
using UnityEngine;

public class Boot : MonoBehaviour
{
    private async void Start()
    {
        await Init();
        //SceneLoader.LoadScene("MainMenu");
    }

    private async UniTask Init()
    {
        await G.Initialize();
    }
}