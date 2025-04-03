using UnityEngine;

namespace _Game.__Scripts.System.DI.Test
{
    [Register]
    public class GameService
    {
        public void LogMessage(string message)
        {
            Debug.Log("[GameService]: " + message);
        }
    }
    
    [Register]
    public class MainService
    {
        public void LogMessage(string message)
        {
            Debug.Log("[MainService]: " + message);
        }
    }
}