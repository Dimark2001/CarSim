using Cysharp.Threading.Tasks;
using UnityEngine;

public class PlayerService : GSingleton
{
    public PlayerFacade Player { get; private set; }

    public override async UniTask Initialize()
    {
        CreatePlayer(new Vector3(2.968f,0,-2.071f));
        await UniTask.CompletedTask;
    }

    public override async UniTask AfterInitialize()
    {
        Player.Input.Initialize();
        await UniTask.CompletedTask;
    }

    public void CreatePlayer(Vector3 worldPos)
    {
        var prefab = GameResources.Prefabs.Player;
        Player = Object.Instantiate(prefab, worldPos, Quaternion.identity);
    }

    public void Disable()
    {
        Player.Input.enabled = false;
        Player.Trigger.Disable();
        Player.Camera.ResetPosition();
        Player.Movement.ResetPosition();
        Player.Visual.Hide();
    }

    public void Enable()
    {
        Player.Input.enabled = true;
        Player.Trigger.enabled = true;
        Player.Camera.ResetPosition();
        Player.Movement.ResetPosition();
        Player.Visual.Show();
    }
}

