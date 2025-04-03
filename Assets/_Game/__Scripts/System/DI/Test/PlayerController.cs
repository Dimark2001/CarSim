using UnityEngine;

namespace _Game.__Scripts.System.DI.Test
{
    public class PlayerController : MonoBehaviour
    {
        [Inject]
        private GameService _gameService;
        
        [Inject]
        private MainService _mainService;

        private void Start()
        {
            _gameService.LogMessage("PlayerController started!");
            _mainService.LogMessage("PlayerController started!");
        }
    }
}