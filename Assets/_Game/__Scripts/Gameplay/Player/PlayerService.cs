using Cysharp.Threading.Tasks;
using UnityEngine;

public class PlayerService : ProjectSingleton
{
    public PlayerFacade Player { get; private set; }
    public PlayerStats PlayerStats { get; private set; }

    public override async UniTask Initialize()
    {
        PlayerStats = new PlayerStats();
    }

    public override async UniTask AfterInitialize()
    {
    }

    public void CreatePlayer(Vector3 worldPos)
    {
    }
}