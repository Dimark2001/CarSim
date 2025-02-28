using Cysharp.Threading.Tasks;
using Object = UnityEngine.Object;

public class UIService : GSingleton
{
    public UIFacade UIFacade { get; private set; }

    public override async UniTask Initialize()
    {
        CreateGameplayWindow();
    }

    public override async UniTask AfterInitialize()
    {
        UIFacade.GameplayWindow.Initialize();
    }

    public void CreateGameplayWindow()
    {
        var prefab = GameResources.Prefabs.UI.UIFacade;
        UIFacade = Object.Instantiate(prefab);
    }
}